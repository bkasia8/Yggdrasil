using AutoFixture;
using AutoFixture.Dsl;
using Yggdrasil.Data.Access;
using Yggdrasil.Data.Entity;
using YggdrasilApi.Services;

namespace Yggdrasil.Tests;

public class MovieServiceTests
{
    private YggdrasilDbContext _dbContext;
    private readonly IPostprocessComposer<Movie> _movieBuilder;
    private readonly IPostprocessComposer<Actor> _actorBuilder;
    private readonly MovieService _movieService;
    public MovieServiceTests()
    {
        _dbContext = TestUtils.CreateInMemoryContext();
        var fixture = new Fixture();
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

        _actorBuilder = fixture.Build<Actor>();
        _movieBuilder = fixture.Build<Movie>();

        _movieService = new MovieService(_dbContext);
    }

    [Fact]
    public async Task Add_ShouldReturnFromDbAddedMovie()
    {
        var movieToAdd = _movieBuilder.Create();

        await _movieService.Add(movieToAdd);

        var movieAdded = _dbContext.Movies.First();
        Assert.NotNull(movieAdded);
    }
    [Fact]
    public async Task Add_ShouldReturnExpectedCountOfActorFromDb()
    {
        var movieToAdd = _movieBuilder.Create();
        int expectedCountOfActors = movieToAdd.Actors.Count;

        await _movieService.Add(movieToAdd);

        var movieAdded = _dbContext.Movies.First();
        Assert.Equal(expectedCountOfActors, movieAdded.Actors.Count);
    }
    [Fact]
    public async Task Add_ShouldSetExistingActorToMovie()
    {
        var actorToAdd = _actorBuilder.Without(x => x.Movies).Create();
        _dbContext.Actors.Add(actorToAdd);
        _dbContext.SaveChanges();

        var movieToAdd = _movieBuilder.Create();
        movieToAdd.Actors.Add(actorToAdd);

        await _movieService.Add(movieToAdd);

        var actorsOfMovieAdded = _dbContext.Movies.First().Actors;
        Assert.Contains(actorToAdd.Id, actorsOfMovieAdded.Select(x => x.Id));
    }
}