using AutoMapper;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentReadDto>()
                .ForMember(dest => dest.Kursevi, opt => opt.MapFrom(src => src.StudentKursevi.Select(sk => sk.Kurs.Naziv)));
            CreateMap<StudentCreateDto, Student>();
            CreateMap<StudentUpdateDto, Student>();

            // Mape za Kurs
            CreateMap<Kurs, KursReadDto>()
                .ForMember(dest => dest.Studenti, opt => opt.MapFrom(src => src.StudentKursevi.Select(sk => sk.Student.Ime + " " + sk.Student.Prezime)))
                .ForMember(dest => dest.Lekcije, opt => opt.MapFrom(src => src.Lekcije));
            CreateMap<KursCreateDto, Kurs>();
            CreateMap<KursUpdateDto, Kurs>();

            // Mape za Lekcije
            CreateMap<Lekcije, LekcijaReadDto>();
            CreateMap<LekcijaCreateDto, Lekcije>();

            // Mape za StudentKurs
            CreateMap<StudentKurs, StudentKursReadDto>();
            CreateMap<StudentKursCreateDto, StudentKurs>();
        }
    }
}
