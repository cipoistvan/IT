

using ITAssets;

namespace ITTest
{
    [TestClass]
    public sealed class ITValidatorsTest
    {
        [TestMethod]
        public void ValidEmailCheck()
        {
            string emailbad = "aaa";
            bool expected = false;
            bool actual = ITValidators.ValidEmail(emailbad);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void ValidEmailCheck2()
        {
            string emailbad = "a@b.hu";
            bool expected = true;
            bool actual = ITValidators.ValidEmail(emailbad);

            Assert.AreEqual(expected, actual);

        }
        [TestMethod]
        public void ValidEmailCheck3()
        {
            string emailbad = "a@ b.hu";
            bool expected = false;
            bool actual = ITValidators.ValidEmail(emailbad);

            Assert.AreEqual(expected, actual);

        }



    }
}
