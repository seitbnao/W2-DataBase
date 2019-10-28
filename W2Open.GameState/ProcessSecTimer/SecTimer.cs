using System;
using W2Open.Common;
using W2Open.Common.Utility;
namespace W2Open.GameState.ProcessSecTimer
{
    public class SecTimer
    {
        public static int Sec = 1;


        public static void Start(DBController gs,pServer Server)
        {
            if(Sec % 15 == 0)//15 segundos
            {
               // ProcessImportItem.Start(gs);

                W2Log.SendUpdate();
            }
            if (Sec % 5 == 0)//5 segundos
            {
                ProcessImportItem.Start(gs);
            
            }


            if (Sec++ > 1000)
                Sec = 0;
        }

    
    }
}
