using EasyMicroservices.MessageBrokers.Interfaces;
using System;
using System.Threading.Tasks;

namespace EasyMicroservices.MessageBrokers.Tests.Providers;

public abstract class BaseProviderTest
{
    readonly IMessageBrokerProvider _messageBrokerProvider;
    public BaseProviderTest(IMessageBrokerProvider messageBrokerProvider)
    {
        _messageBrokerProvider = messageBrokerProvider;
    }

    [Theory]
    [InlineData("Hello world!")]
    [InlineData("Hello!")]
    [InlineData("World!")]
    [InlineData("EasyMicroservices")]
    public async Task SendText(string text)
    {
        string groupName = Guid.NewGuid().ToString();
        TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
        var handler = new TextMessageHandler()
        {
            OnMessage = (msg) =>
            {
                try
                {
                    taskCompletionSource.SetResult(msg);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }
        };
        await _messageBrokerProvider.SubscribeAsync(new Models.Requests.SubscribeRequest()
        {
            GroupName = groupName
        }, handler);
        await _messageBrokerProvider.SendAsync(new Models.Requests.MessageRequest<string>()
        {
            GroupName = groupName,
            Message = text
        });

        var result = await Task.WhenAny(taskCompletionSource.Task, Task.Run<string>(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(20));
            throw new TimeoutException("Test timeout!");
        }));
        var value = result.Result;
        Assert.Equal(text, value);
    }

#if (!NET8_0)
    [Theory]
    [InlineData("Hello world!")]
    [InlineData("Hello!")]
    [InlineData("World!")]
    [InlineData("EasyMicroservices")]
    public async Task SendObject(string text)
    {
        string groupName = Guid.NewGuid().ToString();
        TaskCompletionSource<MessageObject> taskCompletionSource = new TaskCompletionSource<MessageObject>();
        var handler = new ObjectMessageHandler()
        {
            OnMessage = (msg) =>
            {
                try
                {
                    taskCompletionSource.SetResult(msg);
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            }
        };
        await _messageBrokerProvider.SubscribeAsync(new Models.Requests.SubscribeRequest()
        {
            GroupName = groupName
        }, handler);
        await _messageBrokerProvider.SendAsync(new Models.Requests.MessageRequest<MessageObject>()
        {
            GroupName = groupName,
            Message = new MessageObject()
            {
                 Text = text
            }
        });

        var result = await Task.WhenAny(taskCompletionSource.Task, Task.Run<MessageObject>(async () =>
        {
            await Task.Delay(5000);
            throw new TimeoutException("Test timeout!");
        }));
        var value = result.Result;
        Assert.Equal(text, value.Text);
    }
#endif
}

public class TextMessageHandler : IMessageHandler<string>
{
    public Action<string> OnMessage { get; set; }
    public Task HandleMessage(string message)
    {
        OnMessage?.Invoke(message);
        return Task.FromResult(0);
    }
}

public class ObjectMessageHandler : IMessageHandler<MessageObject>
{
    public Action<MessageObject> OnMessage { get; set; }
    public Task HandleMessage(MessageObject message)
    {
        OnMessage?.Invoke(message);
        return Task.FromResult(0);
    }
}

[Serializable]
public class MessageObject
{
    public string Text { get; set; }
}
