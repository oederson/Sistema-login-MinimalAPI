using APIMinima.Data.DTO;
using APIMinima.Models;
using AutoMapper;

namespace APIMinima.Profiles;

public class UsuarioProfile : Profile 
{
    public UsuarioProfile()
    {
        CreateMap<CriarUsuarioDTO, Usuario>();
        CreateMap<LoginUsuarioDTO, Usuario>();
    }
}
