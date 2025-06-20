namespace MadeHuman_User.Services.IServices
{
    public interface ILoginService
    {
        bool ValidateUser(string emailOrID, string password);
    }
}
