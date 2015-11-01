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
using Akka.Actor;

namespace Reply.Cluster.Akka.Actors
{
    public class CompositeActorProps : ActorProps
    {
        private Dictionary<string, ActorProps> children = new Dictionary<string, ActorProps>();
        private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        private Dictionary<string, List<Transition>> actorTransitions = new Dictionary<string, List<Transition>>();

        protected internal CompositeActorProps(Type type, object[] args)
            : base(type, args)
        { }

        /// <summary>
        /// Adds a child to the <see cref="CompositeActor"/>.
        /// </summary>
        /// <param name="actorProps"><see cref="Props"/> used to create the child.</param>
        /// <param name="name">Child name. Must be unique.</param>
        /// <returns><see cref="IActorRef"/> corresponding to the newly created child.</returns>
        /// <exception cref="ArgumentException">A child with the same name was already added.</exception>
        public CompositeActorProps AddChild(string name, ActorProps actorProps)
        {
            if (children.ContainsKey(name))
                throw new ArgumentException(string.Format("A child with name \"{0}\" was already added.", name), "name");

            children[name] = actorProps;

            return this;
        }

        /// <summary>
        /// Adds a transition between two children of the <see cref="CompositeActor"/>. The transition name is automatically generated.
        /// </summary>
        /// <param name="from">Name of the source child for the transition. If empty, the transition starts from outside the <see cref="CompositeActor"/>.</param>
        /// <param name="to">Name of the target child for the transition. Must have a value.</param>
        /// <param name="condition">Condition that is evaluated on the message. If the condition is true, the message is sent to the target child.</param>
        /// <exception cref="ArgumentException">Source or target child names are not registered on the <see cref="CompositeActor"/>.</exception>
        /// <exception cref="ArgumentException">Target child name has not a vale.</exception>
        /// <exception cref="ArgumentException">A transition with the same name was already added.</exception>
        public CompositeActorProps AddTransition(string from, string to, Func<object, bool> condition)
        {
            return AddTransition(new Transition(Guid.NewGuid().ToString(), from, to, condition));
        }

        /// <summary>
        /// Adds a transition between two children of the <see cref="CompositeActor"/>.
        /// </summary>
        /// <param name="name">Transition name. Must be unique.</param>
        /// <param name="from">Name of the source child for the transition. If empty, the transition starts from outside the <see cref="CompositeActor"/>.</param>
        /// <param name="to">Name of the target child for the transition. Must have a value.</param>
        /// <param name="condition">Condition that is evaluated on the message. If the condition is true, the message is sent to the target child.</param>
        /// <exception cref="ArgumentException">Source or target child names are not registered on the <see cref="CompositeActor"/>.</exception>
        /// <exception cref="ArgumentException">Target child name has not a vale.</exception>
        /// <exception cref="ArgumentException">A transition with the same name was already added.</exception>
        public CompositeActorProps AddTransition(string name, string from, string to, Func<object, bool> condition)
        {
            return AddTransition(new Transition(name, from, to, condition));
        }

        /// <summary>
        /// Adds a transition between two children of the <see cref="CompositeActor"/>.
        /// </summary>
        /// <param name="transition"><see cref="Transition"/> to be added.</param>
        /// <exception cref="ArgumentException">Source or target child names are not registered on the <see cref="CompositeActor"/>.</exception>
        /// <exception cref="ArgumentException">Target child name has not a vale.</exception>
        /// <exception cref="ArgumentException">A transition with the same name was already added.</exception>
        public CompositeActorProps AddTransition(Transition transition)
        {
            if (!string.IsNullOrEmpty(transition.Source) && !children.ContainsKey(transition.Source))
                throw new ArgumentException(string.Format("There is not a registered child with name \"{0}\".", transition.Source), "transition");

            if (string.IsNullOrEmpty(transition.Destination))
                throw new ArgumentException("Target child name must have a value.", "transition");

            if (!children.ContainsKey(transition.Destination))
                throw new ArgumentException(string.Format("There is not a registered child with name \"{0}\".", transition.Destination), "transition");

            if (transitions.ContainsKey(transition.Name))
                throw new ArgumentException(string.Format("A transition with name \"{0}\" was already added.", transition.Name), "transition");

            if (!actorTransitions.ContainsKey(transition.Source))
                actorTransitions[transition.Source] = new List<Transition>();

            actorTransitions[transition.Source].Add(transition);
            transitions[transition.Name] = transition;

            if (!string.IsNullOrEmpty(transition.Source))
                children[transition.Source].AddOutboundTransition(transition);
            children[transition.Destination].AddInboundTransition(transition);

            return this;
        }

        public override ActorProps Complete()
        {
            foreach (var name in children.Keys)
                children[name].Complete();

            base.Complete();

            return this;
        }

        public override ActorBase NewActor()
        {
            var actor = base.NewActor() as CompositeActor;

            foreach (var name in children.Keys)
                actor.AddChild(children[name], name);

            actor.Transitions = transitions;
            actor.ActorTransitions = actorTransitions;

            return actor;
        }
    }
}
