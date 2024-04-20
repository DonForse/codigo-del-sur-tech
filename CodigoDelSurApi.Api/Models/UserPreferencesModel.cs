using CodigoDelSurApi.Infrastructure.Enums;

namespace CodigoDelSurApi.Api.Models
{
    public class UserPreferencesModel
    {
        public LanguageEnum Language { get; set; }
        public GenreEnum Genre { get; set; }
    }
}
