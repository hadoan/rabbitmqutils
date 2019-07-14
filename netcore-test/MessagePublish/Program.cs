using System;
using MessagePublishingUtils;
using Microsoft.Extensions.DependencyInjection;

namespace MessagePublish
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            SetupContainer();

            var sendMachineSvc = serviceProvider.GetService<ISendMessageToMachineClientService>();
            sendMachineSvc.InitConfigAndConnect("127.0.0.1", "testuser", "testpassword");
            sendMachineSvc.SendMsgToMachines(new KeyValueMessage { Value = "Hello from cloud" }, CloudToMachineType.AllMachines);

            Console.WriteLine("Sent hello world!");
            sendMachineSvc.Close();
        }

        private static void SetupContainer()
        {
            serviceProvider = new ServiceCollection()
                                   .AddSingleton<ISendMessageToMachineClientService, RabbitMqSendMessageToMachineService>()
                                   .AddSingleton<IConnectToRabbitMqMessageService, ConnectToRabbitMqService>()
                                   .BuildServiceProvider();
        }
    }
}
