using AutoMapper;
using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Application.Mappings;

public class TodoItemProfile : Profile
{
    public TodoItemProfile()
    {
        // Domain to DTO mappings (if needed in future)
        CreateMap<TodoItem, TodoItemDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Progressions, opt => opt.MapFrom(src => src.Progressions));

        CreateMap<Progression, ProgressionDto>()
            .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
            .ForMember(dest => dest.Percent, opt => opt.MapFrom(src => src.Percent));
    }
}

// DTOs for future use
public record TodoItemDto(int Id, string Title, string Description, string Category, List<ProgressionDto> Progressions);
public record ProgressionDto(DateTime DateTime, decimal Percent);
