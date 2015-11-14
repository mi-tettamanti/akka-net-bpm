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
using Akka.Routing;
using Reply.Cluster.Akka.Actors;
using Reply.Cluster.Akka.Messages;

namespace Reply.Cluster.Akka.Engine
{
    public class System
    {
        private ActorSystem actorSystem;
        private Dictionary<string, ActorPath> processes = new Dictionary<string, ActorPath>();

        private System(string name)
        {
            actorSystem = ActorSystem.Create(name);
        }

        public static System Create(string name = "akka-net-bpm")
        {
            return new System(name);
        }

        public void RegisterProcess(string name, ActorProps props)
        {
            var process = actorSystem.ActorOf(props.WithRouter(new ConsistentHashingPool(5)), name);

            processes[name] = process.Path;
        }

        public void Tell(string process, Message message)
        {
            actorSystem.ActorSelection(processes[process]).Tell(message);
        }

        public void AwaitTermination()
        {
            actorSystem.AwaitTermination();
        }
    }
}
