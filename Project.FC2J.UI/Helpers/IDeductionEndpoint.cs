using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;

namespace Project.FC2J.UI.Helpers
{
    public interface IDeductionEndpoint : IEndpoint<Deduction>
    {
        void SetParameters(int isUsed, long customerId);
        Task Remove(long id, long customerId);
        Task<List<Deduction>> GetDeductions(string valuePoNo, long valueCustomerId);
    }
}
