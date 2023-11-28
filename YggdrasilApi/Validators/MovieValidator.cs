using FluentValidation;
using YggdrasilApi.Dto;

namespace YggdrasilApi.Validators {
    public class MovieValidator : AbstractValidator<MovieDto> {
        public MovieValidator() {
            RuleFor(x => x.BoxOffice).InclusiveBetween(1, 1000000000);
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.ReleaseDare).NotEmpty();
            RuleFor(x => x.Director).NotNull()
                .SetValidator(new DirectorValidator()); 
            RuleFor(x => x.Actors).NotNull();
            RuleFor(x => x.Actors).Custom((list, context) => {
                if (list.Count < 2) {
                    context.AddFailure("Movie must contains at least 2 actors");
                }
            });
        }
    }
}
