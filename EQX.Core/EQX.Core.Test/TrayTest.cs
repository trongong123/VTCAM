using EQX.Core.Units;

namespace EQX.Core.Test
{
    [TestClass]
    public class TrayTest
    {
        public enum ETrayCellStatus
        {
            /// <summary>
            /// Ready to work
            /// </summary>
            Ready = 0,

            /// <summary>
            /// There is no material in the cell
            /// </summary>
            Skip,
            /// <summary>
            /// To-Pick cell
            /// </summary>
            Picking,
            /// <summary>
            /// Pick done, material have been removed successed
            /// </summary>
            PickDone,
            /// <summary>
            /// Pick done with fail
            /// </summary>
            NGPickFail,
            /// <summary>
            /// Vision inspect with fail
            /// </summary>
            NGVision
        }

        [TestMethod]
        public void TestGetTrayRowColumnByStatus()
        {
            var tray = new Tray<ETrayCellStatus>("Test Tray");
            tray.Rows = 5;
            tray.Columns = 4;
            tray.GenerateCells();

            Assert.AreEqual(1, tray.GetFirstRow(ETrayCellStatus.Ready));
            Assert.AreEqual(1, tray.GetFirstColumn(ETrayCellStatus.Ready));

            Assert.AreEqual(-1, tray.GetFirstRow(ETrayCellStatus.NGVision));
            Assert.AreEqual(-1, tray.GetFirstColumn(ETrayCellStatus.NGVision));

            tray[7] = ETrayCellStatus.NGVision;

            Assert.AreEqual(2, tray.GetFirstRow(ETrayCellStatus.NGVision));
            Assert.AreEqual(3, tray.GetFirstColumn(ETrayCellStatus.NGVision));

            Assert.IsFalse(tray[7].Equals(ETrayCellStatus.Ready));
            Assert.IsTrue(tray[7].Equals(ETrayCellStatus.NGVision));
        }

        [TestMethod]
        public void TestTrayRowColumn()
        {
            var tray = new Tray<ETrayCellStatus>("Test Tray");
            tray.Rows = 5;
            tray.Columns = 4;
            tray.Orientation = ETrayOrientation.TopLeft;
            tray.GenerateCells();

            Assert.AreEqual(1, tray.GetRow(1));
            Assert.AreEqual(1, tray.GetColumn(1));

            Assert.AreEqual(1, tray.GetRow(4));
            Assert.AreEqual(4, tray.GetColumn(4));

            Assert.AreEqual(3, tray.GetRow(11));
            Assert.AreEqual(3, tray.GetColumn(11));

            Assert.AreEqual(5, tray.GetRow(18));
            Assert.AreEqual(2, tray.GetColumn(18));

            Assert.AreEqual(5, tray.GetRow(20));
            Assert.AreEqual(4, tray.GetColumn(20));



            tray.Orientation = ETrayOrientation.TopRight;
            tray.GenerateCells();

            Assert.AreEqual(1, tray.GetRow(1));
            Assert.AreEqual(4, tray.GetColumn(1));

            Assert.AreEqual(1, tray.GetRow(4));
            Assert.AreEqual(1, tray.GetColumn(4));

            Assert.AreEqual(3, tray.GetRow(11));
            Assert.AreEqual(2, tray.GetColumn(11));

            Assert.AreEqual(5, tray.GetRow(18));
            Assert.AreEqual(3, tray.GetColumn(18));

            Assert.AreEqual(5, tray.GetRow(20));
            Assert.AreEqual(1, tray.GetColumn(20));



            tray.Orientation = ETrayOrientation.BottomLeft;
            tray.GenerateCells();

            Assert.AreEqual(5, tray.GetRow(1));
            Assert.AreEqual(1, tray.GetColumn(1));

            Assert.AreEqual(5, tray.GetRow(4));
            Assert.AreEqual(4, tray.GetColumn(4));

            Assert.AreEqual(3, tray.GetRow(11));
            Assert.AreEqual(3, tray.GetColumn(11));

            Assert.AreEqual(1, tray.GetRow(18));
            Assert.AreEqual(2, tray.GetColumn(18));

            Assert.AreEqual(1, tray.GetRow(20));
            Assert.AreEqual(4, tray.GetColumn(20));



            tray.Orientation = ETrayOrientation.BottomRight;
            tray.GenerateCells();

            Assert.AreEqual(5, tray.GetRow(1));
            Assert.AreEqual(4, tray.GetColumn(1));

            Assert.AreEqual(5, tray.GetRow(4));
            Assert.AreEqual(1, tray.GetColumn(4));

            Assert.AreEqual(3, tray.GetRow(11));
            Assert.AreEqual(2, tray.GetColumn(11));

            Assert.AreEqual(1, tray.GetRow(18));
            Assert.AreEqual(3, tray.GetColumn(18));

            Assert.AreEqual(1, tray.GetRow(20));
            Assert.AreEqual(1, tray.GetColumn(20));
        }
    }
}
