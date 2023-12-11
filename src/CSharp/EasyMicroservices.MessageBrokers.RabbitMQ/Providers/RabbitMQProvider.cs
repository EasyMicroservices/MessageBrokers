using EasyMicroservices.MessageBrokers.Interfaces;
using EasyMicroservices.MessageBrokers.Models.Requests;
using EasyMicroservices.Serialization.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace EasyMicroservices.MessageBrokers.RabbitMQ.Providers;
/// <summary>
/// 
/// </summary>
public class RabbitMQProvider : IMessageBrokerProvider
{
    ConnectionFactory _connectionFactory;
    ITextSerializationProvider _serializer;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connectionFactory"></param>
    /// <param name="serializer"></param>
    public RabbitMQProvider(ConnectionFactory connectionFactory, ITextSerializationProvider serializer)
    {
        _serializer = serializer;
        _connectionFactory = connectionFactory;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serializer"></param>
    public RabbitMQProvider(ITextSerializationProvider serializer)
    {
        _serializer = serializer;
        _connectionFactory = new ConnectionFactory()
        {
            HostName = "localhost"
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageRequest"></param>
    /// <returns></returns>
    public Task SendAsync<T>(MessageRequest<T> messageRequest)
    {
        using (var connection = _connectionFactory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: messageRequest.GroupName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                if (messageRequest.Message is string text)
                {
                    var body = Encoding.UTF8.GetBytes(text);
                    channel.BasicPublish(exchange: "", routingKey: messageRequest.GroupName, basicProperties: null, body: body);
                }
                else
                {
                    var body = Encoding.UTF8.GetBytes(_serializer.Serialize(messageRequest.Message));
                    channel.BasicPublish(exchange: "", routingKey: messageRequest.GroupName, basicProperties: null, body: body);
                }
            }
        }
        return Task.CompletedTask;
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
        var connection = _connectionFactory.CreateConnection();
        var channel = connection.CreateModel();
        channel.QueueDeclare(queue: subscribeRequest.GroupName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            if (typeof(T) == typeof(string))
                _ = RabbitMQ_OnMessage(handler, (T)(object)message);
            else
                _ = RabbitMQ_OnMessage(handler, _serializer.Deserialize<T>(message));

        };
        channel.BasicConsume(queue: subscribeRequest.GroupName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    async Task RabbitMQ_OnMessage<T>(IMessageHandler<T> handler, T message)
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
