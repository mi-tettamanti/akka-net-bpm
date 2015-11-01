using Akka.Actor;
using Reply.Cluster.Akka.Actors;
using Reply.Cluster.Akka.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    internal class TestManagerActor : ReceiveActor
    {
        IActorRef actorToBeTested;

        public TestManagerActor(ActorProps actorToBeTested)
        {
            this.actorToBeTested = Context.ActorOf(actorToBeTested);

            Receive<Message>(m => this.actorToBeTested.Tell(m));
            Receive<Complete>(m => Context.Parent.Tell(Kill.Instance));
        }
    }
}
