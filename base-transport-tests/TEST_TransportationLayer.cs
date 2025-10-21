using System.Text;
using base_transport;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Options;

namespace base_transport_tests;

public class TEST_TransportationLayer
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithName("rabbitmq-test-container")
        .WithImage("docker.io/rabbitmq:4.1.4-management-alpine")
        .WithPortBinding("5672", "5672")
        .WithPortBinding("15672", "15672")
        .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
        .WithEnvironment("RABBITMQ_DEFAULT_PASS", "admin")
        .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "/")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(15672)))
        .WithReuse(true)
        .Build();

    public TEST_TransportationLayer()
    {
         _container.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }


    [Fact]
    public void If_TransportationLayerCredentials_Is_NOT_Null()
    {
        var credentials = new TransportationLayerCredentials
        {
            HostName = "myhost",
            UserName = "myuser",
            Password = "mypassword"
        };

        var options = Microsoft.Extensions.Options.Options.Create(credentials);
        var optionsMonitor = new OptionsMonitorWrapper(options);
        var layer = new TransportationLayer(optionsMonitor);

        Assert.Equal("myhost", layer.HostName);
        Assert.Equal("myuser", layer.UserName);
        Assert.Equal("mypassword", layer.Password);
    }

    // Helper class to wrap IOptions as IOptionsMonitor for testing
    private class OptionsMonitorWrapper : IOptionsMonitor<TransportationLayerCredentials>
    {
        private readonly IOptions<TransportationLayerCredentials> _options;

        public OptionsMonitorWrapper(IOptions<TransportationLayerCredentials> options)
        {
            _options = options;
        }

        public TransportationLayerCredentials CurrentValue => _options.Value;

        public TransportationLayerCredentials Get(string? name) => _options.Value;

        public IDisposable? OnChange(Action<TransportationLayerCredentials, string?> listener) => null;
    }

    [Fact]
    public async Task If_TransportationLayer_Can_Connect()
    {

        
        // Thread.Sleep(15000);

        var credentials = new TransportationLayerCredentials
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };

        var options = Microsoft.Extensions.Options.Options.Create(credentials);
        var optionsMonitor = new OptionsMonitorWrapper(options);
        var layer = new TransportationLayer(optionsMonitor);

        await layer.ConnectAsync();
        Assert.True(layer.IsOpen);
    }

    [Fact]
    public async Task If_TransportationLayer_Can_Send_Message()
    {
        
        var credentials = new TransportationLayerCredentials
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };

        var options = Microsoft.Extensions.Options.Options.Create(credentials);
        var optionsMonitor = new OptionsMonitorWrapper(options);
        var layer = new TransportationLayer(optionsMonitor);

        await layer.ConnectAsync();
        Assert.True(layer.IsOpen);

        var message = "Hello from TransportationLayer!";
        var body = System.Text.Encoding.UTF8.GetBytes(message);

        await layer.BasicPublishAsync("test-queue", body);
    }

    [Fact]
    public async Task If_TransportationLayer_Can_Receive_Message()
    {
        
        var credentials = new TransportationLayerCredentials
        {
            HostName = "localhost",
            UserName = "admin",
            Password = "admin"
        };

        var options = Microsoft.Extensions.Options.Options.Create(credentials);
        var optionsMonitor = new OptionsMonitorWrapper(options);
        var layer = new TransportationLayer(optionsMonitor);

        await layer.ConnectAsync();
        Assert.True(layer.IsOpen);

        string publishedMessage = "Hello from TransportationLayer!";
        string receivedMessage = "";
        
        layer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            receivedMessage = Encoding.UTF8.GetString(body);
            Assert.Equal(publishedMessage, receivedMessage);

            await Task.CompletedTask;
        };

        await layer.BasicConsumeAsync("test-queue", autoAck: true);

        var body = System.Text.Encoding.UTF8.GetBytes(publishedMessage);
        await layer.BasicPublishAsync("test-queue", body);


    }
}