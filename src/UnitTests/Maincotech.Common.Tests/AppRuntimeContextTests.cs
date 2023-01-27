namespace Maincotech.Common.Tests
{
    [TestClass]
    public class AppRuntimeContextTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var path = AppRuntimeContext.ExecutingPath;
            Assert.IsTrue(Directory.Exists(path));
        }
    }
}