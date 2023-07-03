using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
    public class CosmosDbService :ICosmosDbService
    { 
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName;
        private readonly string _collectionName;

       public CosmosDbService(IOptions<CosmosDbSettings> cosmosDbOptions)
         {
             var cosmosDbSettings = cosmosDbOptions.Value;
             _cosmosClient = new CosmosClient(cosmosDbSettings.EndpointUrl, cosmosDbSettings.AuthKey);
             _databaseName = cosmosDbSettings.DatabaseName;
             _collectionName = cosmosDbSettings.CollectionName;
         }

         public async Task<Document> CreateDocumentAsync(object document)
         {
             var container = _cosmosClient.GetContainer(_databaseName, _collectionName);
             var result = await container.CreateItemAsync(document);
             return (Document)result;
         }

           Task<Document> ICosmosDbService.CreateDocumentAsync(object document)
           {
               throw new NotImplementedException();
           }

        

     
    }

}
