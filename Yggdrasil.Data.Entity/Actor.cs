using Yggdrasil.Data.Entity.Enums;

namespace Yggdrasil.Data.Entity
{
    public class Actor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<Movie>? Movies { get; set; }
        
    }
}
