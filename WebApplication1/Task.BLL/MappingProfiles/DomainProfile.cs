using AutoMapper;
using BLL.DTOs;
using DAL.Entities;

namespace BLL.MappingProfiles;

public class DomainProfile : Profile
{
    public DomainProfile()
    {
        // User Mapping
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>();

        // Todo Mapping  
        CreateMap<Todo, TodoDto>();
        CreateMap<CreateTodoDto, Todo>();

        // Comment Mapping
        CreateMap<Comment, CommentItemDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : ""));

        // Like Mapping
        CreateMap<Like, LikeItemDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : ""));

        // Post Mapping
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : ""))
            .ForMember(dest => dest.Comments,
                opt => opt.MapFrom(src => src.Comments))
            .ForMember(dest => dest.Likes,
                opt => opt.MapFrom(src => src.Likes));
        CreateMap<CreatePostDto, Post>();

        // Notification Mapping
        CreateMap<Notification, NotificationDto>();
        CreateMap<NotificationDto, Notification>();
    }
}