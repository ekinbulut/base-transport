namespace base_transport;

public abstract class BaseMessageHandler<T>(IBasicMessagingService service) : IBasicMessageHandler<T>
    where T : IMessage
{
    public async Task StartListeningAsync(string queue, CancellationToken ct = default)
    {
        await service.ConnectAsync(ct);

        service.ReceivedAsync += async (sender, args) =>
        {
            var deliveryTag = args.DeliveryTag;
            var body = args.Body.ToArray();
            var messageString = System.Text.Encoding.UTF8.GetString(body);
            var @event = System.Text.Json.JsonSerializer.Deserialize<T>(messageString);
            
            if (@event != null)
            {
                await HandleAsync(@event, deliveryTag, ct).ConfigureAwait(false);
            }
        };

        await service.BasicConsumeAsync(queue, autoAck: false, ct);
    }


    public virtual async Task HandleAsync(T message, ulong deliveryTag, CancellationToken cancellationToken = default)
    {
        
        await service.AcknowledgeMessageAsync(deliveryTag, cancellationToken);
    }
    
}

public interface IBasicMessageHandler<in T> where T : IMessage
{
    Task StartListeningAsync(string queue, CancellationToken ct = default);
    
    Task HandleAsync(T message, ulong deliveryTag, CancellationToken cancellationToken = default);
}


public interface IMessage
{
    string CorrelationId { get; set; }
}