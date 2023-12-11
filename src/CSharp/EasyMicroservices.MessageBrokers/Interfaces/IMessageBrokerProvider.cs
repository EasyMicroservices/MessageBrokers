using EasyMicroservices.MessageBrokers.Models.Requests;

namespace EasyMicroservices.MessageBrokers.Interfaces;
public interface IMessageBrokerProvider
{
    // Publish a message to a topic or queue
    Task SendAsync<T>(MessageRequest<T> messageRequest);

    // Subscribe to a topic or queue
    Task SubscribeAsync<T>(SubscribeRequest subscribeRequest, IMessageHandler<T> handler);

    // Unsubscribe from a topic or queue
    Task UnsubscribeAsync(SubscribeRequest subscribeRequest);

    //// Acknowledge that a message has been processed successfully
    //Task AcknowledgeAsync(string messageId);

    //// Reject a message in case of an error during processing
    //Task RejectAsync(string messageId);
}
