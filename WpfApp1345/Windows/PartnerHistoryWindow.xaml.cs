
using System;
using System.Linq;
using System.Windows;

using WpfApp1345.Services;

namespace WpfApp1345.Windows
{
    public partial class PartnerHistoryWindow : Window
    {
        public PartnerHistoryWindow(int partnerId)
        {
            InitializeComponent();

            try
            {
                // Получаем выбранного партнёра.
                var partner =
                    PartnerService.GetPartnerById(partnerId);

                partnerNameText.Text =
                    "История продаж: " + partner.Name;

                // Получаем историю продаж только этого партнёра.
                var history =
                    PartnerHistoryService.GetHistory(partnerId);

                historyDataGrid.ItemsSource = history;

                // Формируем итоговую информацию.
                int totalQuantity =
                    history.Sum(sale => sale.Quantity);

                salesCountText.Text =
                    $"Количество записей: {history.Count}";

                salesQuantityText.Text =
                    $"Общее количество продукции: {totalQuantity:N0}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить историю продаж.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();
            }
        }

        // Возврат к списку партнёров.
        private void BackButton_Click(
            object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
