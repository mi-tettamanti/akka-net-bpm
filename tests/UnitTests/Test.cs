using Akka.Actor;
using MbUnit.Framework;
using Reply.Cluster.Akka.Actors;
using Reply.Cluster.Akka.Messages;
using Reply.Cluster.Akka.Engine;
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

                        return Factory.CreateMessage(m);
                    })))
                .AddChild("world", ActorFactory.CreateActor<ActionActor>(new Action<Message, Reply.Cluster.Akka.Actors.IActorContext, object[]>((m, a, p) =>
                    {
                        Console.WriteLine("World.");
                    })))
                .AddTransition(string.Empty, "hello", t => true)
                .AddTransition("hello", "world", t => true)
                .Complete();

            var rootActor = system.CreateTestActor(root);
            rootActor.Tell(Factory.CreateMessage());

            system.AwaitTermination();
            
            Assert.IsTrue(true);
        }

        [Test]
        public void HelloWorldWithSystem()
        {
            var system = Reply.Cluster.Akka.Engine.System.Create("MyActorSystem");

            var root = ActorFactory.CreateContainer()
                .AddChild("hello", ActorFactory.CreateActor<FunctionActor>(new Func<Message, Reply.Cluster.Akka.Actors.IActorContext, object[], Message>((m, a, p) =>
                {
                    Console.Write("Hello ");

                    return Factory.CreateMessage(m);
                })))
                .AddChild("world", ActorFactory.CreateActor<ActionActor>(new Action<Message, Reply.Cluster.Akka.Actors.IActorContext, object[]>((m, a, p) =>
                {
                    Console.WriteLine("World.");
                })))
                .AddTransition(string.Empty, "hello", t => true)
                .AddTransition("hello", "world", t => true)
                .Complete();

            system.RegisterProcess("helloWorld", root);
            system.Tell("helloWorld", Factory.CreateMessage());

            system.AwaitTermination();

            Assert.IsTrue(true);
        }
    }
}
