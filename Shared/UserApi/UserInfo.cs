namespace Shared.UserApi;

public class UserInfo : User
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}