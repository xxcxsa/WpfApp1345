
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using Microsoft.Win32;

using WpfApp1345.Models;
using WpfApp1345.Services;

namespace WpfApp1345.Windows
{
    public partial class EditWindow : Window
    {
        private readonly Phone _tempPhone;

        private readonly string _originalImagePath;

        private string? _selectedImagePath;

        public EditWindow(Phone phone)
        {
            InitializeComponent();

            _tempPhone = phone ??
                throw new ArgumentNullException(nameof(phone));

            _originalImagePath = phone.ImageSource;

            companyView.ItemsSource =
                DatabaseControl.GetCompanies();

            titleView.Text = phone.Title;

            companyView.SelectedValue = phone.CompanyId;

            priceView.Text =
                phone.Price.ToString(
                    CultureInfo.CurrentCulture);

            definitionView.Text = phone.Definition;

            if (!string.IsNullOrWhiteSpace(phone.ImageSource) &&
                phone.ImageSource != "No image")
            {
                selectedImageView.Text =
                    "Текущее изображение: " +
                    Path.GetFileName(phone.ImageSource);
            }
            else
            {
                selectedImageView.Text =
                    "Изображение не выбрано";
            }
        }

        // Выбор нового изображения.
        private void SelectImageButton_Click(
            object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Выберите новое изображение телефона",

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
                    "Новое изображение: " +
                    Path.GetFileName(dialog.FileName);
            }
        }

        // Сохранение изменений.
        private async void EditPhoneButton_Click(
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

            string? newImagePath = null;
            bool phoneSaved = false;

            try
            {
                string imagePath = _originalImagePath;

                // Если пользователь выбрал новое изображение,
                // копируем его в папку приложения.
                if (!string.IsNullOrWhiteSpace(_selectedImagePath))
                {
                    newImagePath =
                        PhoneImageService.CopyImage(_selectedImagePath);

                    imagePath = newImagePath;
                }

                // Создаём объект с обновлёнными данными.
                Phone updatedPhone = new Phone
                {
                    Id = _tempPhone.Id,
                    Title = titleView.Text.Trim(),
                    CompanyId = companyId,
                    Price = price,
                    Definition = definitionView.Text.Trim(),
                    ImageSource = imagePath
                };

                DatabaseControl.UpdatePhone(updatedPhone);

                phoneSaved = true;

                // Удаляем старое изображение только после
                // успешного сохранения нового пути в БД.
                if (newImagePath != null)
                {
                    try
                    {
                        PhoneImageService.DeleteIfManaged(
                            _originalImagePath);
                    }
                    catch (Exception imageError)
                    {
                        MessageBox.Show(
                            "Изменения сохранены, но старое изображение " +
                            "не удалось удалить:\n" + imageError.Message);
                    }
                }

                if (Owner is MainWindow mainWindow)
                {
                    await mainWindow.RefreshTableAsync();
                }

                MessageBox.Show(
                    "Изменения успешно сохранены!",
                    "Редактирование",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
            }
            catch (Exception ex)
            {
                // Если БД не обновилась, удаляем только
                // новую копию изображения.
                if (!phoneSaved && newImagePath != null)
                {
                    try
                    {
                        PhoneImageService.DeleteIfManaged(
                            newImagePath);
                    }
                    catch
                    {
                        // Не скрываем основную ошибку.
                    }
                }

                MessageBox.Show(
                    "Ошибка редактирования телефона:\n" + ex.Message,
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
