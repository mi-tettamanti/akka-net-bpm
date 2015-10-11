using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Messages
{
    public static class Factory
    {
        public static Message CreateMessage()
        {
            return new Message();
        }

        public static Message CreateMessage(Message message)
        {
            return new Message(message);
        }
    }
}
