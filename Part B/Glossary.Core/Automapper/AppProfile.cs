using AutoMapper;
using Glossary.Core.ViewModels;

namespace Glossary.Core.Automapper
{
    public class AppProfile : Profile
    {
        public AppProfile()
        {
            CreateMap<Models.Glossary, GlossaryVm>().ReverseMap();
        }
    }
}
