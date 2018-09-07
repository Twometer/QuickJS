using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickJS.JavaScript
{
    public class Console
    {
        public Action<string> Listener { get; set; }

        public void log(string message)
        {
            Listener(message);
        }
    }
}
