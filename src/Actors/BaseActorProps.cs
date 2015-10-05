using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Reply.Cluster.Akka.Actors
{
    public class BaseActorProps : Props
    {
        private Dictionary<string, Transition> inboundTransitions = new Dictionary<string, Transition>();
        private Dictionary<string, Transition> outboundTransitions = new Dictionary<string, Transition>();

        protected BaseActorProps(Type type, object[] args)
            : base(type, args)
        { }

        public static BaseActorProps Create<T>(params object[] args) where T : BaseActor
        {
            return new BaseActorProps(typeof(T), args);
        }

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
            var actor = base.NewActor() as BaseActor;

            actor.InternalInboundTransitions = inboundTransitions;
            actor.InternalOutboundTransitions = outboundTransitions;

            return actor;
        }
    }
}
