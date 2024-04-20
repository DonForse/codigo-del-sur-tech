using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodigoDelSurApi.Infrastructure.DataEntities;

public class UserPreferences
{
    [Key]
    public int UserId { get; set; }

    public int Genre { get; set; }
    public string Language { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

}