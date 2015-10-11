using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Actors
{
    public static class ActorFactory
    {
        public static ActorProps CreateActor<T>(params object[] args) where T : Actor
        {
            return new ActorProps(typeof(T), args);
        }

        public static CompositeActorProps CreateContainer<T>(params object[] args) where T : CompositeActor
        {
            return new CompositeActorProps(typeof(T), args);
        }

        public static CompositeActorProps CreateContainer(params object[] args)
        {
            return CreateContainer<CompositeActor>();
        }
    }
}
