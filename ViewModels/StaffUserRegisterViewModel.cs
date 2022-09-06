using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CCAP.Api.ViewModels
{
    public class StaffUserRegisterViewModel : GeneralUserRegisterViewModel
    {
        [Required]
        public string[] Roles { get; set; }
    }
}
