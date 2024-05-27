using AutoMapper;
using Lemondo.Libary.API.Modules.Auth.Models;
using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;

namespace Lemondo.Libary.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Book profiles
        CreateMap<Book, BookReadDto>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors));
        CreateMap<Book, BookForAuthorCreateDto>();
        CreateMap<Book, BookForAuthorReadDto>();

        CreateMap<BookCreateDto, Book>()
            .ForMember(dest => dest.Authors, opt => opt.Ignore());
        CreateMap<BookUpdateDto, Book>();
        CreateMap<BookForAuthorCreateDto, Book>();
        
        // Author profiles
        CreateMap<Author, AuthorReadDto>()
            .ForMember(dest => dest.Books, opt => opt.MapFrom(src => src.Books));
        CreateMap<Author, AuthorForBookReadDto>();
        CreateMap<Author, AuthorForBookCreateDto>();

        CreateMap<AuthorCreateDto, Author>()
            .ForMember(dest => dest.Books, opt => opt.Ignore());
        CreateMap<AuthorUpdateDto, Author>();
        CreateMap<AuthorForBookCreateDto, Author>();

        // User profiles
        CreateMap<UserDto, User>();
    }
}
