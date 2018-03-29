using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.ImageService_Logging.Model;
using ImageService.Logging.Modal;

namespace ImageService
{
    class LoggingModal : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;


        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs msg = new MessageRecievedEventArgs(type, message);
            MessageRecieved.Invoke(this, msg);
        }
    }

}

