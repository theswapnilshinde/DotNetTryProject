using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Interfaces
{
    public interface ICosmosDbService
    {
         Task<Document> CreateDocumentAsync(object document);
     
    }
}
