using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "secondexchange", type: ExchangeType.Fanout);

        channel.QueueDeclare(queue: "letterbox");
        channel.QueueBind(queue: "letterbox", exchange: "secondexchange", routingKey: "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mesage = Encoding.UTF8.GetString(body);
            Console.WriteLine("Recieved new message: " + mesage);
        };
        channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);

        Console.WriteLine("Consuming");
        Console.ReadKey();
    }


}
