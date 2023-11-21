using System.ComponentModel.DataAnnotations;

namespace APIMinima.Data.DTO;

public class AtualizarUsuarioDTO
{
    [Required]
    public string Username { get; set; }
    public string Password { get; set; }
}
