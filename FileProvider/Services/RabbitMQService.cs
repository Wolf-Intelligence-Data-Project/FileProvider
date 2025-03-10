using FileProvider.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace FileProvider.Services;

public class RabbitMQService : IRabbitMQService
{
    private readonly ConnectionFactory _factory;

    public RabbitMQService(string connectionString)
    {
        _factory = new ConnectionFactory { Uri = new Uri(connectionString) };
    }

    public async Task PublishMessageAsync(string queueName, string message)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}
