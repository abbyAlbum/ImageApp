using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ImageService
{
    class LoggingModel
    {
        public event OnMsgEvent Log;

        public void OnLog(string msg)
        {
            
            Log.Invoke(this, msg);
            
        }
    }

    public delegate void OnMsgEvent(object sender, string msg); 
}

