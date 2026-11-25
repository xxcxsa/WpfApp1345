
using System.Collections.Generic;

namespace WpfApp1345.Models.MasterFloor
{
    public class Partner
    {
        public int Id { get; set; }

        public int PartnerTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Director { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string LegalAddress { get; set; } = string.Empty;

        public string INN { get; set; } = string.Empty;

        public int Rating { get; set; }

        public PartnerType PartnerType { get; set; } = null!;

        public ICollection<PartnerSale> Sales { get; set; }
            = new List<PartnerSale>();
    }
}
