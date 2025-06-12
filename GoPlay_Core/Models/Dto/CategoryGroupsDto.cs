namespace GoPlay_Core.Models.Dto
{
    public class CategoryGroupsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<GroupDto> Groups { get; set; } = new();
    }

}
