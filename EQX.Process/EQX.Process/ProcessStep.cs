using EQX.Core.Sequence;

namespace EQX.Process
{
    public class ProcessStep : IProcessStep
    {
        public event EventHandler? StepChangedHandler;

        public int OriginStep
        {
            get => originStep;
            set
            {
                if (originStep == value) return;

                originStep = value;
                StepChangedHandler?.Invoke(this, EventArgs.Empty);
            }
        }
        public int RunStep
        {
            get => runStep;
            set
            {
                if (runStep == value) return;

                runStep = value;
                StepChangedHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int ToRunStep
        {
            get 
            {
                return toRunStep; 
            }
            set 
            {
                if(toRunStep == value) return;

                toRunStep = value;
                StepChangedHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        public int PreProcessStep
        {
            get
            {
                return preProcessStep;
            }
            set
            {
                if (preProcessStep == value) return;

                preProcessStep = value;
                StepChangedHandler?.Invoke(this, EventArgs.Empty);
            }
        }

        internal void ClearSteps()
        {
            OriginStep = 0;
            RunStep = 0;
            ToRunStep = 0;

            PreProcessStep = 0;
        }

        #region Privates
        private int originStep;
        private int runStep;
        private int toRunStep;
        private int preProcessStep;
        #endregion
    }
}
