namespace EQX.Core.Recipe
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SingleRecipeMinMaxAttribute : Attribute
    {
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
