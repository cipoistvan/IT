

using ITAssets;
using System.Globalization;
using System.Net.Quic;

namespace ITTest
{
    [TestClass]
    public sealed class ITValidatorsTest
    {
        [TestMethod]
        [DataRow("aaa",false)]
        [DataRow("a@b.hu", true)]
        [DataRow("a@ b.hu", false)]
        [DataRow("a@b.co.hu", true)]

        public void ValidEmailCheck(string email, bool expected)
        {

            bool actual = ITValidators.ValidEmail(email);

            Assert.AreEqual(expected, actual);

        }


        [TestMethod]
        [DataRow("2025.03.26", true)]
        [DataRow("1990.02.04", false)]
        [DataRow("2026.01.01", false)]
        [DataRow("2000.01.01", true)]

        public void PurchaseYearTest(string date, bool expected)
        {
            DateTime? realdate = DateTime.Parse(date,new CultureInfo("hu-HU"));

            bool actual = ITValidators.ValidatePurchaseYear(realdate);

            Assert.AreEqual(expected, actual);

        }


        [TestMethod]
        [DataRow(5, true)]
        [DataRow(1, true)]
        [DataRow(100, true)]
        [DataRow(-1, false)]
        [DataRow(0, false)]
        [DataRow(101, false)]

        public void QtyTest(int qty, bool expected)
        {
            Assert.AreEqual(expected, ITValidators.ValidateQty(qty));
        }










    }
}
