
using System.Collections.Generic;

namespace WpfApp1345.Models
{
    public class Company
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string CEO { get; set; } = string.Empty;

        public decimal Capital { get; set; }

        // Одна компания может производить много телефонов.
        public ICollection<Phone> PhoneEntities { get; set; }
            = new List<Phone>();
    }
}
