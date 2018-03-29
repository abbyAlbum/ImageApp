using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Logging.Modal;

namespace ImageService.ImageService_Logging.Model
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        public MessageRecievedEventArgs(MessageTypeEnum status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
