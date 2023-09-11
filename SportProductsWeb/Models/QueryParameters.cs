namespace SportProductsWeb.Models
{
    public class QueryParameters
    {

        const int _maxSize = 100;
        int _size = 20;

        public int CurPage { get; set; } = 1;

        public int Size {
            get {
                return _size;
            } 
            set {
                _size = Math.Min(_maxSize, value);
            } 
        }

        public string SortBy { get; set; } = "Id";

        string _sortOrder = "asc";
        public string SortOrder
        {
            get { return _sortOrder; }
            set
            {
                if(value=="asc" || value=="desc")
                {
                    _sortOrder = value;
                }
            }
        }


    }
}
