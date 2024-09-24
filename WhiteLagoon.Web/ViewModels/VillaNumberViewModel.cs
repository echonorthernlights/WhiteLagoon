﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class VillaNumberViewModel
    {
        public VillaNumber villaNumber { get; set; } = new VillaNumber();

        [ValidateNever]
        public IEnumerable<SelectListItem>? VillaList { get; set; }
    }
}
