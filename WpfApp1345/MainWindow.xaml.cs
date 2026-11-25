
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

using WpfApp1345.Data;
using WpfApp1345.Models;
using WpfApp1345.Services;
using WpfApp1345.Windows;

namespace WpfApp1345
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Phone> phones = new();

        private ObservableCollection<Company> companies = new();

        public MainWindow()
        {
            InitializeComponent();

            FrameContext.MainWindowFrame = mainFrame;

            mainListBox.ItemsSource = phones;
            mainDataGridView.ItemsSource = phones;

            Loaded += MainWindow_Loaded;
        }

        // Загрузка данных при запуске приложения.
        private async void MainWindow_Loaded(
            object sender, RoutedEventArgs e)
        {
            try
            {
                await InitializeDatabaseAsync();

                RefreshEmployeesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки данных:\n" + ex.Message);
            }
        }

        // Первоначальная настройка базы телефонов.
        private async Task InitializeDatabaseAsync()
        {
            await using (var db = new DbAppContext())
            {
                await db.Database.MigrateAsync();

                if (!await db.Companies.AnyAsync())
                {
                    db.Companies.AddRange(
                        new Company
                        {
                            Title = "Apple",
                            CEO = "Tim Cook",
                            Capital = 1000000
                        },
                        new Company
                        {
                            Title = "Xiaomi",
                            CEO = "Lei Jun",
                            Capital = 500000
                        },
                        new Company
                        {
                            Title = "Nokia",
                            CEO = "Management",
                            Capital = 300000
                        }
                    );

                    await db.SaveChangesAsync();
                }
            }

            await RefreshTableAsync();
        }

        // Обновление телефонов.
        public async Task RefreshTableAsync()
        {
            await using var db = new DbAppContext();

            companies = new ObservableCollection<Company>(
                await db.Companies
                    .OrderBy(c => c.Title)
                    .ToListAsync()
            );

            companyView.ItemsSource = companies;

            phones = new ObservableCollection<Phone>(
                await db.Phones
                    .Include(p => p.CompanyEntity)
                    .OrderBy(p => p.Id)
                    .ToListAsync()
            );

            mainListBox.ItemsSource = phones;
            mainDataGridView.ItemsSource = phones;
        }

        // =========================================
        // ТЕЛЕФОНЫ
        // =========================================

        // Добавление телефона через первую вкладку.
        private async void AddButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(titleView.Text) ||
                string.IsNullOrWhiteSpace(priceView.Text) ||
                companyView.SelectedItem is not Company company ||
                !decimal.TryParse(
                    priceView.Text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out decimal price) ||
                price < 0)
            {
                MessageBox.Show(
                    "Введите корректные данные телефона!");

                return;
            }

            try
            {
                await using var db = new DbAppContext();

                Phone phone = new Phone
                {
                    Title = titleView.Text.Trim(),
                    Price = price,
                    CompanyId = company.Id
                };

                db.Phones.Add(phone);

                await db.SaveChangesAsync();

                await RefreshTableAsync();

                ClearFields();

                MessageBox.Show("Телефон успешно добавлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка добавления:\n" + ex.Message);
            }
        }

        // Удаление телефона через первую вкладку.
        private async void RemoveButton_Click(
            object sender, RoutedEventArgs e)
        {
            if (mainListBox.SelectedItem is not Phone selectedPhone)
            {
                MessageBox.Show("Выберите телефон для удаления!");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Удалить телефон «{selectedPhone.Title}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await using var db = new DbAppContext();

                Phone? phone =
                    await db.Phones.FindAsync(selectedPhone.Id);

                if (phone != null)
                {
                    string imagePath = phone.ImageSource;

                    db.Phones.Remove(phone);

                    await db.SaveChangesAsync();

                    TryDeletePhoneImage(imagePath);
                }

                await RefreshTableAsync();

                ClearFields();

                MessageBox.Show("Телефон удалён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления:\n" + ex.Message);
            }
        }

        // Выбор телефона в ListBox.
        private void mainListBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            if (mainListBox.SelectedItem is Phone phone)
            {
                titleView.Text = phone.Title;

                priceView.Text = phone.Price.ToString(
                    CultureInfo.CurrentCulture);

                companyView.SelectedValue = phone.CompanyId;
            }
        }

        // Открытие окна добавления телефона.
        private void OpenAddWindow_Click(
            object sender, RoutedEventArgs e)
        {
            AddWindow window = new AddWindow
            {
                Owner = this
            };

            window.ShowDialog();
        }

        // Редактирование телефона.
        private void EditButton_Click(
            object sender, RoutedEventArgs e)
        {
            Phone? phone = null;

            if (sender is FrameworkElement element &&
                element.DataContext is Phone rowPhone)
            {
                phone = rowPhone;
            }
            else
            {
                phone = mainDataGridView.SelectedItem as Phone;
            }

            if (phone == null)
            {
                MessageBox.Show(
                    "Выберите телефон для редактирования!");

                return;
            }

            mainDataGridView.SelectedItem = phone;

            EditWindow window = new EditWindow(phone)
            {
                Owner = this
            };

            window.ShowDialog();
        }

        // Удаление телефона из DataGrid.
        private async void DeleteDataGridButton_Click(
            object sender, RoutedEventArgs e)
        {
            Phone? phone = null;

            if (sender is FrameworkElement element &&
                element.DataContext is Phone rowPhone)
            {
                phone = rowPhone;
            }
            else
            {
                phone = mainDataGridView.SelectedItem as Phone;
            }

            if (phone == null)
            {
                MessageBox.Show("Выберите телефон для удаления!");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Удалить телефон «{phone.Title}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await using var db = new DbAppContext();

                Phone? existingPhone =
                    await db.Phones.FindAsync(phone.Id);

                if (existingPhone != null)
                {
                    string imagePath = existingPhone.ImageSource;

                    db.Phones.Remove(existingPhone);

                    await db.SaveChangesAsync();

                    TryDeletePhoneImage(imagePath);
                }

                await RefreshTableAsync();

                MessageBox.Show("Телефон удалён!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка удаления:\n" + ex.Message);
            }
        }

        // Безопасное удаление файла изображения.
        private void TryDeletePhoneImage(string? imagePath)
        {
            try
            {
                PhoneImageService.DeleteIfManaged(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Телефон удалён из базы, но изображение " +
                    "не удалось удалить:\n" + ex.Message);
            }
        }

        // Выбор строки правой кнопкой мыши.
        private void DataGridRow_PreviewMouseRightButtonDown(
            object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.IsSelected = true;
                mainDataGridView.SelectedItem = row.Item;
                row.Focus();
            }
        }

        // Открытие страницы подробной информации.
        private void AboutMenuItem_Click(
            object sender, EventArgs e)
        {
            Phone? phone = null;

            if (sender is DataGridRow row)
            {
                phone = row.Item as Phone;
            }
            else
            {
                phone = mainDataGridView.SelectedItem as Phone;
            }

            if (phone == null)
            {
                MessageBox.Show("Выберите телефон!");
                return;
            }

            mainFrame.Navigate(new AboutPage(phone));

            mainFrame.Visibility = Visibility.Visible;
        }

        // =========================================
        // СОТРУДНИКИ
        // =========================================

        // Первоначальная загрузка списка сотрудников.
        private void RefreshEmployeesList()
        {
            try
            {
                employeeListBox.ItemsSource = null;

                employeeListBox.ItemsSource =
                    DatabaseControl.GetEmployeesList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки сотрудников из EmpDB:\n" +
                    ex.Message);
            }
        }

        // Обновление списка сотрудников по кнопке.
        private void RefreshListButton_Click(
            object sender, RoutedEventArgs e)
        {
            RefreshEmployeesList();

            MessageBox.Show(
                "Список сотрудников обновлён!",
                "Обновление",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // =========================================
        // ОБЩИЕ МЕТОДЫ
        // =========================================

        // Очистка полей телефона.
        private void ClearFields()
        {
            titleView.Clear();

            priceView.Clear();

            companyView.SelectedIndex = -1;

            mainListBox.SelectedItem = null;

            mainDataGridView.SelectedItem = null;
        }
    }
}
