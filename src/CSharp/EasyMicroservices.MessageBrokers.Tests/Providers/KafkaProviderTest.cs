using EasyMicroservices.MessageBrokers.Kafka.Providers;

namespace EasyMicroservices.MessageBrokers.Tests.Providers;
public class KafkaProviderTest : BaseProviderTest
{
    public KafkaProviderTest() : base(new KafkaProvider(new EasyMicroservices.Serialization.Newtonsoft.Json.Providers.NewtonsoftJsonProvider()))
    {
    }
}