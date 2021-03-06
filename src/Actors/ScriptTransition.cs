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
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reply.Cluster.Akka.Messages;

namespace Reply.Cluster.Akka.Actors
{
    internal class ScriptTransition : Transition
    {
        private string condition;
        private LuaFunction conditionFunction;

        public ScriptTransition(string source, string destination, string condition) :
            base(source, destination)
        {
            this.condition = condition;
        }

        public ScriptTransition(string name, string source, string destination, string condition) : 
            base(name, source, destination)
        {
            this.condition = condition;
        }

        protected internal override void AddToActor(CompositeActor actor)
        {
            base.AddToActor(actor);

            string functionName = $"checkTransition{Name}";
            conditionFunction = actor.ScriptSystem.LoadString($"return {condition}", functionName);
        }

        protected internal override void RemoveFromActor(CompositeActor actor)
        {
            conditionFunction.Dispose();

            base.RemoveFromActor(actor);
        }

        protected internal override bool Check(Message message)
        {
            Actor.ScriptSystem["message"] = message;

            return conditionFunction.Call().First() as bool? ?? false;
        }
    }
}
