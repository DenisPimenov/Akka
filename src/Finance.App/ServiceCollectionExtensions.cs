using System.IO;
using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Finance.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.App
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSystem(this IServiceCollection services)
        {
            var config = ConfigurationFactory.ParseString(File.ReadAllText("ActorSystemSettings.hocon"));
            var system = ActorSystem.Create("FinanceSystem", config);
            StandardOutLogger.UseColors = false;
            system.GetTransactionActor();
            services.AddSingleton(p => system);
        }
    }
}