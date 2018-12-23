using Microsoft.AspNetCore.SignalR.Client;
using Puppet.Common.Events;
using Puppet.Common.Models.Automation;
using Puppet.Common.Services;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Puppet.Executive
{
    public class AutomationTaskHub
    {
        /// <summary>
        /// Binds the events received from the SignalR hub.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="hub"></param>
        /// <param name="taskManager"></param>
        public static void SetUpConnectionEvents(HubConnection connection, 
            HomeAutomationPlatform hub, AutomationTaskManager taskManager)
        {
            connection.Closed += async (error) =>
            {
                Console.WriteLine($"{DateTime.Now} Connection closed. Attempting to reconnect...");
                await ConnectToWebApi(connection);
            };

            // This is the bit of code that fires every time an event is received.
            // This is where the magic happens.
            connection.On<string, HubEvent>("handle", (automationName, evt) =>
            {
                Console.WriteLine($"{DateTime.Now} {automationName} event received. {evt?.description}");

                // If this automation is already running, cancel all running instances
                taskManager.CancelAllTasks(automationName);

                // Get a reference to the automation
                IAutomation automation = AutomationFactory.GetAutomation(automationName, hub);

                if (automation != null)
                {
                    // Start a task to handle the automation and a CancellationToken Source
                    // so we can cancel it later.
                    var cts = new CancellationTokenSource();
                    Action action = () =>
                    {
                        // This runs the Handle method on the automation class
                        automation.Handle(evt, cts.Token);
                    };
                    var task = new AutomationTask(action, cts.Token, automationName);
                    task.Start();

                    // Hold on to the task and its cancellation token source for later
                    taskManager.Track(new AutomationTaskTokenSourcePair(task, cts));
                }
            });
        }

        /// <summary>
        /// Starts the connection the the SignalR hub in the web project.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static async Task ConnectToWebApi(HubConnection connection)
        {
            try
            {
                await connection.StartAsync();
                Console.WriteLine($"{DateTime.Now} Connection started...");
            }
            catch (HttpRequestException)
            {
                Console.WriteLine($"{DateTime.Now} Unable to connect. Reattempting in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                await ConnectToWebApi(connection);
            }
        }
    }
}
