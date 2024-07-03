namespace PriceListEditor1.Models
{
    public class PriceListColumn
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DataType { get; set; }
        public int PriceListId { get; set; }
        public PriceList PriceList { get; set; }
    }
}
