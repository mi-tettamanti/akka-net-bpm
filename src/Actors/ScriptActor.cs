using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reply.Cluster.Akka.Messages;
using NLua;

namespace Reply.Cluster.Akka.Actors
{
    public class ScriptActor : ActionActor
    {
        public ScriptActor(string script, params object[] args) : base(
            (m, c, a) =>
            {
                using (var state = new Lua())
                {
                    state["message"] = m;
                    state["context"] = c;
                    state["args"] = a;

                    state.RegisterFunction("createMessage", typeof(ScriptActor).GetMethod("CreateMessage"));// (new Func<Message, Message>(message => Factory.CreateMessage(m))).Method);

                    state.DoString(script);
                }
            }, args)
        { }

        public static Message CreateMessage(Message input)
        {
            return Factory.CreateMessage(input);
        }
    }
}
