using System.Data.Entity.ModelConfiguration;

namespace FinanceSaldo.Model
{
    public class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            Property(p => p.Name)
                .IsRequired();
            HasMany(pt => pt.Invoice)
                .WithRequired(p => p.Company)
                .WillCascadeOnDelete(true);
        }
    }
}
