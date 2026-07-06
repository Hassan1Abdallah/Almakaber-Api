namespace Almakaber.BLL.DTOs.Account
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; }

    }
}