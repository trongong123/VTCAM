namespace EQX.Core.Recipe
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SinglePositionTeachingAttribute : Attribute
    {
        public string Motion { get; set; }
    }
}
