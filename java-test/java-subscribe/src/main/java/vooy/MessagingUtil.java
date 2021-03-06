package vooy;

import com.rabbitmq.client.AMQP;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.Channel;
import com.rabbitmq.client.ConnectionFactory;
import com.rabbitmq.client.DefaultConsumer;
import com.rabbitmq.client.Envelope;
import com.rabbitmq.client.GetResponse;

import org.msgpack.core.MessageBufferPacker;
import org.msgpack.core.MessagePack;
import org.msgpack.core.MessageUnpacker;

import java.io.IOException;
import java.util.concurrent.TimeoutException;

public class MessagingUtil {

    private String userName = "testuser";
    private String password = "testpassword";
    private String hostName = "127.0.0.1";
    private ConnectionFactory factory;
    private Connection conn = null;
    private Channel channel = null;

    public void init() throws IOException, TimeoutException {
        factory = new ConnectionFactory();
        // "guest"/"guest" by default, limited to localhost connections
        factory.setUsername(userName);
        factory.setPassword(password);
        factory.setVirtualHost("/");
        factory.setHost(hostName);
        factory.setPort(5672);

        conn = factory.newConnection();
    }

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
                // String bodyString = new String(body);
                // Log.d("RABBITMQ","received message: "+bodyString);
                // MessageBufferPacker packer = MessagePack.newDefaultBufferPacker();
                MessageUnpacker unpacker = MessagePack.newDefaultUnpacker(body);
                String bodyString = unpacker.unpackString();

                System.out.println("received message: " + bodyString);

                channel.basicAck(deliveryTag, false);
            }
        });
    }

    public void close() throws IOException, TimeoutException {
        if (channel != null)
            channel.close();
        if (conn != null)
            conn.close();
    }
}
