using Yggdrasil.Data.Entity.Enums;

namespace YggdrasilApi.Dto
{
    public record MovieDto(string Title, Genre Genre, DirectorDto Director, DateOnly ReleaseDare, long BoxOffice, TimeSpan Runtime, IList<ActorDto> Actors);
}
