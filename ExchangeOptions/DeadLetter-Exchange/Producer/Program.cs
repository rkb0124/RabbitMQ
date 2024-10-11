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

        channel.ExchangeDeclare(exchange: "mainexchange", type: ExchangeType.Direct);

        var message = "This message might expire";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "mainexchange", routingKey: "test", basicProperties: null, body: body);

        Console.WriteLine("Send Message: " + message);
        Console.ReadKey();
    }
}
