using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoDelSurApi.Infrastructure.DataEntities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public UserPreferences Preferences { get; internal set; }
    }
}
