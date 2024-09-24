﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhiteLagoon.Domain.Entities
{
    public class Villa
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public required string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Price per night")]
        [Range(10, 10000)]
        public double Price { get; set; }

        public int Sqft { get; set; }

        [Range(1, 10)]
        public int Occupancy { get; set; }

        [Display(Name = "Image Url")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public bool IsAvailable { get; set; } = true;

        [ValidateNever]
        public IEnumerable<Amenity>? VillaAmenities { get; set; }
    }
}
