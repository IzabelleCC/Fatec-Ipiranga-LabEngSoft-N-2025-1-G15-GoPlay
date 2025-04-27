namespace GoPlay_App.Api.Controllers.TournamentManager.Models
{
    public class TournamentRequestBase<T>
    {
        /// <summary>
        /// Data property to hold the request data
        /// </summary>
        public T Data { get; set; } = default!;
    }
}