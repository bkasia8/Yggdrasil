using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Data.Entity;
using Yggdrasil.Data.Entity.Enums;
using YggdrasilApi.Dto;
using YggdrasilApi.Services.Contracts;

namespace YggdrasilApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase {
        private readonly ILogger<MovieController> _logger;
        private readonly IMovieService _movieService;
        private readonly IValidator<MovieDto> _validator;
        public MovieController(
            ILogger<MovieController> logger,
            IMovieService movieService,
            IValidator<MovieDto> validator) {
            _logger = logger;
            _movieService = movieService;
            _validator = validator;
        }

        [HttpGet]
        public IEnumerable<MovieDto> GetAll() {
            _logger.LogInformation("Getting movies");
            var movies = _movieService.GetAll();

            var result = movies.Select(Map);
            _logger.LogInformation($"Movies: {result}");

            return result;

        }
        [HttpGet("director")]
        public async Task<ActionResult> GetByDirector([FromQuery] string firstName, [FromQuery] string lastName) {

            _logger.LogInformation($"Getting movies by {firstName} {lastName} ");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) {
                ModelState.AddModelError("Director", "FirstName or LastName is required");

                return BadRequest(ModelState);
            }

            var movies = await _movieService.FindByCondition(x =>
                 x.Director.FirstName == firstName.Trim() &&
                 x.Director.LastName == lastName.Trim());

            var result = movies.Select(Map);

            _logger.LogInformation($"Movies: {result} {lastName} ");

            return Ok(result);
        }
        [HttpGet("genres")]
        public async Task<IEnumerable<MovieDto>> GetByGenre([FromQuery] string[] genres) {

            if (genres.Count() == 0)
                return Enumerable.Empty<MovieDto>();

            var genreEnums = new List<Genre>();
            foreach (var item in genres) {
                Genre genreEnum;
                if (Enum.TryParse(item, out genreEnum))
                    genreEnums.Add(genreEnum);
            }

            var movies = await _movieService
                .FindByCondition(x => genreEnums.Contains(x.Genre));

            return movies.Select(Map);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetSingle(int id) {
            var movie = await _movieService.GetById(id);
            if (movie == null)
                return NotFound();

            return Ok(Map(movie));
        }

        [HttpPost]
        public async Task<ActionResult> Add(MovieDto movieDto) {

            _logger.LogInformation($"Adding movie: {movieDto}");

            if (movieDto == null)
                return BadRequest("MovieDto is null");

            var result = _validator.Validate(movieDto);

            if (!result.IsValid) {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogInformation($"Validation messages: {string.Join(", ",errorMessages)}");
                return BadRequest(errorMessages);
            }
            var movie = Map(movieDto);
            await _movieService.Add(movie);

            _logger.LogInformation($"Added movie: {movieDto}");

            return Ok();
        }

        [HttpPut("{id}")]
        public ActionResult Edit(int id, MovieDto movieDto) {
            var movie = Map(movieDto);
            movie.Id = id;

            _movieService.Edit(movie);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {

            return Ok();
        }

        private MovieDto Map(Movie movie) {
            return new MovieDto(
                                movie.Title,
                                 movie.Genre,
                                 new DirectorDto(
                                     movie.Director.FirstName,
                                     movie.Director.LastName),
                                 movie.ReleaseDate,
                                 movie.BoxOffice,
                                 movie.Runtime,
                                 movie.Actors
                                     .Select(a =>
                                        new ActorDto(
                                            a.FirstName,
                                            a.LastName))
                                        .ToList()
                                        );
        }
        private Movie Map(MovieDto movieDto) {
            return new Movie() {
                Title = movieDto.Title,
                BoxOffice = movieDto.BoxOffice,
                Genre = movieDto.Genre,
                Runtime = movieDto.Runtime,
                ReleaseDate = movieDto.ReleaseDare,
                Actors = movieDto.Actors.Select(x =>
                new Actor {
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .ToList(),
                Director = new Director {
                    FirstName = movieDto.Director.FirstName,
                    LastName = movieDto.Director.LastName
                },
            };
        }
    }
}
