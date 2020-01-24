using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;

namespace Project.FC2J.DataStore.Interfaces.Codesets
{
    public interface IDeductionRepository : IRepository<Deduction>
    {
        Task<List<Deduction>> GetList(int isUsed, long customerId);
        Task<List<Deduction>> GetList(long customerId);
        Task Remove(long id, long customerId);
        Task<List<Deduction>> GetDeductions(string valuePoNo, long valueCustomerId);
    }
}
