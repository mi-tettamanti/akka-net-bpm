using Akka.Actor;
using Reply.Cluster.Akka.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public static class Helpers
    {
        public static IActorRef CreateTestActor(this ActorSystem system, ActorProps actorToBeTested)
        {
            return system.ActorOf(ActorProps.Create<TestManagerActor>(actorToBeTested));
        }
    }
}
