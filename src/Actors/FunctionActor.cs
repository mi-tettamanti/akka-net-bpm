using Reply.Cluster.Akka.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Actors
{
    public class FunctionActor : ProcessingActor
    {
        private Func<Message, IActorContext, object[], Message> function;
        private object[] args;

        public FunctionActor(Func<Message, IActorContext, object[], Message> function, params object[] args)
        {
            this.function = function;
            this.args = args;
        }

        protected override Message Process(Message message)
        {
            return function(message, this, args);
        }
    }
}
