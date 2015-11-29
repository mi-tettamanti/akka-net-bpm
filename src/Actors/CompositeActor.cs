#region Copyright
/*
Copyright 2015 Cluster Reply s.r.l.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Reply.Cluster.Akka.Messages;
using NLua;

namespace Reply.Cluster.Akka.Actors
{
    /// <summary>
    /// Actor that coordinates multiple child actors.
    /// It has a list of registered children and transitions. When it receives a messages, it evaluates the transitions to decide where to send it.
    /// </summary>
    public class CompositeActor : Actor
    {
        private Dictionary<string, ActorPath> children = new Dictionary<string, ActorPath>();
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        private Dictionary<string, List<Transition>> actorTransitions = new Dictionary<string, List<Transition>>();

        private IActorRef parent;
        private Guid originalMessageId;

        private Dictionary<string, HashSet<Guid>> processingMessages = new Dictionary<string, HashSet<Guid>>();

        internal Dictionary<string, Transition> Transitions { set { transitions = value; } }
        internal Dictionary<string, List<Transition>> ActorTransitions { set { actorTransitions = value; } }

        public CompositeActor()
            : base()
        {
            Receive<Complete>(message => CompleteMessage(message));
            Receive<ToBeCompleted>(message => Complete(originalMessageId));
        }

        /// <summary>
        /// To be implemented by concrete <see cref="UntypedActor"/>, this defines the behavior of the <see cref="UntypedActor"/>. This method is called for every message received by the actor.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override bool ProcessMessage(Message message)
        {
            if (message == null)
                return false;

            if (parent == null)
            {
                parent = Sender;
                originalMessageId = message.MessageId;
            }

            string source = Sender.Path.Name;
            var targets = new List<string>();

            if (!children.ContainsKey(source))
                source = string.Empty;
            
            if (actorTransitions.ContainsKey(source))
                foreach (var transition in actorTransitions[source])
                    if (transition.Check(message))
                        targets.Add(transition.Destination);

            if (targets.Count == 0)
                return false;
            else
                foreach (string target in targets)
                {
                    var targetChild = children[target];
                    var targetMessage = Messages.Factory.CreateMessage(message);

                    Context.ActorSelection(targetChild).Tell(targetMessage, Sender);

                    if (!processingMessages.ContainsKey(CorrelationID))
                        processingMessages[CorrelationID] = new HashSet<Guid>();

                    processingMessages[CorrelationID].Add(targetMessage.MessageId);
                }

            return true;
        }

        private bool CompleteMessage(Complete completionMessage)
        {
            if (parent == null)
                return false;

            Guid messageId = completionMessage.MessageId;
            
            if (!processingMessages.ContainsKey(CorrelationID))
                return false;

            var relatedProcessingMessages = processingMessages[CorrelationID];

            if (!relatedProcessingMessages.Contains(messageId))
                return false;

            relatedProcessingMessages.Remove(messageId);

            if (relatedProcessingMessages.Count == 0)
            {
                processingMessages.Remove(CorrelationID);

                Self.Tell(new ToBeCompleted(messageId, CorrelationID), parent);
            }

            return true;
        }

        /// <summary>
        /// Adds a child to the <see cref="CompositeActor"/>.
        /// </summary>
        /// <param name="actorProps"><see cref="Props"/> used to create the child.</param>
        /// <param name="name">Child name. Must be unique.</param>
        internal void AddChild(ActorProps actorProps, string name)
        {
            var child = Context.ActorOf(actorProps, name);
            children[name] = child.Path;
        }

        protected override void PreStart()
        {
            base.PreStart();

            foreach (var transition in transitions.Values)
                transition.AddToActor(this);
        }

        protected override void PostStop()
        {
            foreach (var transition in transitions.Values)
                transition.RemoveFromActor(this);

            base.PostStop();
        }
    }
}
