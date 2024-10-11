using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer;


class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "letterbox",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(channel);


        Console.WriteLine("Enter time required to process a message for this consumer in seconds.");
        int waittime = Convert.ToInt16(Console.ReadLine());

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Message Recieved : " + message);
            Thread.Sleep(TimeSpan.FromSeconds(waittime));
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

        };

        channel.BasicConsume(queue: "letterbox", autoAck: false, consumer);

        Console.ReadKey();

    }
}
