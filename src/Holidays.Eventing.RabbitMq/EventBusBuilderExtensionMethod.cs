namespace Holidays.Eventing.RabbitMq;

public static class EventBusBuilderExtensionMethod
{
    public static EventBusBuilder UseRabbitMq(
        this EventBusBuilder builder, 
        RabbitMqSettings settings, 
        Action<RabbitMqProviderOptions> optionsAction)
    {
        var options = new RabbitMqProviderOptions();
        optionsAction(options);
        
        var provider = new RabbitMqProvider(settings, options);
        builder.UseExternalProvider(provider);
        
        return builder;
    }
}
