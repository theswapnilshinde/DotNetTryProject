using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Interfaces
{
    public interface IPeople
    {
        Task<bool> Create(string idd, object varPeople);
        Task<People> GetOne(string id);
        Task<bool> Update(object varPeople, string id);
        Task<bool> Delete(string id);
        bool peopleValidation(People people);
        List<People> getllPeople();
    }
}
