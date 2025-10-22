using base_transport;
using Microsoft.Extensions.Options;

namespace base_transport_tests;

public class OptionsMonitorWrapper : IOptionsMonitor<MessagingCredentials>
{
    private readonly IOptions<MessagingCredentials> _options;

    public OptionsMonitorWrapper(IOptions<MessagingCredentials> options)
    {
        _options = options;
    }

    public MessagingCredentials CurrentValue => _options.Value;

    public MessagingCredentials Get(string? name) => _options.Value;

    public IDisposable? OnChange(Action<MessagingCredentials, string?> listener) => null;
}