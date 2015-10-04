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
using Reply.Cluster.Akka.Messages;

namespace Reply.Cluster.Akka.Actors
{
    public abstract class BaseActor : UntypedActor, IActorContext
    {
        protected override void OnReceive(object message)
        {
            Execute(message);

            Context.Parent.Tell(new Complete());
        }

        protected abstract void Execute(object message);

        protected void PutMessage(object message)
        {
            Context.Parent.Tell(message);
        }

        #region IActorContext Members

        void IActorContext.PutMessage(object message)
        {
            PutMessage(message);
        }

        #endregion
    }
}
