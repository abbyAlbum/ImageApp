using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private Dictionary<int, CommandEnum> commands;
        private IList<DirectoyHandler> handlers;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public void AddHandlerToPath(string path)
        {
            handlers.Add(new DirectoyHandler(m_controller, m_logging, path));
        }

        public void CloseHandlers()
        {
            for(int i = 0;i < handlers.Count; i++)
            {
                handlers.ElementAt(i).;
            }
        }

        public void SendCommandToController()
        {

        }
       
    }
}
