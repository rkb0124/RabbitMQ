using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace AnalyticsConsumer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "mytopicexchange", ExchangeType.Topic);
        string queuename = channel.QueueDeclare().QueueName;

        channel.QueueBind(queue: queuename, exchange: "mytopicexchange", routingKey: "user.*");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Analytics - Recieved new message: " + message);
        };
        channel.BasicConsume(queue: queuename, autoAck: true, consumer);

        Console.ReadKey();

    }
}
