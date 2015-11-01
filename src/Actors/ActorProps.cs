using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Reply.Cluster.Akka.Actors
{
    public class ActorProps : Props
    {
        private bool completed = false;

        private Dictionary<string, Transition> inboundTransitions = new Dictionary<string, Transition>();
        private Dictionary<string, Transition> outboundTransitions = new Dictionary<string, Transition>();

        protected internal ActorProps(Type type, object[] args)
            : base(type, args)
        { }

        internal void AddInboundTransition(Transition transition)
        {
            inboundTransitions[transition.Name] = transition;
        }

        internal void AddOutboundTransition(Transition transition)
        {
            outboundTransitions[transition.Name] = transition;
        }

        public override ActorBase NewActor()
        {
            if (!completed)
                throw new InvalidOperationException("Props initialization must be completed before generating a new actor.");

            var actor = base.NewActor() as Actor;

            actor.InternalInboundTransitions = inboundTransitions;
            actor.InternalOutboundTransitions = outboundTransitions;

            return actor;
        }

        public virtual ActorProps Complete()
        {
            if (completed)
                throw new InvalidOperationException("Props initialization is already completed.");

            completed = true;

            return this;
        }
    }
}
