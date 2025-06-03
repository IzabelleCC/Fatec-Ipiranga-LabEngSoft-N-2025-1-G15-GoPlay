namespace GoPlay_Core.Models.Dto
{
    public class GroupDto
    {
        public int GroupNumber { get; set; }
        public List<GroupPlayerDto> Players { get; set; } = new();
    }

}
