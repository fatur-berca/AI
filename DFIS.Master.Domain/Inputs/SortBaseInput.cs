namespace DFIS.Master.Domain.Inputs
{
    public class SortBaseInput
    {
        public string SortExpression { get; set; }
        public string SortOrder { get; set; }
        public string SortExpression2 { get; set; }
        public string SortOrder2 { get; set; }

        //public bool isData1Searchable { get; set; }
        //public bool isData1Sortable { get; set; }
        //public bool isData2Searchable { get; set; }
        //public bool isData2Sortable { get; set; }

        //public int sortColumnIndex { get; set; }
        //public string sortDirection { get; set; }
    }
}
