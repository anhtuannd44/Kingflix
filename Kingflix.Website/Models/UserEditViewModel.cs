﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class UserEditViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Phone]
        [Display(Name = "Phone Number")]
        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

    }
}