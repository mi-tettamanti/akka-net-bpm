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

        private Lua state;
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
            state["message"] = message;
            execution.Call();
        }

        protected override void PreStart()
        {
            state = new Lua();

            state["context"] = this;
            state["args"] = args;

            state.RegisterFunction("createMessage", this.GetType().GetMethod(nameof(CreateMessage)));
            state.RegisterFunction("putMessage", this, this.GetType().GetMethod(nameof(PutMessage)));

            execution = state.LoadString(script, "execute");

            base.PreStart();
        }

        protected override void PostStop()
        {
            base.PostStop();

            state.Dispose();
        }
    }
}
