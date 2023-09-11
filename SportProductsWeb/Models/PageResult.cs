namespace SportProductsWeb.Models
{
    public class QueryPageResult
    {
        public int TotalCount { get; set; } = 0;
        public int TotalPages { get; set; } = 1;
        public int? PreviousPage { get; set; }
        public int? NextPage { get; set; }
        public int FirstRowOnPage { get; set; }
        public int LastRowOnPage { get; set; }
    }
}
