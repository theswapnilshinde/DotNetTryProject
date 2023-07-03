
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Models;
using UserService.Services;

[assembly: FunctionsStartup(typeof(UserService.Startup))]
namespace UserService
{
    public class Startup: FunctionsStartup
    {

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IPeople, PeopleService>();
            //builder.Services.AddScoped<IpeopleValidation, PeopleValidation>();
            // builder.Services.AddScoped<IpeopleValidation, >();

        builder.Services.AddSingleton((s) =>
            {
                string endpointUri = "https://swapnil.documents.azure.com:443/";
                string primaryKey = "5CsDUijT51NvNs60jojvBoeZw8LNK3hCMomxl23Sob8BbzRQhvXvjhOXjgi8ZfUuV5sSAUvvLuyPACDbntLaOw==";
                string databaseName = "swapnil";
                string containerName = "Users";

                var cosmosClient = new CosmosClient(endpointUri, primaryKey);
                var database = cosmosClient.GetDatabase(databaseName);
                Container container = database.GetContainer(containerName);
                return container;
            });
        }

    }
}
