using EQX.Core.Sequence;
using System.Threading;

namespace EQX.Process.UnitTest
{
    public class ProcessBaseTest
    {
        private enum ERunMode
        {
            Pick,
            Place,
            Vision
        }

        [Fact]
        public void TestMultiStart()
        {
            IProcess<ERunMode> TestProcess = new ProcessBase<ERunMode>()
            {
                Id = 1,
                Name = "Test Proc."
            };

            Assert.Equal("Test Proc.", TestProcess.Name);

            try
            {
                TestProcess.Start();
                TestProcess.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Start failed with Exception \"{ex.Message}\"");
            }
        }

        [Fact]
        public void TestMultiStartStop()
        {
            IProcess<ERunMode> TestProcess = new ProcessBase<ERunMode>();

            try
            {
                TestProcess.Start();
                TestProcess.Start();
                TestProcess.Stop();
                TestProcess.Stop();
                TestProcess.Stop();
                TestProcess.Start();
                TestProcess.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail($"Start failed with Exception \"{ex.Message}\"");
            }
        }

        [Fact]
        public void TestProcessRelationship()
        {
            IProcess<ERunMode> ProcessParent = new ProcessBase<ERunMode>();
            IProcess<ERunMode> ProcessChild = new ProcessBase<ERunMode>();

            ProcessParent.AddChild(ProcessChild);

            ProcessParent.Start();
            ProcessChild.Start();

            Assert.Null(ProcessParent.Parent);
            Assert.NotNull(ProcessChild.Parent);

            ProcessParent.ProcessMode = EProcessMode.Origin;
            Thread.Sleep(10);  // Sleep before ProcessChild.ProcessMode updated
            Assert.Equal(EProcessMode.Origin, ProcessChild.ProcessMode);

            ProcessParent.ProcessMode = EProcessMode.ToOrigin;
            Thread.Sleep(10);  // Sleep before ProcessChild.ProcessMode updated
            Assert.Equal(EProcessMode.ToOrigin, ProcessChild.ProcessMode);
        }
    }
}