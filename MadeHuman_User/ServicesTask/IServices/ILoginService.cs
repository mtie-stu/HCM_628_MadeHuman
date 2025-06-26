using Madehuman_Share.ViewModel;


namespace MadeHuman_User.Services.IServices
{
    public interface ILoginService
    {
        Task<LoginResultDto?> LoginAsync(LoginModel model);
    }
}
