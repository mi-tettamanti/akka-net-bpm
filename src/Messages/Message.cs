using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Messages
{
    public class Message : MessageBase
    {
        internal Message() : base(Guid.NewGuid().ToString()) { }
        internal Message(string correlationID) : base(correlationID) { }

        internal Message(Message message)
            : this(message.CorrelationID)
        {
            Type = message.Type;

            foreach (string property in message.Properties.Keys)
                Properties[property] = message.Properties[property];
        }

        public virtual void ClearBody() { }

        public void ClearProperties()
        {
            Properties.Clear();
        }

        public string Type { get; set; }

        public DateTime Timestamp { get; } = DateTime.Now;
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();
    }
}
