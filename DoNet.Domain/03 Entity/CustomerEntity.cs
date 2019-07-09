using System;
namespace DoNet.Domain.Entity
{
    /// <summary>
    /// Tbl_CustomerEntity
    /// </summary>	
    public class CustomerEntity : IEntity<CustomerEntity>, ICreationAudited, IDeleteAudited, IModificationAudited
    {
        public string F_Id { get; set; }
        public string F_CustomerName { get; set; }
        public string F_password { get; set; }
        public string F_CreatorUserId { get; set; }
        public string F_UserSecretkey { get; set; }

        public DateTime? F_CreatorTime { get; set; }
        public bool? F_DeleteMark { get; set; }
        public string F_DeleteUserId { get; set; }
        public DateTime? F_DeleteTime { get; set; }
        public string F_LastModifyUserId { get; set; }
        public DateTime? F_LastModifyTime { get; set; }

    }
}