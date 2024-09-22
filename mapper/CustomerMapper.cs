using AutoMapper;
using inventory_api.Data;
using inventory_api.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Define your object-to-object mappings here
        CreateMap<Customer, CustomerDTO>();
        CreateMap<CustomerDTO, Customer>();

        // Mapping for creating a customer
        CreateMap<CustomerCreateDTO, Customer>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // Ignore CreatedAt since it’s set automatically
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());  // Ignore UpdatedAt for create

        // Mapping for updating a customer
        CreateMap<CustomerUpdateDTO, Customer>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())  // Ignore CreatedAt since it should not change
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));  // Set UpdatedAt to current time
    }
}
