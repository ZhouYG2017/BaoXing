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
        public ICustomerRepository service { get; set; }

        public List<CustomerEntity> GetList()
        {
            return service.IQueryable().ToList();
        }

        public Response<LoginResponse> Login(ParamLogin paramLogin)
        {
            var customerEntity = service.FindEntity(x => x.F_CustomerName == paramLogin.CustomerName);

            if (customerEntity!=null)
            {
                string Password = Md5.md5(DESEncrypt.Encrypt(paramLogin.Password, customerEntity.F_UserSecretkey??"").ToLower(), 32).ToLower();
                if (customerEntity.F_password==Password)
                {
                    var payload = new Dictionary<string, object>();
                    payload.Add("F_Id", customerEntity.F_Id);
                    payload.Add("F_CustomerName", customerEntity.F_CustomerName);
                    return new Response<LoginResponse>()
                    {
                        Code = "200",
                        Message = "Success",
                        Data = new LoginResponse()
                        {
                            Id=customerEntity.F_Id,
                            CustomerName=customerEntity.F_CustomerName,
                            Token = JwtHelper.SetJwtEncode(payload)
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
        public Response AddCustomer(ParamAddCustomer paramAddCustomer)
        {
            Response response = new Response();
            CustomerEntity entity = new CustomerEntity() {
                F_CustomerName = paramAddCustomer.CustomerName,
                F_password = paramAddCustomer.Password
            };
            if (this.SubmitForm(entity, "")>0) {
                response = new Response()
                {
                    Code="200",
                    Message="新增成功"
                };
            }
            else
            {
                response = new Response()
                {
                    Code = "500",
                    Message = "新增错误"
                };
            }
            return response;
        }
        public CustomerEntity GetForm(string keyValue)
        {
            return service.FindEntity(keyValue);
        }

        public void Delete(CustomerEntity entity)
        {
            service.Delete(entity);
        }

        public int SubmitForm(CustomerEntity entity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.Modify(keyValue);
                return service.Update(entity);
            }
            else
            {
                entity.Create();
                entity.F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower();
                entity.F_password = Md5.md5(DESEncrypt.Encrypt(Md5.md5(entity.F_password, 32).ToLower(), entity.F_UserSecretkey).ToLower(), 32).ToLower();
                return service.Insert(entity);
            }
        }

    }
}