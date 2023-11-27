using AutoMapper;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Dtos;
using BookStoreApp.API.Models.Dtos.Author;
using Microsoft.Build.Framework.Profiler;

namespace BookStoreApp.API.Config
{
    public class MapperConfig: Profile
    {
        public MapperConfig()
        {
            CreateMap<CreateAuthorDTO, Author>().ReverseMap();
            CreateMap<ReadOnlyAuthorDTO, Author>().ReverseMap();
            CreateMap<UpdateAuthorDTO, Author>().ReverseMap();
        }
    }
}
