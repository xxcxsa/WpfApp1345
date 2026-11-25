
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Win32;

using WpfApp1345.Models;
using WpfApp1345.Services;

namespace WpfApp1345.Windows
{
    public partial class AddWindow : Window
    {
        private string? _selectedImagePath;

        public AddWindow()
        {
            InitializeComponent();

            companyView.ItemsSource =
                DatabaseControl.GetCompanies();
        }

        // Выбор изображения.
        private void SelectImageButton_Click(
            object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Выберите изображение телефона",

                Filter =
                    "Изображения (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|" +
                    "*.jpg;*.jpeg;*.png;*.bmp;*.gif|" +
                    "Все файлы (*.*)|*.*",

                Multiselect = false,
                CheckFileExists = true
            };

            if (dialog.ShowDialog(this) == true)
            {
                _selectedImagePath = dialog.FileName;

                selectedImageView.Text =
                    Path.GetFileName(dialog.FileName);
            }
        }

        // Добавление телефона.
        private async void AddPhoneButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleView.Text) ||
                string.IsNullOrWhiteSpace(definitionView.Text) ||
                companyView.SelectedValue is not int companyId ||
                !decimal.TryParse(
                    priceView.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal price) ||
                price < 0)
            {
                MessageBox.Show(
                    "Заполните все поля корректно. " +
                    "Цена не может быть отрицательной.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string? copiedImagePath = null;
            bool phoneSaved = false;

            try
            {
                string imagePath = "No image";

                // Если изображение выбрано, копируем его в папку проекта.
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    copiedImagePath =
                        PhoneImageService.CopyImage(_selectedImagePath);

                    imagePath = copiedImagePath;
                }

                Phone phone = new Phone
                {
                    Title = titleView.Text.Trim(),
                    CompanyId = companyId,
                    Price = price,
                    Definition = definitionView.Text.Trim(),
                    ImageSource = imagePath
                };

                DatabaseControl.AddPhone(phone);

                phoneSaved = true;

                if (Owner is MainWindow mainWindow)
                {
                    await mainWindow.RefreshTableAsync();
                }

                MessageBox.Show(
                    "Телефон успешно добавлен!",
                    "Добавление",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                // Убираем скопированную картинку,
                // только если сохранение телефона не состоялось.
                if (!phoneSaved && copiedImagePath != null)
                {
                    try
                    {
                        PhoneImageService.DeleteIfManaged(
                            copiedImagePath);
                    }
                    catch
                    {
                        // Не скрываем основную ошибку.
                    }
                }

                MessageBox.Show(
                    "Ошибка добавления телефона:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Отмена.
        private void CancelButton_Click(
            object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
