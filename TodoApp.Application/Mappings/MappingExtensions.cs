using AutoMapper;

namespace TodoApp.Application.Mappings;

public static class MappingExtensions
{
    public static TDestination MapTo<TDestination>(this object source, IMapper mapper)
    {
        return mapper.Map<TDestination>(source);
    }

    public static TDestination MapTo<TSource, TDestination>(this TSource source, IMapper mapper)
    {
        return mapper.Map<TSource, TDestination>(source);
    }

    public static List<TDestination> MapToList<TDestination>(this IEnumerable<object> source, IMapper mapper)
    {
        return mapper.Map<List<TDestination>>(source);
    }
}
