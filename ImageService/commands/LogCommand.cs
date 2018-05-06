using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
        
            EventLogQuery query = new EventLogQuery("ImageServiceLog", PathType.LogName, "*");
            EventLogReader reader = new EventLogReader(query);
           
            EventRecord eventRecord;
            
            
            string output = "";
            int i = 0;
            while ((eventRecord = reader.ReadEvent()) != null && i < 100)
            {
                output += eventRecord.Id + ",";
                output += eventRecord.FormatDescription() + "*";
                i += 1;
            }
           /* try
            {
                output = JsonConvert.SerializeObject(output);
            } catch(Exception e)
            {
                output = e.ToString();
            }*/
            


            /*for(int i = 0; i < eventLogs.Length; i++)
            {
                if(eventLogs[i].LogDisplayName == "ImageServiceLog")
                {
                    EventLogEntryCollection k = eventLogs[i].Entries;
                    
                    for (int j = 0; j < 50; j++)
                    {
                        output += k[i].EntryType + ",";
                        output += k[i].Message + "*";
                    }
                    
                }*/
            //output += eventLogs[i].LogDisplayName + ",";
            //output += eventLogs[i].Log + "*";

            result = true;
            return output;

        }

        }
    }

