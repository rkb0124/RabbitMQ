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

        channel.ExchangeDeclare(exchange: "deadletterexchange", type: ExchangeType.Fanout);
        channel.ExchangeDeclare(exchange: "mainexchange", type: ExchangeType.Direct);

        channel.QueueDeclare(
            queue: "mainexchangequeue",
            arguments: new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "deadletterexchange" },
                { "x-message-ttl", 1000 } ///1000 milli seconds
            });
        channel.QueueBind(queue: "mainexchangequeue", exchange: "mainexchange", routingKey: "test");

        //var mainconsumer = new EventingBasicConsumer(channel);
        //mainconsumer.Received += (model, ea) =>
        //{
        //    var body = ea.Body.ToArray();
        //    var message = Encoding.UTF8.GetString(body);
        //    Console.WriteLine("MAIN - Recieved new message: " + message);
        //};
        //channel.BasicConsume(queue: "mainexchangequeue", autoAck: true, consumer: mainconsumer);

        channel.QueueDeclare(queue: "deadletterexchangequeue");
        channel.QueueBind(queue: "deadletterexchangequeue", exchange: "deadletterexchange", routingKey: "");

        var dlxconsumer = new EventingBasicConsumer(channel);
        dlxconsumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("DLX - Recieved new message: " + message);
        };
        channel.BasicConsume(queue: "deadletterexchangequeue", autoAck: true, consumer: dlxconsumer);

        Console.WriteLine("Consumer Starrted");
        Console.ReadKey();
    }
}
