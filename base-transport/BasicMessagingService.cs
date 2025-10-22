/*
 * A simple transportation layer using RabbitMQ for message queuing.
 * Author: Ekin Bulut
 *
 * Date: 2025-10-20
 *
 * Ref Docs: https://www.rabbitmq.com/tutorials/tutorial-one-dotnet
 */

using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace base_transport;

public class BasicMessagingService(IOptionsMonitor<MessagingCredentials>? options = null) : IBasicMessagingService
{
    /// <summary>
    /// The credentials for connecting to the RabbitMQ server.
    /// </summary>
    private readonly IOptionsMonitor<MessagingCredentials> _options =
        options ?? throw new ArgumentNullException(nameof(options));

    private IConnection _connection;
    private IChannel _channel;

    public string HostName => _options.CurrentValue.HostName;
    public string UserName => _options.CurrentValue.UserName;
    public string Password => _options.CurrentValue.Password;

    public bool IsOpen => _channel?.IsOpen ?? false;


    /// <summary>
    /// The asynchronous eventing basic consumer for receiving messages.
    /// </summary>
    private AsyncEventingBasicConsumer _asyncEventingBasicConsumer;

    /// <summary>
    /// Event triggered when a message is received asynchronously.
    /// </summary>
    public event AsyncEventHandler<BasicDeliverEventArgs> ReceivedAsync;


    /// <summary>
    /// Connects to the RabbitMQ server asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = HostName,
            UserName = UserName,
            Password = Password
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Publishes a message to the specified queue asynchronously.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="body"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task BasicPublishAsync(string queueName, byte[] body, CancellationToken cancellationToken = default)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Connection is not open.");
        }

        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        await _channel.BasicPublishAsync(string.Empty, queueName, body, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Consumes messages from the specified queue asynchronously.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="autoAck"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task BasicConsumeAsync(string queueName, bool autoAck = false,
        CancellationToken cancellationToken = default)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Connection is not open.");
        }

        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false,
            arguments: null, cancellationToken: cancellationToken);

        _asyncEventingBasicConsumer = new AsyncEventingBasicConsumer(_channel);
        _asyncEventingBasicConsumer.ReceivedAsync += ReceivedAsync;

        await _channel.BasicConsumeAsync(queueName, autoAck, _asyncEventingBasicConsumer,
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Acknowledges a message with the given delivery tag asynchronously.
    /// </summary>
    /// <param name="deliveryTag"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task AcknowledgeMessageAsync(ulong deliveryTag, CancellationToken cancellationToken = default)
    {
        if (!IsOpen)
        {
            throw new InvalidOperationException("Connection is not open.");
        }

        await _channel.BasicAckAsync(deliveryTag, multiple: false, cancellationToken: cancellationToken);
    }
}