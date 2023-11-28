using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Yggdrasil.Data.Access;
using Yggdrasil.Data.Entity;
using YggdrasilApi.Services.Contracts;

namespace YggdrasilApi.Services {
    public class MovieService : IMovieService {
        private readonly YggdrasilDbContext _dbContext;

        public MovieService(YggdrasilDbContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task Add(Movie movie) {
            foreach (var item in movie.Actors) {
                var actor = _dbContext
                    .Actors.AsNoTracking()
                    .FirstOrDefault(x =>
                    x.FirstName == item.FirstName &&
                    x.LastName == item.LastName);

                if (actor != null) {
                    item.Id = actor.Id;
                    _dbContext.Actors.Attach(item);
                }
            }

            _dbContext.Movies.Add(movie);
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(int id) {
            throw new NotImplementedException();
        }

        public IEnumerable<Movie> GetAll() {
            return _dbContext.Movies
                .Include(x => x.Actors)
                .Include(x => x.Director);
        }
        public async Task<IEnumerable<Movie>> FindByCondition(Expression<Func<Movie, bool>> expression) {
            return await _dbContext.Movies
                .Include(x => x.Actors)
                .Include(x => x.Director)
                .Where(expression).ToListAsync();
        }
        public async Task<Movie> GetById(int id) {
            throw new NotImplementedException();
        }

        public void Edit(Movie movie) {
            throw new NotImplementedException();
        }
    }
}
