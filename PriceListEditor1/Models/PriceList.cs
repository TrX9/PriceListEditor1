using System.ComponentModel.DataAnnotations;

namespace PriceListEditor1.Models
{
    public class PriceList
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Column> Columns { get; set; } = new List<Column>();
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public void AddCustomColumn(string columnName, ColumnType dataType)
        {
            Columns.Add(new Column
            {
                Name = columnName,
                DataType = dataType,
                IsCustom = true
            });
        }
    }

}
