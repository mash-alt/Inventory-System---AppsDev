using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_System___AppsDev
{
    internal class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public Category ProductCategory { get; set; }
        public Supplier ProductSupplier { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public string Barcode { get; set; }
        public DateTime DateAdded { get; set; }




    }
}
