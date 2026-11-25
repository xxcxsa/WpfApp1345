
using System;
using System.Windows;
using System.Windows.Controls;

using WpfApp1345.Models.MasterFloor;
using WpfApp1345.Services;
using WpfApp1345.Windows;

namespace WpfApp1345.Pages
{
    public partial class PartnersPage : UserControl
    {
        public PartnersPage()
        {
            InitializeComponent();

            LoadPartners();
        }

        private void LoadPartners()
        {
            try
            {
                partnersListBox.ItemsSource = null;

                var partners = PartnerService.GetPartnersForView();

                partnersListBox.ItemsSource = partners;

                partnersCountText.Text =
                    $"Всего партнёров: {partners.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить список партнёров.\n\n" +
                    "Проверьте подключение к базе MasterFloor.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AddPartnerButton_Click(
            object sender, RoutedEventArgs e)
        {
            OpenPartnerForm(null);
        }

        private void EditPartnerButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element ||
                element.Tag is not int partnerId)
            {
                MessageBox.Show(
                    "Не удалось определить выбранного партнёра.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            OpenPartnerForm(partnerId);
        }

        private void OpenPartnerForm(int? partnerId)
        {
            try
            {
                PartnerFormWindow window =
                    new PartnerFormWindow(partnerId)
                    {
                        Owner = Window.GetWindow(this)
                    };

                bool? result = window.ShowDialog();

                if (result == true)
                {
                    LoadPartners();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось открыть форму партнёра.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void HistoryButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (sender is not FrameworkElement element ||
                element.Tag is not int partnerId)
            {
                MessageBox.Show(
                    "Не удалось определить выбранного партнёра.",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                PartnerHistoryWindow window =
                    new PartnerHistoryWindow(partnerId)
                    {
                        Owner = Window.GetWindow(this)
                    };

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось открыть историю продаж.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void MaterialCalculatorButton_Click(
            object sender, RoutedEventArgs e)
        {
            MaterialCalculatorWindow window =
                new MaterialCalculatorWindow
                {
                    Owner = Window.GetWindow(this)
                };

            window.ShowDialog();
        }

        private void RefreshButton_Click(
            object sender, RoutedEventArgs e)
        {
            LoadPartners();
        }
    }
}
