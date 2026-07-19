using Application.Modules.IdeaFields;
using Application.Modules.Languages;
using Application.Modules.Roles;
using Application.Modules.Skills;
using AutoMapper;
using Domain.Models.Entities;
using Domain.Models.Entities.Student;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Skill, SkillDto>();
            CreateMap<Language, LanguageDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<IdeaField, IdeaFieldDto>();
        }
    }
}