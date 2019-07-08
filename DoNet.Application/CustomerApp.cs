using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoNet.Code;
using DoNet.Domain.Entity;
using DoNet.Domain.IRepository;
using DoNet.Domain.ViewModel;
using DoNet.Repository;
namespace DoNet.Application
{
    /// <summary>
    /// Tbl_CustomerApp
    /// </summary>	
    public class CustomerApp
    {
        private ICustomerRepository service =new CustomerRepository();

        public List<CustomerEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public Response<LoginResponse> Login(ParamLogin paramLogin)
        {
            var customerEntity = service.FindEntity(x => x.F_CustomerName == paramLogin.CustomerName);

            if (customerEntity!=null)
            {
                string Password = Md5.md5(DESEncrypt.Encrypt(paramLogin.Password, "").ToLower(), 32).ToLower();
                if (customerEntity.F_password==Password)
                {
                    return new Response<LoginResponse>()
                    {
                        Code = "200",
                        Message = "Success",
                        Data = new LoginResponse()
                        {
                            Id=customerEntity.F_Id,
                            CustomerName=customerEntity.F_CustomerName,
                            Token = ""
                        }
                    };
                }
                else
                {
                    throw new Exception("用户名或密码错误");
                }
            }
            else
            {
                throw new Exception("帐户不存在");
            }

        }
        public CustomerEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public void Delete(CustomerEntity entity)
        {
            service.Delete(entity);
        }

        public void SubmitForm(CustomerEntity entity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                service.Update(entity);
            }
            else
            {
                entity.Create();
                service.Insert(entity);
            }
        }

    }
}