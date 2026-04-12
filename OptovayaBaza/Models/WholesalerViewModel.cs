using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OptovayaBaza.Models
{
    public class WholesalerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Введите название организации")]
        [Display(Name = "Название организации")]
        public string OrganizationName { get; set; }

        [Required(ErrorMessage = "Введите ИНН")]
        [Display(Name = "ИНН")]
        public string INN { get; set; }

        [Required(ErrorMessage = "Введите телефон")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Введите юридический адрес")]
        [Display(Name = "Юридический адрес")]
        public string LegalAddress { get; set; }

        [Display(Name = "Контактное лицо")]
        public string ContactPerson { get; set; }
    }
}