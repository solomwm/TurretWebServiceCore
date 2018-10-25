﻿using System.ComponentModel.DataAnnotations;

namespace TurretWebServiceCore.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указано имя пользователя.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
