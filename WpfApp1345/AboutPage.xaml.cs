
using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WpfApp1345.Models;

namespace WpfApp1345
{
    public partial class AboutPage : Page
    {
        private readonly Phone _phone;

        public AboutPage(Phone phone)
        {
            InitializeComponent();

            _phone = phone ??
                throw new ArgumentNullException(nameof(phone));

            // Название телефона.
            titlePageView.Text = _phone.Title;

            // Производитель.
            companyPageView.Text =
                _phone.CompanyEntity?.Title ?? "Не указан";

            // Цена.
            pricePageView.Text =
                _phone.Price.ToString(
                    "N2",
                    CultureInfo.CurrentCulture) + " руб.";

            // Описание.
            definitionPageView.Text =
                string.IsNullOrWhiteSpace(_phone.Definition)
                    ? "Описание отсутствует."
                    : _phone.Definition;

            // Загружаем изображение.
            LoadPhoneImage();
        }

        // Безопасная загрузка изображения.
        private void LoadPhoneImage()
        {
            string path = _phone.ImageSource;

            noImagePanel.Visibility = Visibility.Visible;

            if (string.IsNullOrWhiteSpace(path) ||
                path == "No image" ||
                !File.Exists(path))
            {
                return;
            }

            try
            {
                BitmapImage bitmap = new BitmapImage();

                using (FileStream stream = File.OpenRead(path))
                {
                    bitmap.BeginInit();

                    // Загружаем изображение в память,
                    // чтобы файл не оставался заблокированным.
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;

                    bitmap.StreamSource = stream;

                    bitmap.EndInit();

                    bitmap.Freeze();
                }

                imagePageView.Source = bitmap;

                noImagePanel.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить изображение:\n" + ex.Message);
            }
        }

        // Возврат к главному окну.
        private void BackButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (FrameContext.MainWindowFrame != null)
            {
                FrameContext.MainWindowFrame.Content = null;

                FrameContext.MainWindowFrame.Visibility =
                    Visibility.Collapsed;
            }
        }
    }
}
