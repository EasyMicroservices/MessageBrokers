namespace EasyMicroservices.MessageBrokers.Models.Requests;
public class MessageRequest<T>
{
    /// <summary>
    /// queue or topic name
    /// </summary>
    public string GroupName { get; set; }
    public T Message { get; set; }

    public static implicit operator MessageRequest<T>((string groupName, T message) request)
    {
        return new MessageRequest<T>()
        {
            GroupName = request.groupName,
            Message = request.message,
        };
    }
}
