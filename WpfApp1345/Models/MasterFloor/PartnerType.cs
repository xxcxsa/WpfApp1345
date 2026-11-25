
using System.Collections.Generic;

namespace WpfApp1345.Models.MasterFloor
{
    public class PartnerType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<Partner> Partners { get; set; }
            = new List<Partner>();
    }
}
