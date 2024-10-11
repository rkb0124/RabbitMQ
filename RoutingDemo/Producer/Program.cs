using RabbitMQ.Client;
using System.Text;

namespace Producer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "mytopicexchange", type: ExchangeType.Topic);

        var message = $"This message needs to be routed";
        var encodedMessage = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "mytopicexchange",
            routingKey: "user.*",
            basicProperties: null,
            body: encodedMessage);

        Console.WriteLine("Message published...");
        Console.ReadKey();

    }
}
