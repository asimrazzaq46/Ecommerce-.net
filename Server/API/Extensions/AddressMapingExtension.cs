using API.DTOs;
using Core.Models;

namespace API.Extensions;

public static class AddressMapingExtension
{
    public static AddressDto ToDto(this Address? address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));

        return new AddressDto
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            Country = address.Country,
            City = address.City,
            PostalCode = address.PostalCode,
            State = address.State,
        };

    }
    
    public static Address ToEntity(this AddressDto address)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));

        return new Address
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            Country = address.Country,
            City = address.City,
            PostalCode = address.PostalCode,
            State = address.State,
        };

    }
    
    public static void UpdateEntityFromDto(this Address address,AddressDto addressDto)
    {
        if (address == null) throw new ArgumentNullException(nameof(address));
        if (addressDto == null) throw new ArgumentNullException(nameof(addressDto));


        address.Line1 = addressDto.Line1;
        address.Line2 = addressDto.Line2;
        address.Country = addressDto.Country;
        address.City = addressDto.City;
        address.PostalCode = addressDto.PostalCode;
        address.State = addressDto.State;
        

    }
}
