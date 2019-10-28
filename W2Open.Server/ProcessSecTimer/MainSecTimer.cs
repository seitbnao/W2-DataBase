using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using W2Open.Common.Utility;

namespace W2Open.Server.ProcessSecTimer
{
    public class MainSecTimer
    {
        public static async Task MainTask()
        {
            while (true)
            {
               
                await Task.Delay(1000);
            }
        }


    }
}
