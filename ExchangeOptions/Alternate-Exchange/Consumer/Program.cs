using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "altexchange", type: ExchangeType.Fanout);
        channel.ExchangeDeclare(
            exchange: "mainexchange",
            type: ExchangeType.Direct,
            arguments: new Dictionary<string, object>
            {
                { "alternate-exchange", "altexchange" }
            });

        channel.QueueDeclare(queue: "altexchangequeue");
        channel.QueueBind(queue: "altexchangequeue", exchange: "altexchange", routingKey: "");

        var altconsumer = new EventingBasicConsumer(channel);
        altconsumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("ALT - Recieved new message: " + message);
        };
        channel.BasicConsume(queue: "altexchangequeue", autoAck: true, consumer: altconsumer);

        channel.QueueDeclare(queue: "mainexchangequeue");
        channel.QueueBind(queue: "mainexchangequeue", exchange: "mainexchange", routingKey: "test");

        var mainconsumer = new EventingBasicConsumer(channel);
        mainconsumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("MAIN - Recieved new message: " + message);
        };
        channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainconsumer);

        Console.WriteLine("Consumer Starrted");
        Console.ReadKey();

    }

}
