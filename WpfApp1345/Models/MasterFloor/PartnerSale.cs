
namespace WpfApp1345.Models.MasterFloor
{
    public class PartnerSale
    {
        public int Id { get; set; }

        public int PartnerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public System.DateTime SaleDate { get; set; }

        public Partner Partner { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
