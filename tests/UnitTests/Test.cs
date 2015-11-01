using Akka.Actor;
using MbUnit.Framework;
using Reply.Cluster.Akka.Actors;
using Reply.Cluster.Akka.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void HelloWorld()
        {
            var system = ActorSystem.Create("MyActorSystem");

            var root = ActorFactory.CreateContainer()
                .AddChild("hello", ActorFactory.CreateActor<FunctionActor>(new Func<Message, Reply.Cluster.Akka.Actors.IActorContext, object[], Message>((m, a, p) => 
                    {
                        Console.Write("Hello ");

                        return Factory.CreateMessage();
                    })))
                .AddChild("world", ActorFactory.CreateActor<ActionActor>(new Action<Message, Reply.Cluster.Akka.Actors.IActorContext, object[]>((m, a, p) =>
                    {
                        Console.WriteLine("World.");
                    })))
                .AddTransition(string.Empty, "hello", new Func<object, bool>(t => true))
                .AddTransition("hello", "world", new Func<object,bool>(t => true))
                .Complete();

            var rootActor = system.CreateTestActor(root);
            rootActor.Tell(Factory.CreateMessage());

            system.AwaitTermination();
            
            Assert.IsTrue(true);
        }
    }
}
