
using System.Collections.Generic;

namespace WpfApp1345.Models.MasterFloor
{
    public class Product
    {
        public int Id { get; set; }

        public int ProductTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Article { get; set; }

        public decimal MinPartnerPrice { get; set; }

        public ProductType ProductType { get; set; } = null!;

        public ICollection<PartnerSale> PartnerSales { get; set; }
            = new List<PartnerSale>();
    }
}
