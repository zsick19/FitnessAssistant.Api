using System;
using AutoMapper;

namespace FitnessAssistant.Api.Shared.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RawIngredient, UpdatePatchRawIngredientReqDto>().ReverseMap().ForMember(dest => dest.FoodGroup, opt => opt.Ignore());
    }
}
