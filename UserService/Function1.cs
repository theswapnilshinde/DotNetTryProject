using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UserService.Interfaces;
using UserService.Models;


namespace TestFunction
{

    public class Function1
    {

        private readonly IPeople _ipeopleService;
        public Function1(IPeople ipeople)
        {
            _ipeopleService = ipeople;
        }

        [FunctionName("CreateUser")]
        public async Task<IActionResult> CreateUser([Microsoft.Azure.WebJobs.HttpTrigger
            (AuthorizationLevel.Function, "post", Route = "People")] HttpRequest req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                People people = JsonConvert.DeserializeObject<People>(requestBody);
                var varPeople = JsonConvert.DeserializeObject(requestBody);

                string idd = people.Id;
                bool validatePeople = _ipeopleService.peopleValidation(people);

                if (validatePeople == false)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    bool people1 = await _ipeopleService.Create(idd, varPeople);
                    if (people1 == true)
                    {
                        return new StatusCodeResult(StatusCodes.Status201Created);
                    }
                    else
                    {
                        return new StatusCodeResult(StatusCodes.Status302Found);
                    }
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [FunctionName("get")]
        public async Task<IActionResult> getOneData([Microsoft.Azure.WebJobs.HttpTrigger
            (AuthorizationLevel.Function, "get", Route = "get/{id}")] HttpRequest req, string id)
        {
            try
            {
                string peopleId = req.Query["id"];
                People people = await _ipeopleService.GetOne(peopleId);

                if (people != null)
                {
                    return new OkObjectResult(people);
                }

                else
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [FunctionName("delete")]
        public async Task<IActionResult> delete([Microsoft.Azure.WebJobs.HttpTrigger
            (AuthorizationLevel.Function, "delete", Route = "delete/{id}")] HttpRequest req, string id)
        {
            try
            {
                string itemId = req.Query["id"];
                bool flag = await _ipeopleService.Delete(itemId);
                if (flag == true)
                {
                    return new StatusCodeResult(StatusCodes.Status202Accepted);
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status404NotFound);
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [FunctionName("update")]
        public async Task<IActionResult> update([Microsoft.Azure.WebJobs.HttpTrigger
             (AuthorizationLevel.Function, "put", Route = "item")] HttpRequest req)//,string id)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                People people = JsonConvert.DeserializeObject<People>(requestBody);
                string id = people.Id;

                var varPeople = JsonConvert.DeserializeObject(requestBody);
                bool validatePeople = _ipeopleService.peopleValidation(people);

                if (validatePeople == false)
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
                else
                {
                    bool update = await _ipeopleService.Update(varPeople, id);

                    if (update == true)
                    {
                        return new OkObjectResult(StatusCodes.Status201Created);
                    }
                    else
                    {
                        return new StatusCodeResult(StatusCodes.Status404NotFound);
                    }
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        [FunctionName("getAll")]
        public async Task<IActionResult> getAll([Microsoft.Azure.WebJobs.HttpTrigger
            (AuthorizationLevel.Function, "get", Route = "getall")] HttpRequest req)
        {
            try
            {
                List<People> peoples =  _ipeopleService.getllPeople();

                if (peoples != null)
                {
                    return new OkObjectResult(peoples);
                }
                else
                {
                    return new StatusCodeResult(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }



    }
}