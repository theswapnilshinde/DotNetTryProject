using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace UserService.Services
{
    public class PeopleService : IPeople
    {

        private readonly Container _container;
        string key1 = "59ae81bd6d043e9d2a69da98f9f6d70e257a9a2d";

        public PeopleService(Container container)
        {
            _container = container;
           
        }
        //
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName="swapnil";
        private readonly string _containerName="users";

        public PeopleService(IConfiguration configuration)
        {
            var cosmosDbSettings = new CosmosDbSettings();
            configuration.GetSection("CosmosDbSettings").Bind(cosmosDbSettings);

            _cosmosClient = new CosmosClient(cosmosDbSettings.EndpointUrl, cosmosDbSettings.AuthKey);
            _databaseName = cosmosDbSettings.DatabaseName;
            _containerName = cosmosDbSettings.CollectionName;
        }


        //
        async Task<bool> IPeople.Create(string idd, object vaePeople)
        {
            try
            {
                bool existingPeople = await FindById(idd);
                if (!existingPeople)
                {
                    // var newPeople = await _container.CreateItemAsync(vaePeople, new PartitionKey(idd));
                    var container = _cosmosClient.GetContainer(_databaseName, _containerName);
                    await container.CreateItemAsync(vaePeople, new PartitionKey(idd));
                    return true;
                }
                else
                {
                    Console.WriteLine("already exists");
                    return false;
                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
        async Task<bool> FindById(string id)
        {
            try
            {
                var existingPeople = await _container.ReadItemAsync<People>(id, new PartitionKey(id));
                People people = existingPeople;
               
                return true;
                 
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; 
            }
        }

        public async Task<People> GetOne(string id)
        {
            try
            {
                People people = await _container.ReadItemAsync<People>(id, new PartitionKey(id));
                return people;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"People object with ID '{id}' not found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> Update(object varPeople, string id)
        {
            try
            {
               // var existingPeople = await _container.ReadItemAsync<People>(id, new PartitionKey(id));
               bool flag = await FindById(id);
                if (flag == true)
                {
                    var people = await _container.ReplaceItemAsync(varPeople, id, new PartitionKey(id));
                    return true;
                   
                }
                else
                {
                    Console.WriteLine($"People object with ID '{id}' not found");
                    return false;
              
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"People object with ID '{id}' not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var existingPeople = await _container.ReadItemAsync<People>(id, new PartitionKey(id));
                if (existingPeople != null)
                {
                    var result = await _container.DeleteItemAsync<People>(id, new PartitionKey(id));
                    return result.StatusCode == HttpStatusCode.NoContent;
                }
                else
                {
                    Console.WriteLine($"People object with ID '{id}' not found");
                    return false;
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"People object with ID '{id}' not found");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
      
        public bool peopleValidation(People people)
        {
            if (string.IsNullOrEmpty(people.Id))
            {
                return false;
            }
            if (string.IsNullOrEmpty(people.FirstName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(people.LastName))
            {
                return false;
            }

            /* if (string.IsNullOrEmpty(people.Email))
             {
                 return false;
             }
             if (string.IsNullOrEmpty(people.Phone))
             {
                 return false;
             }
             if (string.IsNullOrEmpty(people.City))
             {
                 return false;
             }*/
            else
            {
                return true;
            }
        }

        List<People> IPeople.getllPeople()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var results = new List<People>();
            try
            {
                using (var resultSetIterator = _container.GetItemQueryIterator<People>(query))
                {
                    while (resultSetIterator.HasMoreResults)
                    {
                        var response =  resultSetIterator.ReadNextAsync();
                        results.AddRange(response.Result);
                    }
                }
                return results;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"People object with  not found");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }


        /*  async Task<People> IPeople.Create(string idd, object vaePeople)
          {
              try
              {

                  var newPeople = await _container.CreateItemAsync(vaePeople, new PartitionKey(idd));
                  People people = (People)newPeople;
                  return people;

              }
              catch (Exception ex)
              {
                  //
                  Console.WriteLine($"An error occurred: {ex.Message}");
                 // Console.WriteLine($"An error occurred: {ex.Message}");
                  string message = ex.Message;          

              }


          }*/

        /*  string IPeople.createAsync(string idd, object vaePeople)
        {
            try
            {
             bool s=   findById(idd);
                string message = "created";
               _container.CreateItemAsync(vaePeople, new PartitionKey(idd));
                return message;

            }
            catch (Exception ex)
            {
                //
                Console.WriteLine($"An error occurred: {ex.Message}");
                string message = ex.Message;
                return message;
            }


         }*/


        /* async Task<People> IPeople.Create3(string idd, object vaePeople)
         {
             try
             {
                 //var existingPeople = await _container.ReadItemAsync<People>(idd, new PartitionKey(idd));
                 bool existingPeople = FindById(idd);
                 if (existingPeople == true)
                 {
                     var newPeople = await _container.CreateItemAsync(vaePeople, new PartitionKey(idd));
                    People people = new People();

                     people.Id = idd;
                     return people;
                 }
                 else
                 {
                     // Item already exists, handle accordingly
                     Console.WriteLine("People object already exists");
                     // return existingPeople.Resource;
                     return null;
                 }
             }
             catch (Exception ex)
             {
                 // Handle the exception or return a default value
                 Console.WriteLine($"An error occurred: {ex.Message}");
                 return null;
             }
         }*/




    }


}
