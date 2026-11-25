
using System;
using System.Globalization;
using System.Windows;

using WpfApp1345.Services;

namespace WpfApp1345.Windows
{
    public partial class MaterialCalculatorWindow : Window
    {
        public MaterialCalculatorWindow()
        {
            InitializeComponent();

            try
            {
                productTypeComboBox.ItemsSource =
                    MaterialCalculatorService.GetProductTypes();

                materialTypeComboBox.ItemsSource =
                    MaterialCalculatorService.GetMaterialTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить справочники материалов.\n\n" +
                    "Проверьте подключение к MasterFloor.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();
            }
        }

        // Разбор положительного вещественного числа.
        private bool TryParsePositiveDouble(
            string value,
            out double result)
        {
            bool parsed = double.TryParse(
                value,
                NumberStyles.Float,
                CultureInfo.CurrentCulture,
                out result);

            if (!parsed)
            {
                parsed = double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out result);
            }

            return parsed &&
                   !double.IsNaN(result) &&
                   !double.IsInfinity(result) &&
                   result > 0;
        }

        // Расчёт количества материала.
        private void CalculateButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (productTypeComboBox.SelectedValue is not int productTypeId)
            {
                ShowError("Выберите тип продукции.");
                return;
            }

            if (materialTypeComboBox.SelectedValue is not int materialTypeId)
            {
                ShowError("Выберите тип материала.");
                return;
            }

            if (!int.TryParse(
                quantityTextBox.Text.Trim(),
                out int quantity) ||
                quantity <= 0)
            {
                ShowError(
                    "Количество изделий должно быть положительным целым числом.");

                return;
            }

            if (!TryParsePositiveDouble(
                widthTextBox.Text.Trim(),
                out double width))
            {
                ShowError(
                    "Введите положительную ширину изделия. " +
                    "Пример: 1,5.");

                return;
            }

            if (!TryParsePositiveDouble(
                lengthTextBox.Text.Trim(),
                out double length))
            {
                ShowError(
                    "Введите положительную длину изделия. " +
                    "Пример: 2,5.");

                return;
            }

            int result =
                MaterialCalculatorService.CalculateRequiredMaterial(
                    productTypeId,
                    materialTypeId,
                    quantity,
                    width,
                    length);

            if (result == -1)
            {
                resultTextBlock.Text =
                    "Не удалось выполнить расчёт.";

                MessageBox.Show(
                    "Проверьте выбранные типы продукции и материала, " +
                    "а также введённые параметры.",
                    "Ошибка расчёта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            resultTextBlock.Text =
                $"Необходимое количество материала: {result:N0}";
        }

        // Сообщение об ошибке ввода.
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Проверка данных",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            resultTextBlock.Text =
                "Исправьте данные и повторите расчёт.";
        }
    }
}
