using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Server;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("request-queue", exclusive: false);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (sender, args) =>
        {
            Console.WriteLine($"Request recieved: {args.BasicProperties.CorrelationId}");

            var replyMessaage = $"This is your reply. CorrelationId: {args.BasicProperties.CorrelationId}";
            var body = Encoding.UTF8.GetBytes(replyMessaage);
            channel.BasicPublish("", args.BasicProperties.ReplyTo, null, body);

        };
        channel.BasicConsume(queue: "request-queue", autoAck: true, consumer: consumer);

        Console.ReadKey();
    }
}
