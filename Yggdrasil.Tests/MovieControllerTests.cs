using AutoFixture;
using AutoFixture.Dsl;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;
using Yggdrasil.Data.Entity;
using Yggdrasil.Data.Entity.Enums;
using YggdrasilApi.Controllers;
using YggdrasilApi.Dto;
using YggdrasilApi.Services.Contracts;

namespace Yggdrasil.Tests {
    public class MovieControllerTests {
        private readonly MovieController _movieController;
        private readonly Mock<ILogger<MovieController>> _logger;
        private readonly Mock<IMovieService> _movieService;
        private readonly Mock<IValidator<MovieDto>> _validator;
        private readonly IPostprocessComposer<MovieDto> _movieDtoComposer;
        private readonly IPostprocessComposer<Movie> _movieComposer;
        public MovieControllerTests() {
            var fixture = new Fixture();
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

            _movieComposer = fixture.Build<Movie>();
            _movieDtoComposer = fixture.Build<MovieDto>();

            _movieService = new Mock<IMovieService>();
            _logger = new Mock<ILogger<MovieController>>();
            _validator = new Mock<IValidator<MovieDto>>();

            _movieController = new MovieController(_logger.Object, _movieService.Object, _validator.Object);
        }
        [Fact]
        public void GetAll_ShloudInvokeMovieServiceGetAll() {
            _movieController.GetAll();
            _movieService.Verify(x => x.GetAll());
        }
        [Fact]
        public void GetAll_ShouldReturnAllMoviesAsReturnMovieServiceGetAll() {
            var numberRecords = new Random().Next(0, 100);
            var movieFromDb = _movieComposer.CreateMany(numberRecords);
            _movieService.Setup(x => x.GetAll()).Returns(movieFromDb);

            var result = _movieController.GetAll();
            Assert.Equal(numberRecords, result.Count());
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("", "Test")]
        [InlineData("  ", "Test")]
        [InlineData("  ", "  ")]
        [InlineData("Test", "  ")]
        public async Task GetByDirector_ShouldReturnBadRequestIfFirstNameOrLastNameAreNullOrWhiteSpace(string firstName, string lastName) {
            var result = await _movieController.GetByDirector(firstName, lastName);

            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("", "Test")]
        [InlineData("  ", "Test")]
        [InlineData("  ", "  ")]
        [InlineData("Test", "  ")]
        public async Task GetByDirector_ShouldnvokeMovieServiceFindByConditionIfFirstNameOrLastNameAreNotNullOrWhiteSpace(string firstName, string lastName) {
            var result = await _movieController.GetByDirector(firstName, lastName);

            _movieService.Verify(x => x.FindByCondition(It.IsAny<Expression<Func<Movie, bool>>>()), Times.Never);
        }
        [Theory]
        [InlineData("Test", "Test")]
        [InlineData(" Test  ", "Test")]
        [InlineData(" Test  ", " Test")]
        [InlineData("Test", "  Test")]
        public async Task GetByDirector_ShouldNotReturnBadRequestIfFirstNameOrLastNameAreNotNullOrWhiteSpace(string firstName, string lastName) {
            var result = (await _movieController.GetByDirector(firstName, lastName));

            Assert.IsNotType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetByDirector_ShouldInvokeMovieServiceFindByConditionIfFirstNameOrLastNameAreNotNullOrWhiteSpace() {
            var firstName = "FirstName";
            var lastName = "LastName";

            var _ = await _movieController.GetByDirector(firstName, lastName);

            _movieService.Verify(x => x.FindByCondition(x =>
                x.Director.FirstName == firstName.Trim() &&
                x.Director.LastName == lastName.Trim()), Times.Once);
        }

        [Fact]
        public async Task GetByDirector_ShouldReturnAllMoviesAsReturnMovieServiceFindByConditionIfFirstNameOrLastNameAreNotNullOrWhiteSpace() {
            var firstName = "FirstName";
            var lastName = "LastName";
            var numberRecords = new Random().Next(0, 100);
            var movieFromDb = _movieComposer.CreateMany(numberRecords);

            _movieService.Setup(x => x.FindByCondition(x =>
                x.Director.FirstName == firstName.Trim() &&
                x.Director.LastName == lastName.Trim())).ReturnsAsync(movieFromDb);

            var result = ((OkObjectResult)await _movieController.GetByDirector(firstName, lastName)).Value;
            Assert.Equal(numberRecords, (result as IEnumerable<MovieDto>)?.Count());
        }
        [Fact]
        public async Task GetByGenre_ShouldNotInokeFindByConditionShouldReturnEmptyListIfGenresParameterIsNull() {

            var result = await _movieController.GetByGenre(Array.Empty<string>());

            _movieService.Verify(x => x.FindByCondition(It.IsAny<Expression<Func<Movie, bool>>>()), Times.Never);

        }
        [Fact]
        public async Task GetByGenre_ShouldShouldReturnEmptyListIfGenresParameterIsNull() {
            var result = await _movieController.GetByGenre(Array.Empty<string>());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSignle_ShouldInvokeGetByIdMovieServie() {

            var _ = await _movieController.GetSingle(It.IsNotNull<int>());
            _movieService.Verify(x => x.GetById(It.IsNotNull<int>()));
        }
        [Fact]
        public async Task GetSignle_ShouldReturnNotFoundObject() {
            _movieService.Setup(x => x.GetById(It.IsNotNull<int>()));

            var result = (await _movieController.GetSingle(It.IsNotNull<int>())).Result;

            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task GetSignle_ShouldReturnStatusCode404() {
            _movieService.Setup(x => x.GetById(It.IsNotNull<int>()));

            var result = (await _movieController.GetSingle(It.IsNotNull<int>())).Result as NotFoundResult;

            Assert.Equal(StatusCodes.Status404NotFound, result?.StatusCode);
        }
        [Fact]
        public async Task GetSignle_ShouldReturnOkResultObject() {
            var movieDb = _movieComposer.Create();
            _movieService.Setup(x => x.GetById(It.IsNotNull<int>())).ReturnsAsync(movieDb);

            var result = (await _movieController.GetSingle(It.IsNotNull<int>())).Result;

            Assert.IsType<OkObjectResult>(result);
        }
        [Fact]
        public async Task GetSignle_ShouldReturnExpectedTitleFromGetById() {
            var expectedTitle = "Expected Title";
            var movieDb = _movieComposer.With(x => x.Title, expectedTitle).Create();
            _movieService.Setup(x => x.GetById(It.IsNotNull<int>())).ReturnsAsync(movieDb);

            var okObjectResult = (await _movieController.GetSingle(It.IsNotNull<int>())).Result as OkObjectResult;
            var movieDto = okObjectResult.Value as MovieDto;

            Assert.Equal(expectedTitle, movieDto.Title);
        }
        [Fact]
        public async Task GetSignle_ShouldReturnTheSameNumberOfActorsFromGetById() {

            var movieDb = _movieComposer.Create();
            _movieService.Setup(x => x.GetById(It.IsNotNull<int>())).ReturnsAsync(movieDb);

            var okObjectResult = (await _movieController.GetSingle(It.IsNotNull<int>())).Result as OkObjectResult;
            var movieDto = okObjectResult?.Value as MovieDto;

            Assert.Equal(movieDb.Actors.Count, movieDto?.Actors.Count);
        }
        [Fact]
        public async Task Add_ShouldNotInvokeValidateIfParameterIsNull() {
            var _ = await _movieController.Add(It.IsAny<MovieDto>());
            _validator.Verify(x => x.Validate(It.IsAny<MovieDto>()), Times.Never);
        }
        [Fact]
        public async Task Add_ShouldReturnBadRequestObjectResultIfParameterIsNull() {
            var result = await _movieController.Add(It.IsAny<MovieDto>());
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Add_ShouldInvokeValidateIfParameterIsNotNull() {
            var movieDto = _movieDtoComposer.Create();
            _validator.Setup(x => x.Validate(It.IsAny<MovieDto>())).Returns(new ValidationResult());

            var _ = await _movieController.Add(movieDto);
            _validator.Verify(x => x.Validate(movieDto), Times.Once);
        }
        [Fact]
        public async Task Add_ShouldReturnBadRequestObjectIfValidateReturnIsValidFalse() {
            var movieDto = _movieDtoComposer.Create();
            var validationResult = new ValidationResult() {
                Errors = new List<ValidationFailure>() {
                    new ValidationFailure()
                    { ErrorMessage = "ErrorMessage" }
                }
            };

            _validator.Setup(x => x.Validate(movieDto)).Returns(validationResult);

            var result = await _movieController.Add(movieDto);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task Add_ShouldReturnBadRequestWithExpectedErrorMessageIfValidateReturnIsValidFalse() {
            var expectedErrorMessage = "ErrorMessageTest";
            var movieDto = _movieDtoComposer.Create();
            var validationResult = new ValidationResult() {
                Errors = new List<ValidationFailure>() {
                    new ValidationFailure()
                    { ErrorMessage = expectedErrorMessage }
                }
            };

            _validator.Setup(x => x.Validate(movieDto)).Returns(validationResult);

            var result = (await _movieController.Add(movieDto) as BadRequestObjectResult);

            List<string>? errorList = result?.Value as List<string>;
            Assert.Contains(expectedErrorMessage, errorList.AsEnumerable());
        }
        [Fact]
        public async Task Add_ShouldNotInvokeAddMovieServiceIfValidateReturnIsValidFalse() {
            var expectedErrorMessage = "ErrorMessageTest";
            var movieDto = _movieDtoComposer.Create();
            var validationResult = new ValidationResult() {
                Errors = new List<ValidationFailure>() {
                    new ValidationFailure()
                    { ErrorMessage = expectedErrorMessage }
                }
            };

            _validator.Setup(x => x.Validate(movieDto)).Returns(validationResult);

            var result = (await _movieController.Add(movieDto) as BadRequestObjectResult);

            _movieService.Verify(x => x.Add(It.IsAny<Movie>()), Times.Never);
        }
        [Fact]
        public async Task Add_ShoulInvokeAddMovieServiceIfValidateReturnIsValid() {
            var movieDto = _movieDtoComposer.Create();
            var validationResult = new ValidationResult();

            _validator.Setup(x => x.Validate(movieDto)).Returns(validationResult);

            var _ = await _movieController.Add(movieDto);

            _movieService.Verify(x => x.Add(It.IsAny<Movie>()), Times.Once);
        }
        //[Fact]
        //public async Task Add_ShoulInvokeAddMovieServiceIfValidateReturnIsValisssssssssssd() {
        //    var movieDto = _movieDtoComposer.Create();
        //    var validationResult = new ValidationResult();

        //    _validator.Setup(x => x.Validate(movieDto)).Returns(validationResult);

        //    MethodInfo method = _movieController.GetType().GetMethod("Map", BindingFlags.NonPublic | BindingFlags.Instance);

        //    object[] param = new object[1] { movieDto };
        //    var movie = method.Invoke(_movieController, param) as Movie;

        //    var _ = await _movieController.Add(movieDto);

        //    Assert.Equal(movie.Title, movieDto.Title);
        //}
    }
}
