using EasyMicroservices.MessageBrokers.ActiveMQ.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyMicroservices.MessageBrokers.Tests.Providers;
public class ActiveMQProviderTest : BaseProviderTest
{
    public ActiveMQProviderTest() : base(new ActiveMQProvider())
    {
    }
}