using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reply.Cluster.Akka.Messages;
using NLua;

namespace Reply.Cluster.Akka.Actors
{
    public class ScriptActor : ExecutingActor
    {
        private object[] args;
        private string script;

        private LuaFunction execution;

        public ScriptActor(string script, params object[] args)
        {
            this.script = script;
            this.args = args;
        }

        public static Message CreateMessage(Message input)
        {
            return Factory.CreateMessage(input);
        }

        protected override void Execute(Message message)
        {
            ScriptSystem["message"] = message;
            execution.Call();
        }

        protected override void PreStart()
        {
            base.PreStart();

            ScriptSystem["args"] = args;

            ScriptSystem.RegisterFunction("createMessage", this.GetType().GetMethod(nameof(CreateMessage)));
            ScriptSystem.RegisterFunction("putMessage", this, this.GetType().GetMethod(nameof(PutMessage)));

            execution = ScriptSystem.LoadString(script, "execute");
        }

        protected override void PostStop()
        {
            execution.Dispose();

            base.PostStop();
        }
    }
}
