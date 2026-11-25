

namespace WpfApp1345.Models
{
    public class Phone
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public decimal Price { get; set; }

        // Описание телефона
        public string Definition { get; set; } = string.Empty;

        // Путь к изображению телефона.
        // "No image" означает, что изображение ещё не выбрано.
        public string ImageSource { get; set; } = "No image";

        public Company CompanyEntity { get; set; } = null!;
    }
}
