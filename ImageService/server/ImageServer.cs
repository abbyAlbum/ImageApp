using ImageService.Commands;
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
        #endregion


        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(IImageController image, ILoggingService logging)
        {
            m_controller = image;
            m_logging = logging;
            commands = new Dictionary<int, CommandEnum> { };
            commands.Add(0, CommandEnum.CloseCommand);
        }
       
        public void CreateHandler(string path)
        {
            IDirectoryHandler h = new DirectoyHandler(m_controller, m_logging, path);
            
            CommandRecieved += h.OnCommandRecieved;
            h.DirectoryClose += OnCloseServer;
        }

        public void OnCloseServer(Object sender, DirectoryCloseEventArgs args)
        {
            IDirectoryHandler h = (IDirectoryHandler)sender;
            CommandRecieved -= h.OnCommandRecieved;
            h.DirectoryClose -= OnCloseServer;
            m_logging.Log(args.DirectoryPath + args.Message, Logging.Modal.MessageTypeEnum.INFO);
        }

        public void SendCommandToController()
        {
            string[] args = null;
            CommandRecieved("*", new CommandRecievedEventArgs(1, args, ""));
        }
       
    }
}
