using AutoMapper;
using back_bd.Entidades;
using back_bd.DTO_s;

namespace back_bd.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapeos para Generos
            CreateMap<Genero, GeneroReadDTO>();
            CreateMap<GeneroCreateDTO, Genero>();
            CreateMap<Genero, GeneroSimpleDTO>();

            // Mapeos para Estudios
            CreateMap<Estudios, EstudiosReadDTO>();
            CreateMap<EstudiosCreateDTO, Estudios>()
                .ForMember(dest => dest._id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Animes, opt => opt.Ignore());

            // Mapeos para Animes - CORREGIDO para usar AnimeGeneros
            CreateMap<AnimeCreateDTO, Anime>()
                .ForMember(dest => dest._id, opt => opt.Ignore())
                .ForMember(dest => dest.ImagenUrl, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Estudio, opt => opt.Ignore())
                .ForMember(dest => dest.Episodios, opt => opt.Ignore())
                .ForMember(dest => dest.Favoritos, opt => opt.Ignore())
                .ForMember(dest => dest.AnimeGeneros, opt => opt.Ignore());

            CreateMap<Anime, AnimeReadDTO>()
                .ForMember(dest => dest.EstudioNombre, opt => opt.MapFrom(src => src.Estudio.Nombre))
                .ForMember(dest => dest.Generos, opt => opt.MapFrom(src => 
                    src.AnimeGeneros.Select(ag => new GeneroSimpleDTO 
                    { 
                        _id = ag.Genero._id, 
                        Nombre = ag.Genero.Nombre 
                    })));


            // Mapeos para Episodios
            CreateMap<Episodios, EpisodioReadDTO>()
                .ForMember(dest => dest.AnimeTitulo, opt => opt.MapFrom(src => src.Anime.Titulo));

            CreateMap<EpisodioCreateDTO, Episodios>()
                .ForMember(dest => dest._id, opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro, opt => opt.Ignore())
                .ForMember(dest => dest.FechaModificacion, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Anime, opt => opt.Ignore())
                .ForMember(dest => dest.HistorialVisualizaciones, opt => opt.Ignore());
        }
    }
}