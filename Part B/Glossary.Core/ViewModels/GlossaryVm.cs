using System;

namespace Glossary.Core.ViewModels
{
    public class GlossaryVm
    {
        public Guid Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
    }
}
