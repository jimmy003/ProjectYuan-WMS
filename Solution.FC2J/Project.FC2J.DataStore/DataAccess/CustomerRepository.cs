using Project.FC2J.Models.Customer;
using Project.FC2J.Models.Product;
using Project.FC2J.DataStore.Interfaces;
using Project.FC2J.DataStore.Internal.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Project.FC2J.DataStore
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly string _removeCustomer = "RemoveCustomer";
        private readonly string _insertCustomer = "InsertCustomer";
        private readonly string _updateCustomer = "UpdateCustomer";
        private readonly string _insertCustomerPayment = "InsertCustomerPayment";
        private readonly string _removeCustomerPayment = "RemoveCustomerPayment";
        private readonly string _insertCustomerPricelist = "InsertCustomerPricelist";
        private readonly string _removeCustomerPricelist = "RemoveCustomerPricelist";
        private readonly string _insertCustomerProduct = "InsertCustomerProduct";
        private readonly string _removeCustomerProduct = "RemoveCustomerProduct";
        private readonly string _insertCustomerShippingAddress = "InsertCustomerShippingAddress";
        private readonly string _removeCustomerShippingAddress = "RemoveCustomerShippingAddress";

        private readonly string _spGetCustomerPayment = "GetCustomerPayment";
        private readonly string _spGetPayments = "GetPayments";
        private readonly string _spGetCustomers = "GetCustomers";
        private readonly string _spGetCustomerPriceList = "GetCustomerPriceList";

        private readonly string _spUpdatePricelistTemplate = "UpdatePricelistTemplate";
        private readonly string _spGetFarms = "GetFarms";

        private Customer _customer;

        public async Task<IEnumerable<Farm>> GetFarms()
        {
            var list = new List<Farm>();
            list = await _spGetFarms.GetList<Farm>();
            return list;
        }

        public async Task<IEnumerable<Payment>> GetCustomerPayment(long id)
        {
            var list = new List<Payment>();
            var sqlParameters = new[]
            {
                new SqlParameter("@CustomerId", id)
            };
            list = await _spGetCustomerPayment.GetList<Payment>(sqlParameters);

            return list;
        }

        public async Task<IEnumerable<Product>> GetCustomerPriceList(long id, int supplier)
        {
            var list = new List<Product>();
            var sqlParameters = new[]
            {
                new SqlParameter("@CustomerId", id),
                new SqlParameter("@SupplierId", supplier)
            };
            list = await _spGetCustomerPriceList.GetList<Product>(sqlParameters);
            return list;
        }

        public async Task<IEnumerable<Product>> GetCustomerProduct(long id)
        {
            var list = new List<Product>();
            var sqlParameters = new[]
            {
                new SqlParameter("@CustomerId", id)
            };
            using (var connection = await DbHelper.GetOpenConnectionAsync())
            {
                using (var reader = await connection.ExecuteReaderAsync("GetCustomerProduct", sqlParameters))
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetAsync<long>("Id").Result,
                            SFAReferenceNo = reader.GetAsync<string>("SFAReferenceNo").Result,
                            Name = reader.GetAsync<string>("Name").Result,
                            Description = reader.GetAsync<string>("Description").Result,
                            Category = reader.GetAsync<string>("Category").Result,
                            UnitOfMeasure = reader.GetAsync<string>("UnitOfMeasure").Result,
                            SalePrice = reader.GetAsync<decimal>("SalePrice").Result,
                            CostPrice = reader.GetAsync<decimal>("CostPrice").Result,
                            SFAUnitOfMeasure = reader.GetAsync<string>("SFAUnitOfMeasure").Result,
                            IsTaxable = reader.GetAsync<bool>("IsTaxable").Result,
                            Deleted = reader.GetAsync<bool>("Deleted").Result
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var list = new List<Customer>();
            list = await _spGetCustomers.GetList<Customer>();

            return list;
        }

        public async Task<IEnumerable<Customer>> GetCustomersByPartialName(string partialName)
        {
            var list = new List<Customer>();
            var sqlParameters = new[]
            {
                new SqlParameter("@PartialName", partialName)
            };
            using (var connection = await DbHelper.GetOpenConnectionAsync())
            {
                using (var reader = await connection.ExecuteReaderAsync("GetCustomersByNamePartial", sqlParameters))
                {
                    while (reader.Read())
                    {
                        list.Add(new Customer
                        {
                            Id = reader.GetAsync<Int64>("Id").Result,
                            Name = reader.GetAsync<string>("Name").Result,
                            Address1 = reader.GetAsync<string>("Address1").Result,
                            Address2 = reader.GetAsync<string>("Address2").Result,
                            MobileNo = reader.GetAsync<string>("MobileNo").Result,
                            TelNo = reader.GetAsync<string>("TelNo").Result,
                            TIN = reader.GetAsync<string>("TIN").Result,
                            PaymentType = reader.GetAsync<string>("PaymentType").Result,
                            PriceList = reader.GetAsync<string>("PriceList").Result,
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<ShippingAddress>> GetCustomerShipTo(long id)
        {
            var list = new List<ShippingAddress>();
            var sqlParameters = new[]
            {
                new SqlParameter("@CustomerId", id)
            };
            using (var connection = await DbHelper.GetOpenConnectionAsync())
            {
                using (var reader = await connection.ExecuteReaderAsync("GetCustomerShippingAddress", sqlParameters))
                {
                    while (reader.Read())
                    {
                        list.Add(new ShippingAddress
                        {
                            Id = reader.GetAsync<long>("Id").Result,
                            Name = reader.GetAsync<string>("Name").Result,
                            Address1 = reader.GetAsync<string>("Address1").Result,
                            Address2 = reader.GetAsync<string>("Address2").Result,
                            MobileNo = reader.GetAsync<string>("MobileNo").Result,
                            TelNo = reader.GetAsync<string>("TelNo").Result
                        });
                    }
                }
            }
            return list;
        }

        public async Task RemoveCustomer(long id)
        {
            var sqlParameters = new[]
            {
                new SqlParameter("@Id", id)
            };
            await _removeCustomer.ExecuteNonQueryAsync(sqlParameters);
        }

        public async Task<Customer> SaveCustomer(Customer customer)
        {
            _customer = customer;
            decimal id = 0;
            var result = new Customer();
            try
            {
                result = await _insertCustomer.GetRecord<Customer>(GetSqlParameters().ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //using (var connection = await DbHelper.GetOpenConnectionAsync())
            //{
            //    using (var reader = await connection.ExecuteReaderAsync(_insertCustomer, GetSqlParameters().ToArray()))
            //    {
            //        while (reader.Read())
            //        {
            //            id = reader.GetAsync<decimal>("Id").Result;
            //        }
            //    }
            //}
            customer.Id = result.Id; //Convert.ToInt64(id);
            //await SaveCustomerPayment(new CustomerPayment { Id = customer.Id, PaymentDetails = customer.PaymentDetails });
            //await SaveCustomerProduct(new CustomerProduct { Id = customer.Id, ProductDetails = customer.ProductDetails });
            //await SaveCustomerShippingAddress(new CustomerShipTo { Id = customer.Id, ShipToDetails = customer.ShipTo });
            return customer;
        }

        public async Task RemoveCustomerDetails(Int64 id)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", id)
            };
            await _removeCustomerPayment.ExecuteNonQueryAsync(sqlParameters.ToArray());

            try
            {
                await _removeCustomerPricelist.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            await _removeCustomerProduct.ExecuteNonQueryAsync(sqlParameters.ToArray());
            await _removeCustomerShippingAddress.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task RemoveCustomerPayment(Int64 id)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", id)
            };
            await _removeCustomerPayment.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task RemoveCustomerPricelist(Int64 id)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", id)
            };
            await _removeCustomerPricelist.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task RemoveCustomerProduct(Int64 id)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", id)
            };
            await _removeCustomerProduct.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task RemoveCustomerShippingAddress(Int64 id)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@CustomerId", id)
            };
            await _removeCustomerShippingAddress.ExecuteNonQueryAsync(sqlParameters.ToArray());
        }

        public async Task SaveCustomerPayment(CustomerPayment customerPayment)
        {
            foreach (var payment in customerPayment.PaymentDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerPayment.Id),
                    new SqlParameter("@PaymentId", payment.Id)
                };
                await _insertCustomerPayment.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
        }

        public async Task SaveCustomerProduct(CustomerProduct customerProduct)
        {
            foreach (var product in customerProduct.ProductDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerProduct.Id),
                    new SqlParameter("@ProductId", product.Id)
                };
                await _insertCustomerProduct.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
        }

        public async Task SaveCustomerPriceList(PriceListProduct value)
        {
            foreach (var deduction in value.ProductDeductions)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PriceListId", value.PriceListId),
                    new SqlParameter("@ProductId", deduction.Id),
                    new SqlParameter("@DeductionFixPrice", deduction.DeductionFixPrice),
                    new SqlParameter("@DeductionOutright", deduction.DeductionOutright),
                    new SqlParameter("@Discount", deduction.Discount),
                    new SqlParameter("@DeductionPromoDiscount", deduction.DeductionPromoDiscount)
                };
                await _spUpdatePricelistTemplate.ExecuteNonQueryAsync(sqlParameters.ToArray());
                sqlParameters = new List<SqlParameter>();
            }
            
        }

        public async Task SaveCustomerShippingAddress(CustomerShipTo customerShipTo)
        {
            foreach (var shipTo in customerShipTo.ShipToDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerShipTo.Id),
                    new SqlParameter("@Name", shipTo.Name),
                    new SqlParameter("@Address1", shipTo.Address1),
                    new SqlParameter("@Address2", shipTo.Address2),
                    new SqlParameter("@MobileNo", shipTo.MobileNo),
                    new SqlParameter("@TelNo", shipTo.TelNo)
                };
                decimal shipToId = 0;
                using (var connection = await DbHelper.GetOpenConnectionAsync())
                {
                    using (var reader = await connection.ExecuteReaderAsync(_insertCustomerShippingAddress, sqlParameters.ToArray()))
                    {
                        while (reader.Read())
                        {
                            shipToId = reader.GetAsync<decimal>("Id").Result;
                        }
                    }
                }
                shipTo.Id = Convert.ToInt64(shipToId);
            }
        }

        public async Task UpdateCustomer(Customer customer)
        {
            _customer = customer;
            var sqlParams = GetSqlParameters();
            sqlParams.Add(new SqlParameter("@Id", _customer.Id));
            await _updateCustomer.ExecuteNonQueryAsync(sqlParams.ToArray());
            //await RemoveCustomerDetails(_customer.Id);
            //await SaveCustomerPayment(new CustomerPayment { Id = customer.Id, PaymentDetails = customer.PaymentDetails });
            //await SaveCustomerPricelist(new CustomerPricelist { Id = customer.Id, PriceListDetails = customer.PriceListDetails });
            //await SaveCustomerProduct(new CustomerProduct { Id = customer.Id, ProductDetails = customer.ProductDetails });
            //await SaveCustomerShippingAddress(new CustomerShipTo { Id = customer.Id, ShipToDetails = customer.ShipTo });
        }

        private List<SqlParameter> GetSqlParameters()
        {
            var sqlParameters = new List<SqlParameter>();
            try
            {
                sqlParameters.Add(new SqlParameter("@ReferenceNo", _customer.ReferenceNo.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@FarmId", _customer.FarmId));
                sqlParameters.Add(new SqlParameter("@Name", _customer.Name.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@Address1", _customer.Address1.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@Address2", _customer.Address2.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@MobileNo", _customer.MobileNo.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@TelNo", _customer.TelNo.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@TIN", _customer.TIN.EmptyNull().Replace("'", "''")));
                sqlParameters.Add(new SqlParameter("@PaymentTypeId", _customer.PaymentTypeId));
                sqlParameters.Add(new SqlParameter("@PriceListId", _customer.PriceListId));
                sqlParameters.Add(new SqlParameter("@PersonnelId", _customer.PersonnelId));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return sqlParameters;


        }


        public async Task UpdateCustomerPayment(CustomerPayment customerPayment)
        {
            await RemoveCustomerPayment(customerPayment.Id);

            foreach (var payment in customerPayment.PaymentDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerPayment.Id),
                    new SqlParameter("@PaymentId", payment.Id)
                };
                await _insertCustomerPayment.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
        }

        public async Task UpdateCustomerPricelist(CustomerPricelist customerPricelist)
        {
            await RemoveCustomerPricelist(customerPricelist.Id);

            foreach (var priceList in customerPricelist.PriceListDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerPricelist.Id),
                    new SqlParameter("@PriceListId", priceList.Id)
                };
                await _insertCustomerPricelist.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
        }

        public async Task UpdateCustomerProduct(CustomerProduct customerProduct)
        {
            await RemoveCustomerProduct(customerProduct.Id);

            foreach (var product in customerProduct.ProductDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerProduct.Id),
                    new SqlParameter("@ProductId", product.Id)
                };
                await _insertCustomerProduct.ExecuteNonQueryAsync(sqlParameters.ToArray());
            }
        }

        public async Task UpdateCustomerShippingAddress(CustomerShipTo customerShipTo)
        {

            await RemoveCustomerShippingAddress(customerShipTo.Id);

            foreach (var shipTo in customerShipTo.ShipToDetails)
            {
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@CustomerId", customerShipTo.Id),
                    new SqlParameter("@Name", shipTo.Name),
                    new SqlParameter("@Address1", shipTo.Address1),
                    new SqlParameter("@Address2", shipTo.Address2),
                    new SqlParameter("@MobileNo", shipTo.MobileNo),
                    new SqlParameter("@TelNo", shipTo.TelNo)
                };
                decimal shipToId = 0;
                using (var connection = await DbHelper.GetOpenConnectionAsync())
                {
                    using (var reader = await connection.ExecuteReaderAsync(_insertCustomerShippingAddress, sqlParameters.ToArray()))
                    {
                        while (reader.Read())
                        {
                            shipToId = reader.GetAsync<decimal>("Id").Result;
                        }
                    }
                }
                shipTo.Id = Convert.ToInt64(shipToId);
            }
        }

        public async Task<IEnumerable<Payment>> GetPayments()
        {
            var list = new List<Payment>();
            list = await _spGetPayments.GetList<Payment>();
            return list;
        }
    }
}
