using DoNet.Data;
using DoNet.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Domain.IRepository
{
    /// <summary>
    /// Tbl_CustomerRepository
    /// </summary>	
    public interface ICustomerRepository : IRepositoryBase<CustomerEntity>
    {

    }
}