using FinanceSaldo.Design;
using FinanceSaldo.Model;
using FinanceSaldo.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestFinanceSaldo
{
    [TestClass]
    public class UnitTestIvoiceViewModel
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Arange
            var testDataService = new DesignDataService();
            var company = new Company() {Name = "test"};
            var invoice = new Invoice();
            var invoiceViewModel = new InvoiceViewModel(testDataService, company);

            //Act
            invoice.Debit = 120;
            company.Invoice.Add(invoice);

            //Assert
            Assert.AreEqual(120, company.TotalSaldo);
        }
    }
}
