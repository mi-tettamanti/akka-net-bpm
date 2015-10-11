using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.Cluster.Akka.Actors
{
    public class FunctionActor : ProcessingActor
    {
        private Func<object, IActorContext, object[], object> function;
        private object[] args;

        public FunctionActor(Func<object, IActorContext, object[], object> function, params object[] args)
        {
            this.function = function;
            this.args = args;
        }

        protected override object Process(object message)
        {
            return function(message, this, args);
        }
    }
}
