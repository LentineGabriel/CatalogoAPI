using AutoMapper;
using CatagoloAPI.Models;

namespace CatagoloAPI.DTO.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Produto, ProdutoDTO>().ReverseMap();
        CreateMap<Categoria, CategoriaDTO>().ReverseMap();
    }
}