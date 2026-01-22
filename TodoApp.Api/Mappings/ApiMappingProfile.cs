using AutoMapper;
using TodoApp.Api.DTOs.Responses;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Api.Mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<TodoItemView, TodoItemResponseDto>();
        CreateMap<ProgressionView, ProgressionResponseDto>();
    }
}
