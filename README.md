# rabbitmqutils

# Install RabbitMq
- Install for windows
    [RabbitMq for Windows](https://www.rabbitmq.com/install-windows.html)  
- Enable management plugin
    ```sh
    rabbitmq-plugins enable rabbitmq_management
    ```
- Create users
    ```sh
    rabbitmqctl add_user testuser testpassword
    rabbitmqctl set_user_tags testuser administrator
    rabbitmqctl set_permissions -p / testuser ".*" ".*" ".*"
    ```
    Test user
    ```sh
    curl -i -u testuser:testpassword http://localhost:15672/api/whoami
    ```
    or go to http://127.0.0.1:15672/



# Publish a message from server to all clients
Reference: MessagePublish project
```sh
   var sendMachineSvc = serviceProvider.GetService<ISendMessageToMachineClientService>();
   sendMachineSvc.InitConfigAndConnect("vulture-01.rmq.cloudamqp.com", "mlvqiexf", "AXyiTOjiv3Ssd-W_1P5iO5o1ncsVHWFQ");
   sendMachineSvc.SendMsgToMachines(new KeyValueMessage { Value = "Hello from cloud" }, CloudToMachineType.AllMachines);

   Console.WriteLine("Sent hello world!");
   sendMachineSvc.Close();
```

# Subscribe message in client
2. Java subscribe
```sh
 public void consumeNoQueuedMessage() throws IOException {
        channel = conn.createChannel();

        channel.exchangeDeclare("VooyCloud2MachineExchangeNoQueue", "fanout");
        final String queueName = channel.queueDeclare().getQueue();
        channel.queueBind(queueName, "VooyCloud2MachineExchangeNoQueue", "");
        final boolean autoAck = false;

        channel.basicConsume(queueName, autoAck, "my-client-name", new DefaultConsumer(channel) {
            @Override
            public void handleDelivery(String consumerTag, Envelope envelope, AMQP.BasicProperties properties,
                    byte[] body) throws IOException {
                String routingKey = envelope.getRoutingKey();
                String contentType = properties.getContentType();
                long deliveryTag = envelope.getDeliveryTag();
                // (process the message components here ...)
                String bodyString = new String(body);
                // Log.d("RABBITMQ","received message: "+bodyString);
                System.out.println("received message: " + bodyString);
                channel.basicAck(deliveryTag, false);
            }
        });
    }
```
1. C# - Reference [MessageSubscribe project](https://github.com/hadoan/rabbitmqutils/blob/master/netcore-test/MessageSubscribe/Program.cs)
```sh
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
                var message = MessagePackSerializer.Deserialize<KeyValueMessage>(body);
                var json = MessagePackSerializer.ToJson(body);
                Console.WriteLine(json);
            };
            _clientToCloudNoQueueChannel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
        }
```
