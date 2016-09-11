using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
