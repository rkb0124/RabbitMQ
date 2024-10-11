
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

        channel.ExchangeDeclare(exchange: "pubsub", type: ExchangeType.Fanout);

        var message = "Hello I want to broadcast a message";
        var encodedMessage = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
                exchange: "pubsub",
                routingKey: "",
                basicProperties: null,
                body: encodedMessage);
        Console.WriteLine($"Send message: {message}");
    }
}
