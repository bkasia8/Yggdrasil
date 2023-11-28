using System.Linq.Expressions;
using Yggdrasil.Data.Entity;

namespace YggdrasilApi.Services.Contracts
{
    public interface IMovieService
    {
        IEnumerable<Movie> GetAll();
        Task<Movie> GetById(int id);
        Task Add(Movie movie);
        void Edit(Movie movie);
        void Delete(int id);
        Task<IEnumerable<Movie>> FindByCondition(Expression<Func<Movie, bool>> expression);
    }
}
