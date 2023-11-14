using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lab2WEB.Models.ViewModels
{
    public class UserVM
    {
        [Required]
        [DisplayName("Логин")]
        public string Login { get; set; }
        
        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }
    }
}