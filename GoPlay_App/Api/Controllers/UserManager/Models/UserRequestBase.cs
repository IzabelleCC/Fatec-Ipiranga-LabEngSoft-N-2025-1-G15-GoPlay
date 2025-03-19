namespace GoPlay_App.Api.Controllers.UserController.Models
{
    public class UserRequestBase<T>
    {
        public T Data { get; set; } = default!;
    }
}
