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
        private Socket s;
        int value;

        public GetConfigCommand(Socket s, int value)
        {
            this.s = s;
            this.value = value;
        }

        public string Execute(string[] args, out bool result)
        {
           
            ASCIIEncoding asen = new ASCIIEncoding();
            switch (value)
            {
                case 0:
                    s.Send(asen.GetBytes(ConfigurationManager.AppSettings["OutputDir"]));
                    break;
                case 1:
                    s.Send(asen.GetBytes(ConfigurationManager.AppSettings["SourceName"]));
                    break;
                case 2:
                    s.Send(asen.GetBytes(ConfigurationManager.AppSettings["LogName"]));
                    break;
                case 3:
                    s.Send(asen.GetBytes(ConfigurationManager.AppSettings["ThumbnailSize"]));
                    break;
                case 4:
                    s.Send(asen.GetBytes(ConfigurationManager.AppSettings["Handler"]));
                    break;


                default:
                    result = false;
                    return "too many calls";
                    
            }
            result = true;

            s.Close();
            return "sent config number" + value.ToString() + " to gui";
        }
    }
}
