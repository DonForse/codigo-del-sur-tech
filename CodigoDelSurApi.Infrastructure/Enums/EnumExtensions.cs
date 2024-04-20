using System.ComponentModel;
using System.Reflection;

namespace CodigoDelSurApi.Infrastructure.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this LanguageEnum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }

    public static class EnumHelper {
        public static LanguageEnum ParseLanguage(string description)
        {
            foreach (LanguageEnum lang in Enum.GetValues(typeof(LanguageEnum)))
            {
                if (lang.GetDescription() == description)
                {
                    return lang;
                }
            }
            throw new ArgumentException("Invalid language code");
        }
    }
}