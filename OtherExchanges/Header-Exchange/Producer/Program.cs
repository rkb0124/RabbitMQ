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

        channel.ExchangeDeclare(exchange: "headerexchange", type: ExchangeType.Headers);

        var message = "This message will be sent with headers";
        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object> { { "name", "Brian" } };


        channel.BasicPublish("headerexchange", "", properties, body);

        Console.WriteLine($"Send message : {message}");
        Console.ReadKey();
    }
}
