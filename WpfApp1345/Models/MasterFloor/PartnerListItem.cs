
namespace WpfApp1345.Models.MasterFloor
{
    public class PartnerListItem
    {
        public int Id { get; set; }

        public string PartnerTypeName { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Director { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int Rating { get; set; }

        public int TotalSales { get; set; }

        public int DiscountPercent { get; set; }

        public string DiscountText => $"{DiscountPercent}%";
    }
}
