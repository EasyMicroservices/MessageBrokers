namespace EasyMicroservices.MessageBrokers.Interfaces;
public interface IAsyncMessageHandler<T> : IMessageHandler<T>
{
    Task HandleMessageAsync(T message);
}
