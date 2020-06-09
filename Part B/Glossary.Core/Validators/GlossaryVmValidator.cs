using FluentValidation;
using Glossary.Core.ViewModels;

namespace Glossary.Core.Validators
{
    public class GlossaryVmValidator : AbstractValidator<GlossaryVm>
    {
        public GlossaryVmValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
            RuleFor(x => x.Term)
                .NotEmpty();
            RuleFor(x => x.Definition)
                .NotEmpty();
        }
    }
}
