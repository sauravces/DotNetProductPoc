using AutoMapper;
using ProductPOC.Dto;
using ProductPOC.Models;
namespace ProductPOC.Mappings
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Product,ProductDto>().ReverseMap();
        }

    }
}
