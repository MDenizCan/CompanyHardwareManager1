using AutoMapper;
using CHM.ENTITIES.Entities;
using CHM.MODELS.Asset;

namespace CHM.BLL.Mappings;

// AutoMapper kütüphanesinin konfigürasyon (ayar) sınıfı.
// Hangi Entity'nin hangi DTO'ya (Model) nasıl dönüştürüleceğini burada tanımlarız.
// Böylece servis katmanında manuel olarak (dto.Name = entity.Name) eşleştirme yapmaktan kurtuluruz.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Asset (Entity) -> AssetResponse (DTO) Dönüşümü
        CreateMap<Asset, AssetResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name.ToString() : null));

        // Asset (Entity) -> AssetListResponse (DTO) Dönüşümü
        CreateMap<Asset, AssetListResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name.ToString() : null));

        // User (Entity) -> UserResponse (DTO) Dönüşümü
        CreateMap<CHM.ENTITIES.Entities.User, CHM.MODELS.User.UserResponse>()
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

        // Assignment (Entity) -> AssignmentResponseDto (DTO) Dönüşümü
        CreateMap<CHM.ENTITIES.Entities.Assignment, CHM.MODELS.Assignment.AssignmentResponseDto>()
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
            .ForMember(dest => dest.AssetSerialNumber, opt => opt.MapFrom(src => src.Asset.SerialNumber))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.AssignedByUsername, opt => opt.MapFrom(src => src.AssignedByUser != null ? src.AssignedByUser.Username : null));

        // Request (Entity) -> RequestResponseDto (DTO) Dönüşümü
        CreateMap<CHM.ENTITIES.Entities.Request, CHM.MODELS.Request.RequestResponseDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.Name : null))
            .ForMember(dest => dest.AssetSerialNumber, opt => opt.MapFrom(src => src.Asset != null ? src.Asset.SerialNumber : null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
