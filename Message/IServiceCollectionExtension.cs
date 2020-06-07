using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Messaging
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddMessagingLibrary(this IServiceCollection services)
        {
            services.AddSingleton<IMessageQueue, MessageQueue>();

            return services;
        }
    }
}
