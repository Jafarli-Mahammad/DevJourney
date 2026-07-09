using Application.Modules.Languages;
using Application.Modules.Skills;
using AutoMapper;
using Domain.Models.Entities.Student;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Skill, SkillDto>();
            CreateMap<Language, LanguageDto>();
        }
    }
}