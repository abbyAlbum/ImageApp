using System;

using ImageService.ImageService_Logging.Model;
using ImageService.Logging.Modal;

namespace ImageService
{
    public interface ILoggingService
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        void Log(string message, MessageTypeEnum type);           // Logging the Message
    }
}
