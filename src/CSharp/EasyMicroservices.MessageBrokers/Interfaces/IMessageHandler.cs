namespace EasyMicroservices.MessageBrokers.Interfaces;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IMessageHandler<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task HandleMessage(T message);
}
