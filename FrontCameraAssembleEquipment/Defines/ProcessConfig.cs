namespace FrontCameraAssembleEquipment.Defines
{
    public  class ProcessConfig
    {
        public  bool IsTwoConveyor { get; set; } = true;
        public  EMachineType MachineType => IsTwoConveyor ? EMachineType.TwoConveyor : EMachineType.OneConveyor;
    }
}
