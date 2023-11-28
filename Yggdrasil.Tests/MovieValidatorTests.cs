using AutoFixture;
using AutoFixture.Dsl;
using YggdrasilApi.Dto;
using YggdrasilApi.Validators;

namespace Yggdrasil.Tests {
    public class MovieValidatorTests {

        private readonly MovieValidator _validator;
        private readonly IPostprocessComposer<MovieDto> _movieDtoComposer;
        private readonly IPostprocessComposer<ActorDto> _actorDtoComposer;
        public MovieValidatorTests() {
            var fixture = new Fixture();
            fixture.Customize<DateOnly>(composer =>
            composer.FromFactory<DateTime>(DateOnly.FromDateTime));

            _actorDtoComposer = fixture.Build<ActorDto>();
            _movieDtoComposer = fixture.Build<MovieDto>();
            _validator = new MovieValidator();
        }
        [Fact]
        public void ShouldReturnIsValidFalseWhenTitleIsNull() {
            var movie = _movieDtoComposer.With(x => x.Title, string.Empty).Create();

            var result = _validator.Validate(movie);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShouldReturnIsValidFalseWhenAssignActorsLessThenTwo() {
            var actors = _actorDtoComposer.CreateMany(1).ToList();
            var movie = _movieDtoComposer.With(x => x.Actors, actors).Create();

            var result = _validator.Validate(movie);

            Assert.False(result.IsValid);
        }
        [Fact]
        public void ShouldReturnExpectedErrorMessageWhenAssignActorsLessThenTwo() {
            var expectedErrorMessage = "Movie must contains at least 2 actors";
            var actors = _actorDtoComposer.CreateMany(1).ToList();
            var movie = _movieDtoComposer.With(x => x.Actors, actors).Create();

            var result = _validator.Validate(movie);

            Assert.Contains(expectedErrorMessage, result.Errors.Select(x => x.ErrorMessage).ToList());
        }
        [Fact]
        public void ShouldReturnIsValidTrueWhenAssignActorsAreTwo() {
            var actors = _actorDtoComposer.CreateMany(2).ToList();
            var movie = _movieDtoComposer.With(x => x.Actors, actors).Create();

            var result = _validator.Validate(movie);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ShouldReturnIsValidFalseWhenDirectorFirstNameIsEmpty() {
            var director = new DirectorDto(string.Empty, "LastName");
            var movie = _movieDtoComposer.With(x => x.Director, director).Create();

            var result = _validator.Validate(movie);

            Assert.False(result.IsValid);
        }
    }
}
