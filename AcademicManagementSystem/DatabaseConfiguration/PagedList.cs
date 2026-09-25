namespace AcademicManagementSystem.DatabaseConfiguration
{
    public class PagedList<T>
    {
        public List<T> Records { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
