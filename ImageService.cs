using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ImageProject
{
	public partial class ImageService : ServiceBase
	{
		
		public ImageService()
		{
			InitializeComponent();
			eventLog1 = new System.Diagnostics.EventLog();
			if (!System.Diagnostics.EventLog.SourceExists("MySource"))
			{
				System.Diagnostics.EventLog.CreateEventSource(
					"MySource", "MyNewLog");
			}
			eventLog1.Source = "MySource";
			eventLog1.Log = "MyNewLog";
		}

		protected override void OnStart(string[] args)
		{
            eventLog1.WriteEntry("In OnStart");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
                 
           
		}

        public void OnTimer(Object sender, System.Timers.ElapsedEventArgs args)
        {
            eventLog1.WriteEntry("Monitoring the system", EventLogEntryType.Information, eventId++);
        }

		protected override void OnStop()
		{
		}

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,

        }
	}
}
