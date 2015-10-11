using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Messages
{
    public class Message
    {
        public Message() : this(Guid.NewGuid().ToString()) { }

        public Message(string messageId)
        {
            MessageId = messageId;
            Timestamp = DateTime.Now;
            Properties = new Dictionary<string, object>();
        }

        public virtual void ClearBody() { }

        public void ClearProperties()
        {
            Properties.Clear();
        }

        public virtual string CorrelationID { get; set; }

        public string MessageId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string Type { get; set; }

        public Dictionary<string, object> Properties { get; private set; }
    }
}
