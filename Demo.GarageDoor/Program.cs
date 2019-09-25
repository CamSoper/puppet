using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.GarageDoor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Config
            // Read the configuration file
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Directory where the json files are located
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string svcBusConn = configuration["ConnectionString"];

            // Setup
            Console.WriteLine("Opening Service Bus Connection...");
            ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(svcBusConn);
            QueueClient client = new QueueClient(builder, ReceiveMode.ReceiveAndDelete);

            RegisterHandlers(client);

            // Wait
            Console.WriteLine("Ready. Awaiting message from Puppet.");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Clean up
            await client.CloseAsync();
        }

        static void RegisterHandlers(QueueClient client)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1
            };

            client.RegisterMessageHandler(MessageReceivedHandler, messageHandlerOptions);
        }

        static Task MessageReceivedHandler(Message message, CancellationToken token)
        {
            if (message.Label == "open sesame")
            {
                Console.WriteLine($"Cycling door at {DateTime.Now}");
                using (GpioController gpio =
                    new GpioController(PinNumberingScheme.Logical, new RaspberryPi3Driver()))
                {
                    gpio.OpenPin(19, PinMode.Output);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                    gpio.ClosePin(19);
                }
            }
            return Task.CompletedTask;
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            if (exceptionReceivedEventArgs.Exception.GetType() != typeof(OperationCanceledException))
            {
                Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            }
            return Task.CompletedTask;
        }
    }
}
