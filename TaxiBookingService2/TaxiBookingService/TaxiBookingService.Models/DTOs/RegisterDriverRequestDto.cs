using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiBookingService.Models.DTOs
{
    public class RegisterDriverRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int CabType { get; set; }
    }
}
}
