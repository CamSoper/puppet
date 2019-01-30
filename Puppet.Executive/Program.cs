using System;
using System.Threading;
using System.Threading.Tasks;
using Puppet.Common.Services;
using Puppet.Common.Automation;
using Microsoft.Extensions.Configuration;
using System.IO;

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
            _hub = new Hubitat(configuration);
            
            // Class to manage long-running tasks
            _taskManager = new AutomationTaskManager();

            // Bind a method to handle the events raised
            // by the Hubitat device
            _hub.AutomationEvent += Hub_AutomationEvent; 

            // Loop forever, this is a daemon process
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(60));
                _taskManager.RemoveCompletedTasks();
            }
            
        }



        /// <summary>
        /// Event handler for AutomationEvents raised by the HomeAutomationPlatform.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Hub_AutomationEvent(object sender, Common.Events.AutomationEventEventArgs e)
        {
            var evt = e.HubEvent;

            // Get a reference to the automation
            var automations = AutomationFactory.GetAutomations(evt, _hub);

            foreach (IAutomation automation in automations)
            {
                if (automation != null)
                {
                    // If this automation is already running, cancel all running instances
                    _taskManager.CancelAllTasks(automation.GetType());

                    // Start a task to handle the automation and a CancellationToken Source
                    // so we can cancel it later.
                    var cts = new CancellationTokenSource();
                    var task = new AutomationTask(() =>
                    {
                        // This runs the Handle method on the automation class
                        Console.WriteLine($"{DateTime.Now} {automation} event: {evt.descriptionText}");
                        automation.Handle(cts.Token);
                    }, cts.Token, automation.GetType());

                    // Ready... go handle it!
                    task.Start();

                    // Hold on to the task and its cancellation token source for later.
                    _taskManager.Track(new AutomationTaskTokenSourcePair(task, cts));
                }
            }
        }
    }
}
