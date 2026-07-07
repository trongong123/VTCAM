using EQX.Core.Common;

namespace EQX.Core.Test
{
    [TestClass]
    public class TestMultiThreadingHelpers
    {
        private enum ETest
        {
            None,
            Start,
            UserDefined,
            End,
        }

        [TestMethod]
        public void TestSafeSetValue()
        {
            int testObj = (int)ETest.None;
            MultiThreadingHelpers.SafeSetValue(ref testObj, ETest.UserDefined);
            Assert.AreEqual((int)ETest.UserDefined, testObj);
        }
    }
}