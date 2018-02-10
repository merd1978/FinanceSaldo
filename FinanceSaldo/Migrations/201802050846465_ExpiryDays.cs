namespace FinanceSaldo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpiryDays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invoices", "ExpiryDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoices", "ExpiryDays");
        }
    }
}
