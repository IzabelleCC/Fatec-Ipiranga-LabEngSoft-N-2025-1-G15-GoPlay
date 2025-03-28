﻿using System.ComponentModel.DataAnnotations;

namespace GoPlay_App.Api.Controllers.AccessManager.Models
{
    public class PasswordResetRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
