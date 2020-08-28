using System.ComponentModel.DataAnnotations;

namespace System
{

    public static class EnumExtension
    {
        public static string DisplayName(this Enum value)
        {
            var enumType = value.GetType();
            var enumValue = Enum.GetName(enumType, value);
            var member = enumType.GetMember(enumValue)[0];

            var attrs = member.GetCustomAttributes(typeof(DisplayAttribute), false);
            var outString = member.Name;
            if (attrs.Length > 0)
            {
                outString = ((DisplayAttribute)attrs[0]).ResourceType != null ? ((DisplayAttribute)attrs[0]).GetDescription() : ((DisplayAttribute)attrs[0]).Name;
            }
            return outString;
        }
    }
}