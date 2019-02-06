using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Puppet.Common.Automation;
using Puppet.Common.Events;
using Puppet.Common.Services;

namespace Puppet.Executive
{
    class Program
    {
        const string _appSettingsFileName = "appsettings.json";

        static AutomationTaskManager _taskManager;
        static HomeAutomationPlatform _hub;

        public static async Task Main(string[] args)
        {
            // Read in the configuration file
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                .AddJsonFile(_appSettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            // Abstraction representing the home automation system
            _hub = new Hubitat(configuration, HttpClientFactory.Create());

            // Class to manage long-running tasks
            _taskManager = new AutomationTaskManager();

            // Bind a method to handle the events raised
            // by the Hubitat device
            _hub.AutomationEvent += Hub_AutomationEvent;
            var hubTask = _hub.StartAutomationEventWatcher();

            // Wait forever, this is a daemon process
            await hubTask;
        }



        /// <summary>
        /// Event handler for AutomationEvents raised by the HomeAutomationPlatform.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Hub_AutomationEvent(object sender, Common.Events.AutomationEventEventArgs e)
        {
            var evt = e.HubEvent;

            Task.Run(() => StartRelevantAutomationHandlers(evt));
            //Task.Run(() => SendEventToMqtt(evt));
            //Task.Run(() => SendEventToAlexa(evt));
        }

        private static void SendEventToAlexa(HubEvent evt)
        {
            // TODO: Forward events to Alexa
        }

        private static void SendEventToMqtt(HubEvent evt)
        {
            // TODO: Forward events to MQTT
        }

        private static void StartRelevantAutomationHandlers(HubEvent evt)
        {
            // Get a reference to the automation
            var automations = AutomationFactory.GetAutomations(evt, _hub);

            foreach (IAutomation automation in automations)
            {
                if (automation != null)
                {
                    // If this automation is already running, cancel all running instances
                    _taskManager.CancelExistingTasks(automation.GetType());

                    // Start a task to handle the automation and a CancellationToken Source
                    // so we can cancel it later.
                    var cts = new CancellationTokenSource();
                    async Task handleTask()
                    {
                        // This runs the Handle method on the automation class
                        Console.WriteLine($"{DateTime.Now} {automation} event: {evt.descriptionText}");
                        await automation.Handle(cts.Token);
                    }

                    var task = new AutomationTask(handleTask, automation.GetType());

                    // Ready... go handle it!
                    Task work = task.Start();

                    // Hold on to the task and its cancellation token source for later.
                    _taskManager.Track(work, cts, automation.GetType());
                }
            }

            // Let's take this opportunity to get rid of any completed tasks.
            _taskManager.RemoveCompletedTasks();
        }
    }
}
