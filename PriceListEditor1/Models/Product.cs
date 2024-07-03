using System.ComponentModel.DataAnnotations.Schema;

namespace PriceListEditor1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }

        public int PriceListId { get; set; }
        public PriceList? PriceList { get; set; }

        [NotMapped]
        public IDictionary<string, string> DynamicColumns { get; set; }

        public Product()
        {
            DynamicColumns = new Dictionary<string, string>();
        }
    }

}
