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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Reply.Cluster.Akka.Actors
{
    /// <summary>
    /// Actor that executes a function, sending back the response to the parent.
    /// </summary>
    public abstract class ActionActor : ExecutingActor
    {
        private Action<object, IActorContext, object[]> action;
        private object[] args;

        public ActionActor(Action<object, IActorContext, object[]> action, params object[] args)
        {
            this.action = action;
            this.args = args;
        }

        protected override void Execute(object message)
        {
            action(message, this, args);
        }
    }
}
