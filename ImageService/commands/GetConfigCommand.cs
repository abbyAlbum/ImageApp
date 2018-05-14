using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;

namespace ImageService.ImageService.commands
{
    class GetConfigCommand : ICommand
    {

        /// <summary>
        ///Constrctor for the appconfig command 
        /// </summary>
        public GetConfigCommand()
        {

        }

        /// <summary>
        ///Executes the log command
        /// </summary>
        /// <param string[]="args"></param>
        /// <param out bool="result"></param>
        /// <return the log ></return>
        public string Execute(string[] args, out bool result)
        {

            string app = ConfigurationManager.AppSettings["OutputDir"] + "*" + ConfigurationManager.AppSettings["SourceName"] + "*" +
                ConfigurationManager.AppSettings["LogName"] + "*" + ConfigurationManager.AppSettings["ThumbnailSize"] + "*" + 
                ConfigurationManager.AppSettings["Handler"];


            result = true;
            return app;
            //s.Close();
            //return "sent config number" + value.ToString() + " to gui";
        }
    }
}
