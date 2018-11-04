using System.ComponentModel.DataAnnotations;

namespace TurretWebServiceCore.ViewModel
{
    public class ChangePasswordModel
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указан старый пароль.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Не указан новый пароль.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароль введён неверно.")]
        public string ConfirmNewPassword { get; set; }
    }
}
