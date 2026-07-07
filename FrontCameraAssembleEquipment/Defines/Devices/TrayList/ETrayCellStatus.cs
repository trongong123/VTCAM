namespace FrontCameraAssembleEquipment.Defines
{
    public enum ETrayCellStatus
    {
        Ready = 0,
        /// <summary>
        /// There is no material in the cell
        /// </summary>
        Skip,
        /// <summary>
        /// To-Pick cell
        /// </summary>
        Working,
        /// <summary>
        /// Cell may placed or picked, need result update (auto or manual)
        /// </summary>       
        Done,
        /// <summary>
        /// Pick done with fail
        /// </summary>
        PickFail,
    }
}
