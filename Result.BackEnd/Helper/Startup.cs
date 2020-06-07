using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Result.BackEnd.Repository;

[assembly: FunctionsStartup(typeof(Result.BackEnd.Helper.Startup))]
namespace Result.BackEnd.Helper
{
    public class Startup : FunctionsStartup
    {        
        public override void Configure(IFunctionsHostBuilder builder)
        {           
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });
            builder.Services.AddSingleton<IRepository, VoteRepository>();
        }
    }
}
