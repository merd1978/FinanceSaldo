namespace FinanceSaldo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompnayNameRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Companies", "Name", c => c.String());
        }
    }
}
