using EasyMicroservices.MessageBrokers.Models.Requests;

namespace EasyMicroservices.MessageBrokers.Interfaces;
/// <summary>
/// 
/// </summary>
public interface IMessageBrokerProvider
{
    /// <summary>
    /// Publish a message to a topic or queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageRequest"></param>
    /// <returns></returns>
    Task SendAsync<T>(MessageRequest<T> messageRequest);

    /// <summary>
    /// Subscribe to a topic or queue
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subscribeRequest"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    Task SubscribeAsync<T>(SubscribeRequest subscribeRequest, IMessageHandler<T> handler);

    /// <summary>
    /// Unsubscribe from a topic or queue
    /// </summary>
    /// <param name="subscribeRequest"></param>
    /// <returns></returns>
    Task UnsubscribeAsync(SubscribeRequest subscribeRequest);

    //// Acknowledge that a message has been processed successfully
    //Task AcknowledgeAsync(string messageId);

    //// Reject a message in case of an error during processing
    //Task RejectAsync(string messageId);
}
