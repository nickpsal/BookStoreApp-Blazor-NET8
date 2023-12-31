﻿using AutoMapper;
using BookStoreApp.API.Models;
using BookStoreApp.API.Models.Dtos;
using BookStoreApp.API.Models.Dtos.Author;
using BookStoreApp.API.Models.Dtos.Book;
using BookStoreApp.API.Models.Dtos.User;
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
            CreateMap<ReadOnlyBookDTO, Book>().ReverseMap();
            //map with include author name
            CreateMap<Book, ReadOnlyBookDTO>()
                .ForMember(q => q.AuthorName, d => d.MapFrom(map => $"{map.Author!.FirstName} {map.Author.LastName}")).ReverseMap();
            CreateMap<CreateBookDTO, Book>().ReverseMap();
            CreateMap<UpdateBookDTO, Book>().ReverseMap();
            CreateMap<DetailsBookDTO, Book>().ReverseMap();
            //map with include author name
            CreateMap<Book, DetailsBookDTO>()
                .ForMember(q => q.AuthorName, d => d.MapFrom(map => $"{map.Author!.FirstName} {map.Author.LastName}")).ReverseMap();
            CreateMap<UserDTO, ApiUser>().ReverseMap();
        }
    }
}
