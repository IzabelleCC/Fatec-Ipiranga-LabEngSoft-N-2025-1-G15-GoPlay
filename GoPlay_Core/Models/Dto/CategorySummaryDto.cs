namespace GoPlay_Core.Models.Dto
{
    public class CategorySummaryDto
    {
        public int Id { get; set; }
        public string CategoryType { get; set; } = string.Empty;
        public bool IsDoubles { get; set; }
        public int RegisterCount { get; set; }
    }
}
