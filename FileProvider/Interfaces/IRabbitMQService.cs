namespace FileProvider.Interfaces
{
    public interface IRabbitMQService
    {
        Task PublishMessageAsync(string queueName, string message);
    }
}
