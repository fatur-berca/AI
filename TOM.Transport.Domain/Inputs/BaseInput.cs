namespace TOM.Transport.Domain.Inputs
{
    public class BaseInput : SortBaseInput
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
