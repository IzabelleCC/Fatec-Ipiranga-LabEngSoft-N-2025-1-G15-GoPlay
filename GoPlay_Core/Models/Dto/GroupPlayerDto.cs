namespace GoPlay_Core.Models.Dto
{
    public class GroupPlayerDto
    {
        public int Id { get; set; }
        public string FirstUserId { get; set; } = string.Empty;
        public string FirstUserName { get; set; } = string.Empty;
        public string? FirstUserPictureUrl { get; set; }

        public string? SecondUserId { get; set; }
        public string? SecondUserName { get; set; }
        public string? SecondUserPictureUrl { get; set; }
    }

}
