using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace base_transport;

public class TransportationLayer(TransportationLayerCredentials? credentials = null)
{
    private readonly TransportationLayerCredentials _credentials = credentials ?? new TransportationLayerCredentials();

    private IConnection _connection;
    private IChannel _channel;

    public string HostName => _credentials.HostName;
    public string UserName => _credentials.UserName;
    public string Password => _credentials.Password;

    public bool IsOpen => _channel?.IsOpen ?? false;


    private AsyncEventingBasicConsumer _asyncEventingBasicConsumer;
    public AsyncEventHandler<BasicDeliverEventArgs> ReceivedAsync;

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
}