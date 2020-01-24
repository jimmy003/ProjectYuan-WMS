using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;



namespace Project.FC2J.Models.Dtos
{
    public class UserForRegisterDto
    {
        
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
