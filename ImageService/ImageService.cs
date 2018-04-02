﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService;
using ImageService.Server;
using ImageService.Modal;
using ImageService.Controller;
using ImageService.ImageService_Logging.Model;
using ImageService.Model;
using System.Configuration;

namespace ImageProject
{

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

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        public partial class ImageService : ServiceBase
        {

            private ImageServer m_imageServer;          // The Image Server
            private IImageServiceModel model;
            private IImageController controller;
            private ILoggingService logging;

            public ImageService(string[] args)
            {
                InitializeComponent();
                string eventSourceName = ConfigurationManager.AppSettings["SourceName"];
                string logName = ConfigurationManager.AppSettings["LogName"];

                eventLog1 = new System.Diagnostics.EventLog();
                if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
                {
                    System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
                }
                eventLog1.Source = eventSourceName;
                eventLog1.Log = logName;
            }

            // Here You will Use the App Config!
            protected override void OnStart(string[] args)
            {
                // Update the service state to Start Pending.  
                ServiceStatus serviceStatus = new ServiceStatus();
                logging = new LoggingModal();
                model = new ImageServiceModel(ConfigurationManager.AppSettings["OutputDir"], Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]));
                controller = new ImageController(model);
                m_imageServer = new ImageServer();
                m_imageServer.CreateHandler(ConfigurationManager.AppSettings["Handler"]);
                logging.MessageRecieved += OnMsg;
                serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
                serviceStatus.dwWaitHint = 100000;

                eventLog1.WriteEntry("In OnStart");

                // Update the service state to Running.  
                serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            }

            protected override void OnStop()
            {
                eventLog1.WriteEntry(ServiceState.SERVICE_STOPPED.ToString());
            }

            public void OnMsg(object sender, MessageRecievedEventArgs msg)
            {
                eventLog1.WriteEntry(msg.Message);
            }


        }
    }


/* private LoggingModal Logger;

         public ImageService(string[] args)
         {
             InitializeComponent();
             string eventSourceName = "MySource";
             string logName = "MyNewLog";
             if (args.Count() > 0)
             {
                 eventSourceName = args[0];
             }
             if (args.Count() > 1)
             {
                 logName = args[1];
             }
             eventLog1 = new System.Diagnostics.EventLog();
             if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
             {
                 System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
             }
             eventLog1.Source = eventSourceName;
             eventLog1.Log = logName;
         }

         protected override void OnStart(string[] args)
         {
             // Update the service state to Start Pending.  
             ServiceStatus serviceStatus = new ServiceStatus();
             Logger = new LoggingModal();
             Logger.MessageRecieved += OnMsg;
             serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
             serviceStatus.dwWaitHint = 100000;
             SetServiceStatus(this.ServiceHandle, ref serviceStatus);

             eventLog1.WriteEntry("In OnStart");
             System.Timers.Timer timer = new System.Timers.Timer();
             timer.Interval = 60000;
             timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
             timer.Start();

             // Update the service state to Running.  
             serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
             SetServiceStatus(this.ServiceHandle, ref serviceStatus);


         }
         public void OnMsg(object sender, string msg)
         {
             eventLog1.WriteEntry(msg);
         }

         public void OnTimer(Object sender, System.Timers.ElapsedEventArgs args)
         {
             eventLog1.WriteEntry("Monitoring the system", EventLogEntryType.Information, eventId++);
         }

         protected override void OnStop()
         {
             eventLog1.WriteEntry(ServiceState.SERVICE_STOPPED.ToString());
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

         [StructLayout(LayoutKind.Sequential)]
         public struct ServiceStatus
         {
             public int dwServiceType;
             public ServiceState dwCurrentState;
             public int dwControlsAccepted;
             public int dwWin32ExitCode;
             public int dwServiceSpecificExitCode;
             public int dwCheckPoint;
             public int dwWaitHint;
         };

         [DllImport("advapi32.dll", SetLastError = true)]
         private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
     }*/
