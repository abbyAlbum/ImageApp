using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;

namespace ImageService.ImageService.commands
{
    class LogCommand : ICommand
    {
        private Socket s;

        public LogCommand(Socket s)
        {
            this.s = s;
        }

        public string Execute(string[] args, out bool result)
        {
            ASCIIEncoding asen = new ASCIIEncoding();
            s.Send(asen.GetBytes("2; will this work?"));
            result = true;
            return "";
        }
    }
}
