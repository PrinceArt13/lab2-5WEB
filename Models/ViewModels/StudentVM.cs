using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lab2WEB.Models.ViewModels
{
    public class StudentVM
    {
        public System.Guid ID_студента { get; set; }
        //id lastname name patronumic pall adres
        [Required]
        [DisplayName("Фамилия")]
        public string Фамилия { get; set; }
        [Required]
        [DisplayName("Имя")]
        public string Имя { get; set; }
        [Required]
        [DisplayName("Отчество")]
        public string Отчество { get; set; }
        [Required]
        [DisplayName("Пол")]
        public bool Пол { get; set; }
        [Required]
        [DisplayName("Адрес проживания")]
        public string Адрес_проживания { get; set; }
        [Required]
        [DisplayName("Дата рождения")]
        [DisplayFormat(DataFormatString="{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime Дата_рождения { get; set; }
        public int Код_студента { get; set; }
    }
}