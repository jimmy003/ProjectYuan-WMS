using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.FC2J.Models.Customer;
using Project.FC2J.UI.Helpers.AppSetting;

namespace Project.FC2J.UI.Helpers
{
    public class DeductionEndpoint :IDeductionEndpoint
    {
        private readonly IApiAppSetting _apiAppSetting;
        private readonly IAPIHelper _apiHelper;


        public int IsUsed { get; private set; }
        public long CustomerId { get; private set; }
        public DeductionEndpoint(IAPIHelper apiHelper, IApiAppSetting apiAppSetting)
        {
            _apiHelper = apiHelper;
            _apiAppSetting = apiAppSetting;
        }

        public async Task<List<Deduction>> GetList()
        {
            return await _apiHelper.GetList<Deduction>(_apiAppSetting.Deduction + $"?isUsed={IsUsed}&customerId={CustomerId}");
        }

        public async Task<Deduction> Save(Deduction value)
        {
            return await _apiHelper.Save(_apiAppSetting.Deduction, value);
        }

        public async Task Update(Deduction value)
        {
            await _apiHelper.Update(_apiAppSetting.Deduction, value);
        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public void SetParameters(int isUsed, long customerId)
        {
            IsUsed = isUsed;
            CustomerId = customerId;
        }

        public async Task Remove(long id, long customerId)
        {
            await _apiHelper.Remove(_apiAppSetting.Deduction + $"?id={id}&customerId={CustomerId}");
        }

        public async Task<List<Deduction>> GetDeductions(string valuePoNo, long valueCustomerId)
        {
            return await _apiHelper.GetList<Deduction>(_apiAppSetting.Deduction + $"?PONo={valuePoNo}&customerId={valueCustomerId}");
        }
    }
}
