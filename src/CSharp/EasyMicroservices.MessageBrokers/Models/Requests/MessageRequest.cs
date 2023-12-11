namespace EasyMicroservices.MessageBrokers.Models.Requests;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class MessageRequest<T>
{
    /// <summary>
    /// queue or topic name
    /// </summary>
    public string GroupName { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public T Message { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>

    public static implicit operator MessageRequest<T>((string groupName, T message) request)
    {
        return new MessageRequest<T>()
        {
            GroupName = request.groupName,
            Message = request.message,
        };
    }
}
