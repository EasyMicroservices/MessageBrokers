namespace EasyMicroservices.MessageBrokers.Interfaces;
public interface IMessageHandler<T>
{
    Task HandleMessage(T message);
}
