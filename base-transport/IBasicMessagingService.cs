namespace base_transport;

public interface IBasicMessagingService
{
    /// <summary>
    /// Connects to the RabbitMQ server asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    Task ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to the specified queue asynchronously.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    Task BasicPublishAsync(string queueName, byte[] body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumes messages from the specified queue asynchronously.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="autoAck"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    Task BasicConsumeAsync(string queueName, bool autoAck = false,
        CancellationToken cancellationToken = default);
}