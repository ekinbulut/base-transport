using System.Text;
using base_transport;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;
namespace base_transport_tests;


public class TestMessage : IMessage
{
    public string CorrelationId { get; set; }
}

public class TestMessageHandler(IBasicMessagingService service) : BaseMessageHandler<TestMessage>(service)
{
    public override Task HandleAsync(TestMessage message, ulong deliveryTag, CancellationToken cancellationToken = default)
    {
        return base.HandleAsync(message, deliveryTag, cancellationToken);
    }
}


public class DemoMessage : IMessage
{
    public string CorrelationId { get; set; }
}

public class DemoMessageHandler(IBasicMessagingService service) : BaseMessageHandler<DemoMessage>(service)
{
    public override Task HandleAsync(DemoMessage message, ulong deliveryTag, CancellationToken cancellationToken = default)
    {
        return base.HandleAsync(message, deliveryTag, cancellationToken);
    }
}