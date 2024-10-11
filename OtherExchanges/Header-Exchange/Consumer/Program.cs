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

        channel.ExchangeDeclare(exchange: "headerexchange", type: ExchangeType.Headers);
        channel.QueueDeclare(queue: "letterbox");

        var bindingArguements = new Dictionary<string, object>
        {
            {"x-match","any" },
            { "name","Brian"},
            {"age","21" }
        };
        channel.QueueBind(queue: "letterbox", exchange: "headerexchange", routingKey: "", arguments: bindingArguements);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var mesage = Encoding.UTF8.GetString(body);
            Console.WriteLine("Recieved new message: " + mesage);
            channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
        };
        channel.BasicConsume(queue: "letterbox", autoAck: true, consumer: consumer);

        Console.WriteLine("Consuming");
        Console.ReadKey();
    }


}
