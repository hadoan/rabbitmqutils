using MessagePack;
using MessagePublishingUtils;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Threading.Tasks;

namespace MessageSubscribe
{
    class Program
    {
        private static IServiceProvider serviceProvider;
        private static IConnectToRabbitMqMessageService _connectToRabbitMqService;
        static void Main(string[] args)
        {
            SetupContainer();
            Console.WriteLine("Hello World!");
            _connectToRabbitMqService = serviceProvider.GetService<IConnectToRabbitMqMessageService>();
            _connectToRabbitMqService.Connect("127.0.0.1", "testuser", "testpassword");
            ConsumeClientNoQueuedMessages();
            while(true)
            {
                //Console.WriteLine("checking...");
                Task.Delay(1000);
            }
        }

        private static void SetupContainer()
        {
            serviceProvider = new ServiceCollection()
                .AddSingleton<IConnectToRabbitMqMessageService, ConnectToRabbitMqService>()
                .BuildServiceProvider();
        }

        private static void ConsumeClientNoQueuedMessages()
        {
            var _clientToCloudNoQueueChannel = _connectToRabbitMqService.GetNoQueuedModel();

            _clientToCloudNoQueueChannel.ExchangeDeclare(RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_NOQUEUE, "fanout");
            var queueName = _clientToCloudNoQueueChannel.QueueDeclare().QueueName;

            _clientToCloudNoQueueChannel.QueueBind(queueName, RabbitMqConstants.EXCHANGE_CLOUD_TO_MACHINE_NOQUEUE, "");

            var consumer = new EventingBasicConsumer(_clientToCloudNoQueueChannel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = MessagePackSerializer.Deserialize<string>(body);
                //var json = MessagePackSerializer.ToJson(body);
                Console.WriteLine(message);
            };
            _clientToCloudNoQueueChannel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }
    }
}
