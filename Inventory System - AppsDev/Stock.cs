using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_System___AppsDev
{
    internal class Stock
    {
        public int StockID { get; set; }
        public Product Product { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // "IN" or "OUT"
        public int Quantity { get; set; }
        public string Remarks { get; set; }
    }
}
