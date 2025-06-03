namespace GoPlay_Core.Models.Dto
{
    public class CategoryGroupsDto
    {
        public int CategoryId { get; set; }
        public List<GroupDto> Groups { get; set; } = new();
    }

}
