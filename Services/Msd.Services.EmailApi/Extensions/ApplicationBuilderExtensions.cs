using Msd.Services.EmailApi.Messaging;

namespace Msd.Services.EmailApi.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer? ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>()
                ?? throw new Exception($"Internal error: {nameof(IAzureServiceBusConsumer)} isn't registered in IoC!");
            
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>()
                ?? throw new Exception($"Internal error: {nameof(IHostApplicationLifetime)} isn't registered in IoC!");

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            ServiceBusConsumer?.Stop();  
        }

        private static void OnStart()
        {
            ServiceBusConsumer?.Start();
        }
    }
}
