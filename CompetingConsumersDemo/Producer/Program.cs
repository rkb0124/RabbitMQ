using RabbitMQ.Client;
using System.Text;

namespace Producer;

class Program
{
    static void Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "letterbox",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        while (true)
        {
            var message = $"This is my message {DateTime.Now.ToString("HH:mm:ss")}";
            var encodedMessage = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "letterbox",
                mandatory: true,
                basicProperties: null,
                body: encodedMessage);

            Console.WriteLine("Message published ...Press q to quit or else press any key to continue");
            ConsoleKeyInfo key = Console.ReadKey();

            if (key.KeyChar=='q')
            {
                break;
            }
        }



    }
}
