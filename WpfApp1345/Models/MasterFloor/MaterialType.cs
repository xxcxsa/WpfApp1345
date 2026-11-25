
namespace WpfApp1345.Models.MasterFloor
{
    public class MaterialType
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Доля брака: например, 0.001 = 0.1%.
        public decimal ScrapRate { get; set; }
    }
}
