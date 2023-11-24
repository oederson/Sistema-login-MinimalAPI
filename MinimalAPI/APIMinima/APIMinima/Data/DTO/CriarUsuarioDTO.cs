using System.ComponentModel.DataAnnotations;

namespace APIMinima.Data.DTO;

public class CriarUsuarioDTO
{
    
    public string Username { get; set; }
    
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Compare("Password")]
    public string RePassword { get; set; }
}
