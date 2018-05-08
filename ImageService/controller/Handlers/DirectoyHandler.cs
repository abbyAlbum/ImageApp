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
        /// <summary>
        /// Creates the DirectoryHandler.
        /// Param: IImageController, ILoggingService, string.
        /// </summary>
        public DirectoyHandler(IImageController m_controller, ILoggingService m_logging, string m_path)
        {
            this.m_controller = m_controller;
            this.m_logging = m_logging;
            this.m_path = m_path;
            this.m_dirWatcher = new FileSystemWatcher();
            if (Directory.Exists(m_path))
            {
                StartHandleDirectory(m_path);
            } else m_logging.Log("No such directory! Handler Not Created for path " + m_path, Logging.Modal.MessageTypeEnum.FAIL);
        }

        /// <summary>
        ///See if command is meant for its directory
        ///Close command of execute command.
        ///Param: object, CommandRecuevedEventArgs.
        /// </summary>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e) //how to check if command is meant for its directory?
        {
            if (e.CommandID == (int)CommandEnum.CloseCommand && (e.RequestDirPath.Equals(m_path) || e.RequestDirPath.Equals(""))) CloseHandler(sender, e);
            else m_controller.ExecuteCommand(e.CommandID, e.Args, out bool result); // why object sender? its "*"
        }
        /// <summary>
        /// Stops listening for changes in the file, closes the handler.
        /// Param: Object sender, CommandRecievedArgs args.
        /// </summary>
        void CloseHandler(object sender, CommandRecievedEventArgs args)
        {
            DirectoryCloseEventArgs eventArgs;
            try
            {
                m_dirWatcher.EnableRaisingEvents = false;
                eventArgs = new DirectoryCloseEventArgs(m_path, " Handler closed");
            } catch(Exception e)
            {
                eventArgs = new DirectoryCloseEventArgs(m_path, " Problem with closeing handler");
                m_logging.Log(e.ToString(), Logging.Modal.MessageTypeEnum.FAIL);
            }
            DirectoryClose?.Invoke(this, eventArgs);
        }
        /// <summary>
        /// Listen for images to be added to the path - for a change in the file.
        /// Param: string - path.
        /// </summary>
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
        /// <summary>
        /// If image has the correct file type then add it to the directroy.
        /// Logs the result.
        /// Param: object, FileSystemEventArgs.
        /// </summary>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            while (IsFileLocked(new FileInfo(e.FullPath)));
            string strFileExt = Path.GetExtension(e.FullPath);

            // filter file types 
            if (strFileExt.Equals(".jpg") || strFileExt.Equals(".png") || strFileExt.Equals(".gif") || strFileExt.Equals(".bmp")
                || strFileExt.Equals(".JPG") || strFileExt.Equals(".PNG") || strFileExt.Equals(".GIF") || strFileExt.Equals(".BMP"))
            {
                string[] args = { e.FullPath };

                string msg = m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out bool result);

                if (result == true) m_logging.Log(msg, Logging.Modal.MessageTypeEnum.INFO); //when to use warning?
                else m_logging.Log(msg, Logging.Modal.MessageTypeEnum.FAIL);


            }
        }
        /// <summary>
        ///In case file is too big, checks whole file is transferred
        ///Param: FileInfo file
        /// </summary>
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
