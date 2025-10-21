using base_transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace base_transport_tests;

public class TEST_ServiceExtension
{
    [Fact]
    public void AddTransportationLayer_ShouldRegisterTransportationLayerCredentials()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RabbitMQ:HostName", "localhost" },
                { "RabbitMQ:UserName", "user" },
                { "RabbitMQ:Password", "password" }
            })
            .Build();

        // Act
        services.AddTransportationLayer(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<MessagingCredentials>>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal("localhost", options.Value.HostName);
        Assert.Equal("user", options.Value.UserName);
        Assert.Equal("password", options.Value.Password);
    }

    [Fact]
    public void AddTransportationLayer_ShouldRegisterITransportationLayerAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddTransportationLayer(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var instance1 = serviceProvider.GetService<IBasicMessagingService>();
        var instance2 = serviceProvider.GetService<IBasicMessagingService>();

        // Assert
        Assert.NotNull(instance1);
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void AddTransportationLayer_ShouldHandleMissingConfigurationSectionGracefully()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        // Act
        services.AddTransportationLayer(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<MessagingCredentials>>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal("localhost", options.Value.HostName);
        Assert.Equal("guest", options.Value.UserName);
        Assert.Equal("guest", options.Value.Password);
    }

    [Fact]
    public void AddTransportationLayer_ShouldReadCredentialsFromEnvironmentVariables()
    {
        // Arrange
        var services = new ServiceCollection();
        Environment.SetEnvironmentVariable("RabbitMQ__HostName", "env-host");
        Environment.SetEnvironmentVariable("RabbitMQ__UserName", "env-user");
        Environment.SetEnvironmentVariable("RabbitMQ__Password", "env-password");

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        // Act
        services.AddTransportationLayer(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<MessagingCredentials>>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal("env-host", options.Value.HostName);
        Assert.Equal("env-user", options.Value.UserName);
        Assert.Equal("env-password", options.Value.Password);

        // Cleanup
        Environment.SetEnvironmentVariable("RabbitMQ__HostName", null);
        Environment.SetEnvironmentVariable("RabbitMQ__UserName", null);
        Environment.SetEnvironmentVariable("RabbitMQ__Password", null);
    }

    [Fact]
    public void AddTransportationLayer_ShouldPrioritizeInMemoryConfigurationOverEnvironmentVariables()
    {
        // Arrange
        var services = new ServiceCollection();
        Environment.SetEnvironmentVariable("RabbitMQ__HostName", "env-host");
        Environment.SetEnvironmentVariable("RabbitMQ__UserName", "env-user");
        Environment.SetEnvironmentVariable("RabbitMQ__Password", "env-password");

        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "RabbitMQ:HostName", "memory-host" },
                { "RabbitMQ:UserName", "memory-user" },
                { "RabbitMQ:Password", "memory-password" }
            })
            .Build();

        // Act
        services.AddTransportationLayer(configuration);
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<MessagingCredentials>>();

        // Assert
        Assert.NotNull(options);
        Assert.Equal("memory-host", options.Value.HostName);
        Assert.Equal("memory-user", options.Value.UserName);
        Assert.Equal("memory-password", options.Value.Password);

        // Cleanup
        Environment.SetEnvironmentVariable("RabbitMQ__HostName", null);
        Environment.SetEnvironmentVariable("RabbitMQ__UserName", null);
        Environment.SetEnvironmentVariable("RabbitMQ__Password", null);
    }
}