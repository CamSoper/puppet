using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Puppet.Automation;
using Puppet.Common.Models;
using Puppet.Common.Services;
using System.Linq;
using System.Collections.Generic;

namespace Puppet.Executive
{
    class Program
    {
        static void Main(string[] args)
        {
            // Abstraction representing the home automation system
            HomeAutomationPlatform hub = new Hubitat();

            // Class to manage long-running tasks
            var taskManager = new AutomationTaskManager();
           
            // Connect to the SignalR hub
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/automationhub")
                .AddMessagePackProtocol()
                .Build();
            
            // Add event handling to the SignalR client to 
            // handle events sent from the hub
            AutomationTaskHub.SetUpConnectionEvents(connection, hub, taskManager);
            AutomationTaskHub.ConnectToWebApi(connection).Wait();

            // Loop forever, this is a daemon process
            while(true)
            {
                Task.Delay(TimeSpan.FromSeconds(60)).Wait();
                taskManager.RemoveCompletedTasks();
            }
            
        }
    }
}
