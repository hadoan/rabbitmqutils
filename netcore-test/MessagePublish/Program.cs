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
           

            var i=0;
            while(true)
            {
                i++;
                Console.WriteLine("Sent hello world! "+i);
                 sendMachineSvc.SendMsgToMachines(new KeyValueMessage { Value = "Hello from cloud "+i }, CloudToMachineType.AllMachines);
                System.Threading.Thread.Sleep(1000);
            }
            
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
