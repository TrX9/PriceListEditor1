using System.ComponentModel.DataAnnotations;

namespace PriceListEditor1.Models
{
    public enum ColumnType
    {
        String,
        Integer,
        Decimal
    }

    public class Column
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public ColumnType DataType { get; set; }
        public bool IsCustom { get; set; }
        public int PriceListId { get; set; }
        public PriceList? PriceList { get; set; }

        public bool HasData(IEnumerable<Product> products)
        {
            foreach (var product in products)
            {
                if (product.DynamicColumns != null && product.DynamicColumns.ContainsKey(Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
