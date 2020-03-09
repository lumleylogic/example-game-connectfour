using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectFour.UI;

namespace ConnectFour
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectUI = new ConnectUI();
            connectUI.Init();
            connectUI.Start();
        }
    }
}
