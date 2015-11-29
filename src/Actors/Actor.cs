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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Reply.Cluster.Akka.Messages;
using NLua;

namespace Reply.Cluster.Akka.Actors
{
    public abstract class Actor : ReceiveActor, IActorContext
    {
        private Dictionary<string, Transition> inboundTransitions = new Dictionary<string, Transition>();
        private Dictionary<string, Transition> outboundTransitions = new Dictionary<string, Transition>();

        private Lua scriptSystem;

        public Actor()
        {
            Receive<Message>(message => 
            {
                CorrelationID = message.CorrelationID;

                ProcessMessage(message);
            });
        }

        internal Lua ScriptSystem
        {
            get
            {
                if (scriptSystem == null)
                {
                    scriptSystem = new Lua();

                    scriptSystem["context"] = this;
                }

                return scriptSystem;
            }
        }

        protected override void PostStop()
        {
            if (scriptSystem != null)
            {
                scriptSystem.Dispose();
                scriptSystem = null;
            }

            base.PostStop();
        }

        protected abstract bool ProcessMessage(Message message);

        internal void Complete(Guid messageId)
        {
            Context.Parent.Tell(new Complete(messageId, CorrelationID));
        }

        #region IActorContext Members

        public void PutMessage(Message message)
        {
            Context.Parent.Tell(message);
        }

        public string CorrelationID { get; private set; }

        public IEnumerable<Transition> InboundTransitions => inboundTransitions.Values;
        public IEnumerable<Transition> OutboundTransitions => outboundTransitions.Values; 

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
