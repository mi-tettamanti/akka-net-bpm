using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Messages
{
    public class ToBeCompleted : MessageBase
    {
        public ToBeCompleted(Guid messageId, string correlationID)
        {
            MessageId = messageId;
            CorrelationID = correlationID;
        }
    }
}
