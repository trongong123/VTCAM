using System.Drawing;

namespace EQX.UI.Language
{
    public class AlertModel
    {
        #region Properties
        public int Id { get; set; }
        public string Message { get; set; }
        public string AlertOverviewSource { get; set; }
        public Rectangle AlertOverviewHighlightRectangle { get; set; }
        public string AlertDetailviewSource { get; set; }
        public Rectangle AlertDetailviewHighlightRectangle { get; set; }
        public List<string> TroubleshootingSteps { get; set; }
        public string IOName { get;set; }
        #endregion
    }
}
