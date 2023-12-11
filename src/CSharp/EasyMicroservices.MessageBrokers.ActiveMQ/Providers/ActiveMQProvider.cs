using Apache.NMS;
using EasyMicroservices.MessageBrokers.Interfaces;
using EasyMicroservices.MessageBrokers.Models.Requests;

namespace EasyMicroservices.MessageBrokers.ActiveMQ.Providers;

/// <summary>
/// 
/// </summary>
public class ActiveMQProvider : IMessageBrokerProvider
{
    Apache.NMS.IConnection _connection;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    public ActiveMQProvider(Apache.NMS.IConnection connection)
    {
        _connection = connection;
    }

    /// <summary>
    /// 
    /// </summary>
    public ActiveMQProvider()
    {
        string brokerUri = "tcp://localhost:61616";
        Apache.NMS.IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(new Uri(brokerUri));
        _connection = factory.CreateConnection();
        _connection.Start();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageRequest"></param>
    /// <returns></returns>
    public async Task SendAsync<T>(MessageRequest<T> messageRequest)
    {
        ISession session = await _connection.CreateSessionAsync();
        IDestination destination = session.GetTopic(messageRequest.GroupName);
        IMessageProducer producer = session.CreateProducer(destination);
        if (messageRequest.Message is string textMessage)
        {
            ITextMessage message = await session.CreateTextMessageAsync(textMessage);
            await producer.SendAsync(message);
        }
        else
        {
            IObjectMessage message = await session.CreateObjectMessageAsync(messageRequest.Message);
            await producer.SendAsync(message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscribeRequest"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    public async Task SubscribeAsync<T>(SubscribeRequest subscribeRequest, IMessageHandler<T> handler)
    {
        ISession session = await _connection.CreateSessionAsync();

        IDestination destination = session.GetTopic(subscribeRequest.GroupName);

        // ایجاد یک اشتراک‌گذار برای دریافت پیام‌ها
        IMessageConsumer consumer = session.CreateConsumer(destination);
        consumer.Listener += new MessageListener((msg) =>
        {
            _ = ActiveMQ_OnMessage(handler, msg);
        });
    }

    async Task ActiveMQ_OnMessage<T>(IMessageHandler<T> handler, IMessage message)
    {
        await handler.HandleMessage(message.Body<T>());
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
