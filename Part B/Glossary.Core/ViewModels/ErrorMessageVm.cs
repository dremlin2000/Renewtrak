using System.Collections.Generic;

namespace Glossary.Core.ViewModels
{
    public class ErrorMessageVm
    {
        public string ErrorCode { get; set; }
        public string ErrorType { get; set; }
        public IEnumerable<ValidationErrorVm> Errors { get; set; }
    }
}
