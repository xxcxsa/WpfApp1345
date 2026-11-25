
using System.Collections.Generic;

namespace WpfApp1345.Models.MasterFloor
{
    public class ProductType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Coefficient { get; set; }

        public ICollection<Product> Products { get; set; }
            = new List<Product>();
    }
}
