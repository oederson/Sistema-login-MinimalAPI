using AutoMapper;
using MinimalApiLogin.Data.DTO;
using MinimalApiLogin.Models;

namespace MinimalApiLogin.Profiles;

public class UsuarioProfile : Profile 
{
    public UsuarioProfile()
    {
        CreateMap<CriarUsuarioDTO, Usuario>();
        CreateMap<LoginUsuarioDTO, Usuario>();
    }
}
