using DoNet.Domain.Entity;
using System.Data.Entity.ModelConfiguration;
namespace DoNet.Mapping
{
    /// <summary>
    /// Tbl_CustomerMap
    /// </summary>	
    public class CustomerMap : EntityTypeConfiguration<CustomerEntity>
    {
        public CustomerMap()
        {
            this.ToTable("Tbl_Customer");
            this.HasKey(t => t.F_Id);
        }
    }
}