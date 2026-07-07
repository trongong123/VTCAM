namespace EQX.Core.Sequence
{
    public enum EProcessStatus
    {
        None = 0,
        ToWarningDone = 1,
        //WarningDone,
        ToAlarmDone = 3,
        //AlarmDone,
        ToOriginDone = 5,
        OriginDone,
        ToStopDone = 7,
        //StopDone,
        ToRunDone = 9,
    }
}
