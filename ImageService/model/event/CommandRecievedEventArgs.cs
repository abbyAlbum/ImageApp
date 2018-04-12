using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class CommandRecievedEventArgs : EventArgs
    {
        /// <summary>
		///Gets and sets the commandID
		/// </summary>
        public int CommandID { get; set; }      // The Command ID
        /// <summary>
		/// Gets and sets the string args
		/// </summary>
        public string[] Args { get; set; }
        /// <summary>
		/// Gets and sets the request directory
		/// </summary>
        public string RequestDirPath { get; set; }  // The Request Directory
        /// <summary>
        /// Creates the CommandRecievedEventArgs.
        /// Param: int - the command id, string - the args, string - the path.
        /// </summary>
        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
