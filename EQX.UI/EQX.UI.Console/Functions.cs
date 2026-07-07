// See https://aka.ms/new-console-template for more information

public static class Functions
{
    public static string ListingBitmapImageResource()
    {
        string result = "";

        DirectoryInfo di = new DirectoryInfo(Path.Combine(@"..\..\..\..\EQX.UI\Resources\Images"));

        string template = @"            <BitmapImage x:Key=""{0}""
                         UriSource=""/EQX.UI;component/Resources/Images/{1}""/>";

        foreach (FileInfo fi in di.GetFiles("*", SearchOption.AllDirectories))
        {
            string fileName = fi.Name.Replace(fi.Extension, "");
            fileName = "image_" + fileName;

            string fileNameWithPath = fi.FullName.Split("Resources\\Images\\")[1].Replace('\\', '/');

            result += string.Format(template, fileName, fileNameWithPath) + Environment.NewLine;
        }

        return result;
    }
}