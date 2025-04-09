namespace GoPlay_App.Api.Controllers
{
    /// <summary>
    /// Base class for user requests
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UserRequestBase<T>
    {
        /// <summary>
        /// Data property to hold the request data
        /// </summary>
        public T Data { get; set; } = default!;
    }
}
