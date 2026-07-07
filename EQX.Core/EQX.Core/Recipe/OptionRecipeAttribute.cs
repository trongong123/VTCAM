namespace EQX.Core.Recipe
{
    /// <summary>
    /// Đánh dấu property hiển thị dạng tùy chọn Use/NotUse (0 = NotUse, 1 = Use).
    /// Dùng cùng <see cref="SingleRecipeDescriptionAttribute"/> trên cùng property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionRecipeAttribute : Attribute
    {
    }
}
