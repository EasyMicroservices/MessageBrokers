using Confluent.Kafka;
using EasyMicroservices.MessageBrokers.Interfaces;
using EasyMicroservices.MessageBrokers.Models.Requests;
using EasyMicroservices.Serialization.Interfaces;

namespace EasyMicroservices.MessageBrokers.Kafka.Providers;
/// <summary>
/// 
/// </summary>
public class KafkaProvider : IMessageBrokerProvider
{
    ProducerConfig _producerConfig;
    ITextSerializationProvider _serializer;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="producerConfig"></param>
    /// <param name="serializer"></param>
    public KafkaProvider(ProducerConfig producerConfig, ITextSerializationProvider serializer)
    {
        _serializer = serializer;
        _producerConfig = producerConfig;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serializer"></param>
    public KafkaProvider(ITextSerializationProvider serializer)
    {
        _serializer = serializer;
        _producerConfig = new ProducerConfig()
        {
            BootstrapServers = "localhost:9092"
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageRequest"></param>
    /// <returns></returns>
    public async Task SendAsync<T>(MessageRequest<T> messageRequest)
    {
        using (var p = new ProducerBuilder<Null, string>(_producerConfig).Build())
        {
            if (messageRequest.Message is string text)
            {
                var dr = await p.ProduceAsync(messageRequest.GroupName, new Message<Null, string> { Value = text });
            }
            else
            {
                var body = _serializer.Serialize(messageRequest.Message);
                var dr = await p.ProduceAsync(messageRequest.GroupName, new Message<Null, string> { Value = body });
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscribeRequest"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public Task SubscribeAsync<T>(SubscribeRequest subscribeRequest, IMessageHandler<T> handler)
    {
        var conf = new ConsumerConfig
        {
            GroupId = Guid.NewGuid().ToString(),
            BootstrapServers = _producerConfig.BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
                    {
                        c.Subscribe(subscribeRequest.GroupName);

                        try
                        {
                            while (true)
                            {
                                var cr = c.Consume();
                                var body = cr.Message.Value;
                                if (typeof(T) == typeof(string))
                                    _ = Kafka_OnMessage(handler, (T)(object)body);
                                else
                                    _ = Kafka_OnMessage(handler, _serializer.Deserialize<T>(body));
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            c.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var a = ex;
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });

        return Task.CompletedTask;
    }

    async Task Kafka_OnMessage<T>(IMessageHandler<T> handler, T message)
    {
        await handler.HandleMessage(message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="subscribeRequest"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task UnsubscribeAsync(SubscribeRequest subscribeRequest)
    {
        throw new NotImplementedException();
    }
}
