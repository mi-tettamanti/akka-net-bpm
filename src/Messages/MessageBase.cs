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
using Akka.Routing;

namespace Reply.Cluster.Akka.Messages
{
    public abstract class MessageBase : IConsistentHashable
    {
        protected MessageBase() : this(Guid.NewGuid(), Guid.NewGuid().ToString()) { }

        protected MessageBase(Guid messageId) : this(messageId, Guid.NewGuid().ToString()) { }

        protected MessageBase(string correlationID) : this(Guid.NewGuid(), correlationID) { }

        protected MessageBase(Guid messageId, string correlationID)
        {
            MessageId = messageId;
            CorrelationID = correlationID;
        }

        public virtual string CorrelationID { get; }

        public Guid MessageId { get; protected set; } = Guid.NewGuid();

        #region IConsistentHashable Members

        object IConsistentHashable.ConsistentHashKey
        {
            get { return CorrelationID; }
        }

        #endregion
    }
}
