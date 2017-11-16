using System.Data.Entity.ModelConfiguration;

namespace FinanceSaldo.Model
{
    class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            Property(p => p.Name)
                .IsRequired();
        }
    }
}
