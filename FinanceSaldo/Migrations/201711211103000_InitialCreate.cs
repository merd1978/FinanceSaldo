namespace FinanceSaldo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Saldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        InvoiceId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Date = c.DateTime(nullable: false),
                        Debit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DebitCash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreditCash = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InvoiceId)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Invoices", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Invoices", new[] { "CompanyId" });
            DropTable("dbo.Invoices");
            DropTable("dbo.Companies");
        }
    }
}
