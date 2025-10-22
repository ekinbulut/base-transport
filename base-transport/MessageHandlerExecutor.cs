using Microsoft.Extensions.DependencyInjection;

namespace base_transport;

public static class MessageHandlerExecutor
{
    public static Task StartHandlerAsync<TMessage>(IServiceProvider serviceProvider, string queue, CancellationToken ct = default) 
        where TMessage : IMessage
    {
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && IsBaseMessageHandler(t));
        
        foreach (var handlerType in handlerTypes)
        {
            var handler = ActivatorUtilities.CreateInstance(serviceProvider, handlerType);
            var method = handlerType.GetMethod("StartListeningAsync");
            if (method == null) continue;
            var task = method.Invoke(handler, [queue, CancellationToken.None]) as Task;
            task?.GetAwaiter().GetResult();
        }

        return Task.CompletedTask;
    }

    private static bool IsBaseMessageHandler(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(BaseMessageHandler<>))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}