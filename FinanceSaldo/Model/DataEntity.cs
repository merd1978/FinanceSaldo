using System.Data.Entity;

namespace FinanceSaldo.Model
{
    public class DataEntity : DbContext
    {
        public DataEntity()
            : base("name=CompanyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //throw new UnintentionalCodeFirstException();
            modelBuilder.Configurations.Add(new CompanyMap());
        }
    
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
    }
}
