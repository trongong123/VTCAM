namespace EQX.Core.Recipe
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CIMParameterAddressAttribute : Attribute
    {
        public static List<int> CimParaterIndexDouplicateCheckList = new List<int>();

        public int Index { get; set; }
        public string Name { get; set; }

        public CIMParameterAddressAttribute(int index)
            : this(index, "")
        {
        }

        public CIMParameterAddressAttribute(int index, string name)
        {
            if (index <= 0)
            {
                throw new Exception($"PARAMETER index must >= 1");
            }

            CIMParameterAddressAttribute.CimParaterIndexDouplicateCheckList.Add(index);

            Index = index;
            Name = name;
        }
    }
}
