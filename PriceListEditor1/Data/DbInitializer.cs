using Microsoft.EntityFrameworkCore;
using PriceListEditor1.Data;
using PriceListEditor1.Models;

namespace PriceListEditor.Data
{
    public static class DbInitializer
    {
        public static void Initialize(PriceListContext context)
        {
            // Check if the database has been created
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Look for any price lists
            if (context.PriceLists.Any())
            {
                return;   // DB has been seeded
            }

            var priceLists = new[]
            {
                new PriceList
                {
                    Name = "Electronics",
                    Columns = new List<Column>
                    {
                        new Column { Name = "Warranty Period", DataType = ColumnType.String, IsCustom = false },
                        new Column { Name = "Manufacturer", DataType = ColumnType.String, IsCustom = false },
                        new Column { Name = "RAM Size", DataType = ColumnType.Integer, IsCustom = true },
                        new Column { Name = "Price", DataType = ColumnType.Decimal, IsCustom = true } 
                    },
                    Products = new List<Product>
                    {
                        new Product
                        {
                            ProductName = "Laptop",
                            ProductCode = "LP1001",
                            DynamicColumns = new Dictionary<string, string>
                            {
                                { "Warranty Period", "2 years" },
                                { "Manufacturer", "Dell" },
                                { "RAM Size", "8" },
                                { "Price", "1200" }
                            }
                        },
                        new Product
                        {
                            ProductName = "Smartphone",
                            ProductCode = "SP2002",
                            DynamicColumns = new Dictionary<string, string>
                            {
                                { "Warranty Period", "1 year" },
                                { "Manufacturer", "Samsung" },
                                { "RAM Size", "6" },
                                { "Price", "800" }
                            }
                        }
                    }
                },
                new PriceList
                {
                    Name = "Furniture",
                    Columns = new List<Column>
                    {
                        new Column { Name = "Material", DataType = ColumnType.String, IsCustom = false },
                        new Column { Name = "Weight Capacity", DataType = ColumnType.String, IsCustom = false },
                        new Column { Name = "Length", DataType = ColumnType.Integer, IsCustom = true },
                        new Column { Name = "Width", DataType = ColumnType.Integer, IsCustom = true }
                    },
                    Products = new List<Product>
                    {
                        new Product
                        {
                            ProductName = "Office Chair",
                            ProductCode = "OC3003",
                            DynamicColumns = new Dictionary<string, string>
                            {
                                { "Material", "Leather" },
                                { "Weight Capacity", "120 kg" },
                                { "Length", "120" },
                                { "Width", "60" }
                            }
                        },
                        new Product
                        {
                            ProductName = "Dining Table",
                            ProductCode = "DT4004",
                            DynamicColumns = new Dictionary<string, string>
                            {
                                { "Material", "Wood" },
                                { "Weight Capacity", "250 kg" },
                                { "Length", "180" },
                                { "Width", "90" }
                            }
                        }
                    }
                }
            };

            context.PriceLists.AddRange(priceLists);
            context.SaveChanges();
        }
    }
}
