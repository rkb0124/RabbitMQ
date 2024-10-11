using RabbitMQ.Client;
using System.Text;

namespace Producer;

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

        var message = "This is my first message";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "mainexchange", routingKey: "test11", basicProperties: null, body: body);

        Console.WriteLine("Send Message: " + message);
        Console.ReadKey();

    }
}
