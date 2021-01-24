using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Models.ViewModel
{
    public class UserManageViewModel
    {
        public List<Profile> Profile { get; set; }
        public List<AppUser> UserList { get; set; }
    }
}