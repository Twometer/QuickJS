using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickJS.JavaScript
{
    public class JsExecutor
    {
        private JavascriptContext javascriptContext;
        private Action<string> callback;

        public void Initialize(Action<string> consoleCallback)
        {
            Console console = new Console();
            console.Listener = consoleCallback;
            callback = consoleCallback;
            javascriptContext = new JavascriptContext();
            javascriptContext.SetParameter("console", console);
        }

        public void Execute(string script)
        {
            var i = javascriptContext.Run(script);
            if (i != null)
                callback(i.ToString());
        }

    }
}
