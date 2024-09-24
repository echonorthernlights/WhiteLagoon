using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class AmenityViewModel
    {
        public Amenity Amenity { get; set; } = new Amenity()
        {
            Name = ""
        };
        public IEnumerable<SelectListItem>? VillaList { get; set; }
    }
}
