using ImageService.Commands;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.ImageService.commands;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private Dictionary<int, CommandEnum> commands;
        /* Initializes the Listener */
        TcpListener myList = new TcpListener(IPAddress.Parse("127.0.0.1"), 8000);
        #endregion


        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion
        /// <summary>
        /// Creates the ImagerServer.
        /// Param: IImageController, ILoggingService.
        /// </summary>
        public ImageServer(IImageController image, ILoggingService logging)
        {
            m_controller = image;
            m_logging = logging;
            commands = new Dictionary<int, CommandEnum> { };
            commands.Add(0, CommandEnum.CloseCommand);
        }
        /// <summary>
        /// Creates the directory handler that listens for images to be added (and eventually adds them).
        /// Param: the path 
        /// </summary>
        public void CreateHandler(string path)
        {
            IDirectoryHandler h = new DirectoyHandler(m_controller, m_logging, path);
            
            CommandRecieved += h.OnCommandRecieved;
            h.DirectoryClose += OnCloseServer;
        }
        /// <summary>
        /// Closes the server and logs the result.
        /// Param: Object sender, DirectoryCloseEventArgs args.
        /// </summary>
        public void OnCloseServer(Object sender, DirectoryCloseEventArgs args)
        {
            IDirectoryHandler h = (IDirectoryHandler)sender;
            CommandRecieved -= h.OnCommandRecieved;
            h.DirectoryClose -= OnCloseServer;
            myList.Stop();
            m_logging.Log(args.DirectoryPath + args.Message, Logging.Modal.MessageTypeEnum.INFO);
        }
        /// <summary>
        /// Sends the command to the controller, that then executes the command.
        /// </summary>
        public void SendCommandToController()
        {
            string[] args = null;
            CommandRecieved("*", new CommandRecievedEventArgs(3, args, ""));
        }

        public void StartServer()
        {
            try
            {

                /* Start Listeneting at the specified port */
                myList.Start();
                m_logging.Log("The server is running at port 8001...", Logging.Modal.MessageTypeEnum.INFO);
                m_logging.Log("The local End point is  :" +
                                  myList.LocalEndpoint, Logging.Modal.MessageTypeEnum.INFO);
                m_logging.Log("Waiting for a connection.....", Logging.Modal.MessageTypeEnum.INFO);
                int value = 0;
                while (true)
                {
                    if (value == 5) value = 0;
                    Socket s = myList.AcceptSocket();
                    Thread t = new Thread(() => ReadCommand(s, value));
                    t.Start();
                    value++;

                }


                
                /*Socket sk = myList.AcceptSocket();
                m_logging.Log("Connection accepted from " + sk.RemoteEndPoint, Logging.Modal.MessageTypeEnum.INFO);
                s.Send(asen.GetBytes(ConfigurationManager.AppSettings["SourceName"]));
                m_logging.Log("\nSent Acknowledgement", Logging.Modal.MessageTypeEnum.INFO);

                /* clean up */
               /* sk.Close();
                myList.Stop();
                m_logging.Log("closed", Logging.Modal.MessageTypeEnum.INFO);*/

            }
            catch (Exception e)
            {
                m_logging.Log("Error..... " + e.StackTrace, Logging.Modal.MessageTypeEnum.FAIL);
            }
        }
        private void ReadCommand(Socket s, int value)
        {
            byte[] b = new byte[100];
            int k = s.Receive(b);
            m_logging.Log("Recieved...", Logging.Modal.MessageTypeEnum.INFO);
            m_logging.Log(Convert.ToChar(b[0]).ToString(), Logging.Modal.MessageTypeEnum.INFO);
            int i = Convert.ToChar(b[0]);
            m_logging.Log(i.ToString(), Logging.Modal.MessageTypeEnum.INFO);
            switch (i)
            {
                case 49:
                    ICommand c = new GetConfigCommand(s, value);
                    c.Execute(null, out bool result);
                    break;
                case 50:
                    ICommand d = new LogCommand(s);
                    d.Execute(null, out bool j);
                    break;
            }
        }

        private void Write(Socket s, int value)
        {
          

        }

    }

}



