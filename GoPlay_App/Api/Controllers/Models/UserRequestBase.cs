namespace GoPlay_UserManagementService_App.Api.Controllers.Models
{
    public class UserRequestBase<T>
    {
        public T Data { get; set; } = default!;
    }
}
