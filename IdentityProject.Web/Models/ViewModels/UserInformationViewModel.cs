namespace IdentityProject.Web.Models.ViewModels
{
    public class UserInformationViewModel
    {
        public string? UserName { get; set; } = null!; //null olamaz
        public string? Email { get; set; }
        public string? phoneNumber { get; set; } //? ile nullable olabildiği anlamına gelir

    }
}
