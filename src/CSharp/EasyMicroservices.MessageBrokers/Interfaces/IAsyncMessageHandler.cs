﻿namespace EasyMicroservices.MessageBrokers.Interfaces;
/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IAsyncMessageHandler<T> : IMessageHandler<T>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task HandleMessageAsync(T message);
}
