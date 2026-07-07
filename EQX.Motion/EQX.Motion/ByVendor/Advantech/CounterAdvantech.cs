using Automation.BDaq;

namespace EQX.Motion.ByVendor
{
    public class CounterAdvantech : MotionBase
    {
        private UdCounterCtrl udCounterCtrl;
        private readonly MotionParameter parameter;
        private int channel;
        public override bool IsConnected { get; protected set; }

        public CounterAdvantech(int id, string name, MotionParameter parameter, int channel)
            : base(id, name, parameter)
        {
            this.parameter = parameter;
            this.channel = channel;
        }

        protected override bool ActualConnect()
        {
            try
            {
                udCounterCtrl = new UdCounterCtrl();
                udCounterCtrl.SelectedDevice = new DeviceInformation(Id);

                IsConnected = udCounterCtrl.Initialized;


                return IsConnected;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        protected override bool ActualInitialization()
        {
            if (IsConnected == false) return false;

            udCounterCtrl.ChannelStart = channel;
            UdChannel[] udChannel = udCounterCtrl.Channels;
            udChannel[channel].ResetTimesByIndex = 0;
            udChannel[channel].InitialValue = 0;
            udCounterCtrl.Enabled = true;

            return true;
        }

        protected override bool ActualDisconnect()
        {
            try
            {
                udCounterCtrl.Dispose();
                IsConnected = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool ActualSearchOrigin()
        {
            udCounterCtrl.ValueReset();
            return true;
        }
        public override bool ClearPosition()
        {
            udCounterCtrl.ValueReset();
            return true;
        }

        protected override void UpdateAxisStatus()
        {
            int counter = 0;
            if (udCounterCtrl == null) return;
            udCounterCtrl.Read(out counter);
            ((MotionStatus)Status).ActualPosition = PulseToMM(counter);
        }
    }
}
