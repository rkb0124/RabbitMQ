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

        channel.ExchangeDeclare(exchange: "firstexchange", type: ExchangeType.Direct);
        channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

        channel.ExchangeBind(destination: "secondexchange", source: "firstexchange", routingKey: "");

        var message = "This message has gone through multiple exchanges";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish("firstexchange", "", null, body);

        Console.WriteLine($"Send message : {message}");
        Console.ReadKey();
    }
}
