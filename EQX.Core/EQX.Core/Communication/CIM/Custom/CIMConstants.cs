using TOPENG_Device;

namespace EQX.Core.Communication.CIM
{
    public static class CIMConstants
    {
        public static readonly List<CIMCommand> FromCIMCommands = new List<CIMCommand>
        {
            CIMCommand.TerminalDisplay,
            CIMCommand.DatetimeSet,
            CIMCommand.OperatorCall,
            CIMCommand.Interlock,
            CIMCommand.FormattedProcessProgramSend2,
            CIMCommand.FormattedProcessProgramRequest,
            CIMCommand.CurrentEquipPPIDListRequest, // NO WORD
            CIMCommand.EquipConstantNameList, // NO WORD
            CIMCommand.MaterialInfoSend1,
            CIMCommand.EquipFunctionChangeCommand,
            CIMCommand.CurrentProcessControlDataReq,
            CIMCommand.ProcessControlInformSend1,
            CIMCommand.ProcessControlInformSend2,
            CIMCommand.ProcessControlInformSend3,
            CIMCommand.ProcessControlInformSend4,
        };
    }
}
