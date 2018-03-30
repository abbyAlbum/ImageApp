using ImageService.Modal;
using System;
using System.IO;


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
            this.m_dirWatcher = new FileSystemWatcher(m_path);
        }


        event EventHandler<DirectoryCloseEventArgs> IDirectoryHandler.DirectoryClose
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void IDirectoryHandler.OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            
        }

        void IDirectoryHandler.StartHandleDirectory(string dirPath)
        {
            m_dirWatcher.Path = dirPath;
            m_dirWatcher.Filter = "*.jpg";
            m_dirWatcher.Filter = "*.pmg";
            m_dirWatcher.Filter = "*.gif";
            m_dirWatcher.Filter = "*.bmp";
        }

        // Implement Here!
    }
}
