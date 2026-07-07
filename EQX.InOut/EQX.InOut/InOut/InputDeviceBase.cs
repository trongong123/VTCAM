using EQX.Core.InOut;

namespace EQX.InOut
{
    /// <summary>
    /// Passing the <typeparamref name="TInputs"/> enum as IO List for the Input Device
    /// </summary>
    /// <typeparam name="TInputs"></typeparam>
    public class InputDeviceBase<TInputs> : IDInputDevice<TInputs> where TInputs : Enum
    {
        #region Properties
        public List<IDInput> Inputs { get; private set; }
        public int Id { get; init; }
        public string Name { get; init; }
        public virtual bool IsConnected { get; protected set; }

        public bool this[int index] => GetInput(index % MaxPin);
        //public IDInput this[Enum key]
        //{
        //    get
        //    {
        //        if (typeof(TInputs) != key.GetType())
        //        {
        //            throw new ArgumentException($"Key type must be of type {typeof(TInputs).Name}");
        //        }

        //        return this[(TInputs)key];
        //    }
        //}

        public IDInput this[TInputs key]
        {
            get
            {
                return Inputs.First(i => i.Id == Convert.ToInt32(key));
            }
        }

        public int MaxPin { get; init; } = 32;

        public int SimulationOffset { get; set; } = 32;
        #endregion

        #region Constructor(s)
        public InputDeviceBase()
        {
            Name ??= GetType().Name;
            Inputs = new List<IDInput>();
        }
        #endregion

        #region Public methods
        public bool Initialize()
        {
            var inputList = Enum.GetNames(typeof(TInputs)).ToList();
            var inputIndex = (int[])Enum.GetValues(typeof(TInputs));

            for (int i = 0; i < MaxPin; i++)
            {
                if (i >= inputList.Count) break;

                Inputs.Add(new DInput(inputIndex[i], inputList[i], this)
                {
                    SimulationOffset = this.SimulationOffset,
                });
            }

            return true;
        }

        public void InverseStatus(IList<IDInput> inputs)
        {
            foreach (var input in inputs)
            {
                InverseStatus(input);
            }
        }

        public virtual void InverseStatus(IDInput input) { }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }
        #endregion

        protected virtual bool ActualGetInput(int index)
        {
            return true;
        }

        private bool GetInput(int index)
        {
            if (IsConnected == false) return false;

            return ActualGetInput(index);
        }
    }
}