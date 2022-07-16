using AutoMapper;
using Holidays.Core.Monads;

namespace Holidays.WebAPI.Mapping;

internal class MaybeConverter<TSource, TDestination> : ITypeConverter<Maybe<TSource>, Maybe<TDestination>>
    where TSource : notnull
    where TDestination : notnull
{
    public Maybe<TDestination> Convert(
        Maybe<TSource> source,
        Maybe<TDestination> destination,
        ResolutionContext context)
    {
        return source.Match(
            Maybe<TDestination>.None,
            some =>
            {
                var mapped = context.Mapper.Map<TDestination>(some);
                return Maybe<TDestination>.Some(mapped);
            });
    }
}
