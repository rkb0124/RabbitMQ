using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace Consumer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "acceptrejectexchange", type: ExchangeType.Fanout);

        channel.QueueDeclare(queue: "letterbox");
        channel.QueueBind(queue: "letterbox", exchange: "acceptrejectexchange", routingKey: "test");

        var mainconsumer = new EventingBasicConsumer(channel);
        mainconsumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            if (ea.DeliveryTag % 5 == 0)
            {
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: true);
            }

            Console.WriteLine("Recieved new message: " + message);
        };
        channel.BasicConsume(queue: "letterbox", autoAck: false, consumer: mainconsumer);

        Console.WriteLine("Consumer Started");
        Console.ReadKey();
    }
}
