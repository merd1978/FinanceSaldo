namespace FinanceSaldo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeDateTime2DateTime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Invoices", "Date", c => c.DateTime(nullable: false));
        }
    }
}
