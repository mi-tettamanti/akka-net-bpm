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

namespace Reply.Cluster.Akka.Actors
{
    /// <summary>
    /// Actor that coordinates multiple child actors.
    /// It has a list of registered children and transitions. When it receives a messages, it evaluates the trasitions to decide where to send it.
    /// </summary>
    public class CompositeActor : BaseActor
    {
        private Dictionary<string, ActorPath> children = new Dictionary<string, ActorPath>();
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        private Dictionary<string, List<Transition>> actorTransitions = new Dictionary<string, List<Transition>>();

        internal Dictionary<string, Transition> Transitions { set { transitions = value; } }
        internal Dictionary<string, List<Transition>> ActorTransitions { set { actorTransitions = value; } }

        /// <summary>
        /// To be implemented by concrete <see cref="UntypedActor"/>, this defines the behavior of the <see cref="UntypedActor"/>. This method is called for every message received by the actor.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void OnReceive(object message)
        {
            string source = Sender.Path.Name;
            var targets = new List<string>();

            if (!children.ContainsKey(source))
                source = string.Empty;
            
            if (actorTransitions.ContainsKey(source))
                foreach (var transition in actorTransitions[source])
                    if (transition.Condition(message))
                        targets.Add(transition.Destination);

            if (targets.Count == 0)
                Unhandled(message);
            else
                foreach (string target in targets)
                    Context.ActorSelection(children[target]).Tell(message, Sender);
        }

        /// <summary>
        /// Adds a child to the <see cref="CompositeActor"/>.
        /// </summary>
        /// <param name="actorProps"><see cref="Props"/> used to create the child.</param>
        /// <param name="name">Child name. Must be unique.</param>
        internal void AddChild(BaseActorProps actorProps, string name)
        {
            var child = Context.ActorOf(actorProps, name);
            children[name] = child.Path;
        }
    }
}
