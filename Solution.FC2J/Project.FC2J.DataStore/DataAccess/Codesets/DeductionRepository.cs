using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Project.FC2J.DataStore.Interfaces.Codesets;
using Project.FC2J.DataStore.Internal.DataAccess;
using Project.FC2J.Models.Customer;

namespace Project.FC2J.DataStore.DataAccess.Codesets
{
    public class DeductionRepository : IDeductionRepository
    {

        private readonly string _spManageSaleDeduction = "ManageSaleDeduction";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isUsed">0 when records is not used and 1 when records is used</param>
        /// <param name="customerId">Customer Id</param>
        /// <returns></returns>
        public async Task<List<Deduction>> GetList(int isUsed, long customerId)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", customerId)
            };
            //sqlParameters.Add(isUsed > 0 ? new SqlParameter("@t", 4) : new SqlParameter("@t", 3));
            sqlParameters.Add(new SqlParameter("@t", 5));

            var value = await _spManageSaleDeduction.GetList<Deduction>(sqlParameters.ToArray());
            return value;
        }

        public async Task<List<Deduction>> GetList(long customerId)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@t", 5)
            };
            var value = await _spManageSaleDeduction.GetList<Deduction>(sqlParameters.ToArray());
            return value;
        }

        public async Task Remove(long id, long customerId)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", customerId),
                new SqlParameter("@Id", id),
                new SqlParameter("@t", 6)
            };
            await _spManageSaleDeduction.ExecuteNonQueryAsync(sqlParameters.ToArray());

        }

        public async Task<List<Deduction>> GetDeductions(string valuePoNo, long valueCustomerId)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@CustomerId", valueCustomerId),
                new SqlParameter("@PONo", valuePoNo),
                new SqlParameter("@t", 8)
            };
            var value = await _spManageSaleDeduction.GetList<Deduction>(sqlParameters.ToArray());
            return value;

        }

        public Task Remove(long id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Deduction>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<Deduction> Save(Deduction value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@t", 1),
                new SqlParameter("@CustomerId", value.CustomerId),
                new SqlParameter("@Particular", value.Particular.Replace("'","''")),
                new SqlParameter("@Amount", value.Amount)
            };

            value = await _spManageSaleDeduction.GetRecord<Deduction>(sqlParameters.ToArray());
            return value;
        }

        public async Task Update(Deduction value)
        {
            var sqlParameters = new List<SqlParameter>()
            {
                new SqlParameter("@t", 2),
                new SqlParameter("@Id", value.Id),
                new SqlParameter("@PONo", value.PONo),
                new SqlParameter("@UsedAmount", value.UsedAmount),
                new SqlParameter("@CustomerId", value.CustomerId),
                new SqlParameter("@Particular", value.Particular),
                new SqlParameter("@Amount", value.Amount)
            };

            await _spManageSaleDeduction.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }
    }
}
