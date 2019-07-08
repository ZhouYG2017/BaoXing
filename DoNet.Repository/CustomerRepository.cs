using DoNet.Data;
using DoNet.Domain.Entity;
using DoNet.Domain.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoNet.Repository
{
    /// <summary>
    /// Tbl_CustomerRepository
    /// </summary>	
    public class CustomerRepository : RepositoryBase<CustomerEntity>, ICustomerRepository
    {

    }
}