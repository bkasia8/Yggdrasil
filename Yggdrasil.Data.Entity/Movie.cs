using Yggdrasil.Data.Entity.Enums;

namespace Yggdrasil.Data.Entity
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Genre Genre { get; set; }
        public int DirectorId { get; set; }
        public required Director Director { get; set; }
        public required ICollection<Actor> Actors { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public long BoxOffice { get; set; }
        public TimeSpan Runtime { get; set; }
    }
}
