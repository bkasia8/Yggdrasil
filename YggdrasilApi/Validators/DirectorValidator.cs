using FluentValidation;
using YggdrasilApi.Dto;

namespace YggdrasilApi.Validators {
    public class DirectorValidator :AbstractValidator<DirectorDto> {

        public DirectorValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
        }
    }
}
