using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class DirectoryCloseEventArgs : EventArgs
    {
        /// <summary>
        /// Getter and setter for the directory path.
        /// </summary>
        public string DirectoryPath { get; set; }
        /// <summary>
        /// Getter and setter for the message for the logger.
        /// </summary>
        public string Message { get; set; }             // The Message That goes to the logger
        /// <summary>
        /// Creates the DirectoryCloseEventArgs.
        /// Param: string - path, string - message,
        /// </summary>
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                    // Setting the Directory Name
            Message = message;                          // Storing the String
        }

    }
}
