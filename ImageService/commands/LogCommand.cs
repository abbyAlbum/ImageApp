using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;

namespace ImageService.ImageService.commands
{
    class LogCommand : ICommand
    {
        

        public LogCommand()
        {
            
        }

        public string Execute(string[] args, out bool result)
        {
            EventLog[] eventLogs = EventLog.GetEventLogs();
            
            string output = "";
            for(int i = 0; i < eventLogs.Length; i++)
            {
                if(eventLogs[i].LogDisplayName == "ImageServiceLog")
                {
                    EventLogEntryCollection k = eventLogs[i].Entries;
                    
                    for (int j = 0; j < 50; j++)
                    {
                        output += k[i].EntryType + ",";
                        output += k[i].TimeGenerated + "*";
                    }
                    
                }
                //output += eventLogs[i].LogDisplayName + ",";
                //output += eventLogs[i].Log + "*";
                
                

            }

            result = true;
            return output;
        }
    }
}
