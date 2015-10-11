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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Reply.Cluster.Akka.Messages;

namespace Reply.Cluster.Akka.Actors
{
    public abstract class Actor : ReceiveActor, IActorContext
    {
        private Dictionary<string, Transition> inboundTransitions = new Dictionary<string, Transition>();
        private Dictionary<string, Transition> outboundTransitions = new Dictionary<string, Transition>();

        protected void PutMessage(Message message)
        {
            Context.Parent.Tell(message);
        }

        internal void Complete()
        {
            Context.Parent.Tell(new Complete());
        }

        #region IActorContext Members

        void IActorContext.PutMessage(Message message)
        {
            PutMessage(message);
        }

        public IEnumerable<Transition> InboundTransitions
        {
            get { return inboundTransitions.Values; }
        }

        public IEnumerable<Transition> OutboundTransitions
        {
            get { return outboundTransitions.Values; }
        }

        #endregion

        public Dictionary<string, Transition> InternalInboundTransitions
        {
            set { inboundTransitions = value; }
        }

        public Dictionary<string, Transition> InternalOutboundTransitions
        {
            set { outboundTransitions = value; }
        }
    }
}