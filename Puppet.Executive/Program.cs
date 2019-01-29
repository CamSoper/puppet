using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Puppet.Common.Services;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Puppet.Common.Automation;

namespace Puppet.Executive
{
    class Program
    {
        static AutomationTaskManager _taskManager;
        static HomeAutomationPlatform _hub;
        static void Main(string[] args)
        {
            // Abstraction representing the home automation system
            _hub = new Hubitat();
            
            // Class to manage long-running tasks
            _taskManager = new AutomationTaskManager();
            _hub.AutomationEvent += Hub_AutomationEvent; 

            // Loop forever, this is a daemon process
            while (true)
            {
                Task.Delay(TimeSpan.FromSeconds(60)).Wait();
                _taskManager.RemoveCompletedTasks();
            }
            
        }

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
                    Action action = () =>
                    {
                        // This runs the Handle method on the automation class
                        Console.WriteLine($"{DateTime.Now} {automation} event: {evt.descriptionText}");
                        automation.Handle(cts.Token);
                    };
                    var task = new AutomationTask(action, cts.Token, automation.GetType());
                    task.Start();

                    // Hold on to the task and its cancellation token source for later
                    _taskManager.Track(new AutomationTaskTokenSourcePair(task, cts));
                }
            }
        }
    }
}
