using AutoMapper;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student -> StudentDto
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Kursevi, opt => opt.MapFrom(src => src.StudentKursevi.Select(sk => sk.Kurs.Naziv)));

            // StudentKurs -> StudentKursDto
            CreateMap<StudentKurs, StudentKursDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.KursId, opt => opt.MapFrom(src => src.KursId))
                .ForMember(dest => dest.DatumPrijave, opt => opt.MapFrom(src => src.DatumPrijave))
                .ForMember(dest => dest.StatusPrijave, opt => opt.MapFrom(src => src.StatusPrijave))
                .ForMember(dest => dest.StudentIme, opt => opt.MapFrom(src => src.Student.Ime))
                .ForMember(dest => dest.KursNaziv, opt => opt.MapFrom(src => src.Kurs.Naziv));

            // Kurs -> KursDto
            CreateMap<Kurs, KursDto>()
                .ForMember(dest => dest.Studenti, opt => opt.MapFrom(src => src.StudentKursevi.Select(sk => sk.Student.Ime + " " + sk.Student.Prezime)))
                .ForMember(dest => dest.Lekcije, opt => opt.MapFrom(src => src.Lekcije));

            // Lekcije -> LekcijaDto
            CreateMap<Lekcije, LekcijaDto>();

            // LekcijaDto -> Lekcije (za kreiranje/izmenu)
            CreateMap<LekcijaDto, Lekcije>();

            // KursDto -> Kurs (za kreiranje/izmenu)
            CreateMap<KursDto, Kurs>();

            // StudentDto -> Student (za kreiranje/izmenu)
            CreateMap<StudentDto, Student>();

            // KursCreateDto -> Kurs (za kreiranje/izmenu)
            CreateMap<KursCreateDto, Kurs>();

            // LekcijaCreateDto -> Lekcije (za kreiranje/izmenu)
            CreateMap<LekcijaCreateDto, Lekcije>();

            // KursUpdateDto -> Kurs (za kreiranje/izmenu)
            CreateMap<KursUpdateDto, Kurs>();
        }
    }
}
