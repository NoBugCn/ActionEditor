using System.Reflection;

namespace NBC.ActionEditor
{
    public static class FieldInfoExtensions
    {
        public static string GetShowName(this FieldInfo field)
        {
            var name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(field.Name);

            var menuNameAttribute = field.GetCustomAttribute<MenuNameAttribute>();
            if (menuNameAttribute != null)
            {
                return menuNameAttribute.showName;
            }

            return name;
        }
    }
}