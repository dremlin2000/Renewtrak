using System.Collections.Generic;

namespace Glossary.Core.ViewModels
{
    public class ValidationErrorVm
    {
        public string PropertyName { get; set; }
        public IEnumerable<string> Messages { get; set; }
    }
}
