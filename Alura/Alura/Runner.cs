using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alura
{
    public class Runner
    {
        public static void Horas()
        {
            Speaker.Speak(DateTime.Now.ToShortTimeString());
        }
        public static void Datas()
        {
            Speaker.Speak(DateTime.Now.ToShortDateString());
        }
    }
}
