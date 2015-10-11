using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Messages
{
    public class Message
    {
        internal Message()
        {
            MessageId = Guid.NewGuid();
            Timestamp = DateTime.Now;
            Properties = new Dictionary<string, object>();
        }

        internal Message(Message message)
            : this()
        {
            CorrelationID = message.CorrelationID;
            Type = message.Type;

            foreach (string property in message.Properties.Keys)
                Properties[property] = message.Properties[property];
        }

        public virtual void ClearBody() { }

        public void ClearProperties()
        {
            Properties.Clear();
        }

        public virtual string CorrelationID { get; set; }

        public Guid MessageId { get; private set; }

        public DateTime Timestamp { get; private set; }

        public string Type { get; set; }

        public Dictionary<string, object> Properties { get; private set; }
    }
}
