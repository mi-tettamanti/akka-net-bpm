﻿#region Copyright
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
using Reply.Cluster.Akka.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Actors
{
    public abstract class Transition
    {
        protected internal Transition(string source, string destination) : 
            this(Guid.NewGuid().ToString("N").ToUpper(), source, destination)
        { }

        protected internal Transition(string name, string source, string destination)
        {
            Name = name;
            Source = source;
            Destination = destination;
        }

        public string Name { get; }

        public string Source { get; }
        public string Destination { get; }


        protected CompositeActor Actor { get; private set; }

        protected internal abstract bool Check(Message message);

        protected internal virtual void AddToActor(CompositeActor actor)
        {
            Actor = actor;
        }

        protected internal virtual void RemoveFromActor(CompositeActor actor)
        {
            Actor = null;
        }
    }
}
