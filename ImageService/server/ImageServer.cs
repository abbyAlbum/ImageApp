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
        private IList<Socket> clients = new List<Socket>();
        private readonly object syncLock = new object();
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
            m_logging.Log(args.DirectoryPath + args.Message, Logging.Modal.MessageTypeEnum.INFO);
        }
        /// <summary>
        /// Sends the command to the controller, that then executes the command.
        /// </summary>
        public void SendCommandToController()
        {
            string[] args = null;
            CommandRecieved?.Invoke("*", new CommandRecievedEventArgs(3, args, ""));
            myList.Stop();
        }

        public void StartServer()
        {
            try
            {
                /* Start Listeneting at the specified port */
                myList.Start();
                m_logging.Log("The server is running at port 8001", Logging.Modal.MessageTypeEnum.INFO);
                m_logging.Log("The local End point is  :" +
                                  myList.LocalEndpoint, Logging.Modal.MessageTypeEnum.INFO);
                m_logging.Log("Waiting for a connection.....", Logging.Modal.MessageTypeEnum.INFO);

                while (true)
                {

                    Socket s = myList.AcceptSocket();
                    clients.Add(s);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    Thread t = new Thread(() => ReadCommand(s));
                    t.Start();


                }


            }
            catch (Exception e)
            {
                m_logging.Log("Error..... " + e.StackTrace, Logging.Modal.MessageTypeEnum.FAIL);
            }
        }
        private void ReadCommand(Socket s)
        {
            while (IsConnected(s))
            {
                try
                {

                    byte[] b = new byte[1000];
                    string send = "empty";

                    int k = s.Receive(b);

                    //m_logging.Log("Recieved...", Logging.Modal.MessageTypeEnum.INFO);
                    int i = (int)Char.GetNumericValue(Convert.ToChar(b[0]));
                    switch (i)
                    {
                        case 1:
                            send = m_controller.ExecuteCommand(i, null, out bool result);
                            break;
                        case 2:
                            send = m_controller.ExecuteCommand(i, null, out bool res);
                            break;
                        case 3: clients.Remove(s);
                                return;
                            
                        case 5:
                            string handel = ByteToString(b, k);
                            IList<string> each = handel.Split(',').Reverse().ToList();
                            CommandRecieved?.Invoke("*", new CommandRecievedEventArgs(3, null, each[0]));
                            send = "2" + each[0];
                            SendCloseHandler(each[0], s);
                            break;
                    }
                    // m_logging.Log("sending " + send, Logging.Modal.MessageTypeEnum.INFO);
                    if (IsConnected(s)) Write(s, send);

                }
                catch (Exception e)
                {
                    break;
                }

            }
        }

        private string ByteToString(byte[] b, int k)
        {
            string output = "";
            
            for (int i = 0; i < k; i++)
                output += Convert.ToChar(b[i]).ToString();
            return output;
        }

        public bool IsConnected(Socket s)
        {
            try
            {
                return !(s.Poll(1, SelectMode.SelectRead) && s.Available == 0);
            }
            catch (SocketException e)
            {
                return false;
            }
        }

        private void Write(Socket s, string send)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            lock (syncLock)
            {
                s.Send(asen.GetBytes(send));
            }
        }

        public void SendCloseHandler(string path, Socket s)
        {
            for (int i = 0; i < clients.Count; i++)
            {

                if (IsConnected(clients.ElementAt(i)) && clients.ElementAt(i) != s)
                {
                    try
                    {
                        Write(clients.ElementAt(i), "2" + path);
                    } catch(Exception e)
                    {
                        clients.Remove(clients.ElementAt(i));
                        i = i - 1;
                    }
                }

            }
        }
    



        public void SendLog(string message)
        {
            for (int i = 0; i < clients.Count; i++)
            {

                if (IsConnected(clients.ElementAt(i)))
                { 
                    try
                    {
                        Write(clients.ElementAt(i), "1" + message);
                    }
                    catch (Exception e)
                    {
                        clients.Remove(clients.ElementAt(i));
                        i = i - 1;
                    }
                }

            }
        }
    }

}




