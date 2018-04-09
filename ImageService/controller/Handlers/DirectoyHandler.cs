using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController m_controller, ILoggingService m_logging, string m_path)
        {
            this.m_controller = m_controller;
            this.m_logging = m_logging;
            this.m_path = m_path;
            this.m_dirWatcher = new FileSystemWatcher();
            StartHandleDirectory(m_path);
        }


        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) //how to check if command is meant for its directory?
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand) CloseHandler(sender, e);
            m_controller.ExecuteCommand(e.CommandID, e.Args, out bool result); // why object sender? its "*"
        }

        void CloseHandler(object sender, CommandRecievedEventArgs args)
        {
            DirectoryCloseEventArgs eventArgs;
            try
            {
                m_dirWatcher.EnableRaisingEvents = false;
                eventArgs = new DirectoryCloseEventArgs(args.RequestDirPath, "deleted");
            } catch(Exception e)
            {
                eventArgs = new DirectoryCloseEventArgs(args.RequestDirPath, "problem");
                m_logging.Log(e.ToString(), Logging.Modal.MessageTypeEnum.FAIL);
            }
            DirectoryClose.Invoke(sender, eventArgs);
        }

        public void StartHandleDirectory(string dirPath)
        {
            m_dirWatcher.Path = dirPath;
            //m_dirWatcher.Filter = "*.*";

            m_dirWatcher.NotifyFilter = NotifyFilters.Attributes |
                                        NotifyFilters.CreationTime |
             NotifyFilters.FileName |
             NotifyFilters.LastAccess |
             NotifyFilters.LastWrite |
             NotifyFilters.Size |
             NotifyFilters.Security;
            //m_dirWatcher.Changed += new FileSystemEventHandler(OnChanged);
            m_dirWatcher.Created += new FileSystemEventHandler(OnChanged);
            m_dirWatcher.EnableRaisingEvents = true;

        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            while (IsFileLocked(new FileInfo(e.FullPath)));
            string strFileExt = Path.GetExtension(e.FullPath);

            // filter file types 
            if (strFileExt.Equals(".jpg") || strFileExt.Equals(".pmg") || strFileExt.Equals(".gif") || strFileExt.Equals(".bmp"))
            {
                string[] args = { e.FullPath };

                string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out bool result);

                if (result == true) m_logging.Log(msg, Logging.Modal.MessageTypeEnum.INFO); //when to use warning?
                else m_logging.Log(msg, Logging.Modal.MessageTypeEnum.FAIL);


            }
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }
}
