namespace EQX.Core.Recipe
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SingleRecipeDescriptionAttribute : Attribute
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public string DescriptionKey { get; set; }
        public string Unit { get; set; }
        public bool IsSpacer { get; set; } = false;
        public string Detail { get; set; }
        public string DetailKey { get; set; }
    }
}
