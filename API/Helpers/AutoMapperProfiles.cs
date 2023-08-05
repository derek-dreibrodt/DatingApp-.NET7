using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;
using AutoMapper.Execution;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember( // maps a particular property on the Dto to a property on the original class
                    dest => dest.PhotoUrl, // map the destination variable
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url) // map the source
                    )
                .ForMember( // maps a particular property on the Dto to a property on the original class
                    dest => dest.Age, // map the destination variable
                    opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()) // map the source
                    );
            CreateMap<Photo, PhotoDto>();

            CreateMap<MemberUpdateDTO, AppUser> ();

            CreateMap<RegisterDto, AppUser>();
        }
    }
}