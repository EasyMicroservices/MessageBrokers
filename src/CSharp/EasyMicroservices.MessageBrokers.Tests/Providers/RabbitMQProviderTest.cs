using EasyMicroservices.MessageBrokers.RabbitMQ.Providers;

namespace EasyMicroservices.MessageBrokers.Tests.Providers;
public class RabbitMQProviderTest : BaseProviderTest
{
    public RabbitMQProviderTest() : base(new RabbitMQProvider(new EasyMicroservices.Serialization.Newtonsoft.Json.Providers.NewtonsoftJsonProvider()))
    {
    }
}