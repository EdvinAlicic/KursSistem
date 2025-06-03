using AutoMapper;
using KursSistemDiplomskiRad.DTOs;
using KursSistemDiplomskiRad.Entities;

namespace KursSistemDiplomskiRad.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.Kursevi, opt => opt.MapFrom(src => src.StudentKursevi.Select(sk => sk.Kurs.Naziv)));

            CreateMap<StudentKurs, StudentKursDto>()
                .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
                .ForMember(dest => dest.KursId, opt => opt.MapFrom(src => src.KursId))
                .ForMember(dest => dest.DatumPrijave, opt => opt.MapFrom(src => src.DatumPrijave))
                .ForMember(dest => dest.StatusPrijave, opt => opt.MapFrom(src => src.StatusPrijave))
                .ForMember(dest => dest.StudentIme, opt => opt.MapFrom(src => src.Student.Ime))
                .ForMember(dest => dest.KursNaziv, opt => opt.MapFrom(src => src.Kurs.Naziv));

            CreateMap<Kurs, KursDto>().ReverseMap();

            CreateMap<Lekcije, LekcijaDto>().ReverseMap();

            CreateMap<StudentDto, Student>();

            CreateMap<KursCreateDto, Kurs>();

            CreateMap<LekcijaCreateDto, Lekcije>();

            CreateMap<KursUpdateDto, Kurs>();

            CreateMap<LekcijaZaUpdateDto, Lekcije>();

            CreateMap<Kurs, KursBasicDto>();

            CreateMap<Student, IspisStudenataDto>();

            CreateMap<Kurs, KursIspisZaStudentaDto>();

            CreateMap<StudentKurs, StudentOnKursDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Student.Id))
                .ForMember(dest => dest.Ime, opt => opt.MapFrom(src => src.Student.Ime))
                .ForMember(dest => dest.Prezime, opt => opt.MapFrom(src => src.Student.Prezime))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Student.Email))
                .ForMember(dest => dest.DatumPrijave, opt => opt.MapFrom(src => src.DatumPrijave))
                .ForMember(dest => dest.StatusPrijave, opt => opt.MapFrom(src => src.StatusPrijave));

            CreateMap<KursOcjenaCreateDto, KursOcjena>();
            CreateMap<KursOcjena, KursOcjenaDto>();
            CreateMap<KursOcjena, KursOcjenaPrikazDto>()
                .ForMember(dest => dest.ImeStudenta, opt => opt.MapFrom(src => src.Student.Ime))
                .ForMember(dest => dest.PrezimeStudenta, opt => opt.MapFrom(src => src.Student.Prezime))
                .ForMember(dest => dest.Datum, opt => opt.MapFrom(src => src.Datum));

            CreateMap<StudentLekcijaProgress, StudentLekcijaProgressDto>();
        }
    }
}