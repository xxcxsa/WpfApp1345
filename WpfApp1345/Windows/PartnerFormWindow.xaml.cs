
using System;
using System.Globalization;
using System.Net.Mail;
using System.Windows;

using WpfApp1345.Models.MasterFloor;
using WpfApp1345.Services;

namespace WpfApp1345.Windows
{
    public partial class PartnerFormWindow : Window
    {
        private readonly int? _partnerId;

        // null означает добавление, Id означает редактирование.
        public PartnerFormWindow(int? partnerId = null)
        {
            InitializeComponent();

            _partnerId = partnerId;

            try
            {
                partnerTypeComboBox.ItemsSource =
                    PartnerService.GetPartnerTypes();

                if (_partnerId.HasValue)
                {
                    LoadPartner(_partnerId.Value);

                    Title = "Редактирование партнёра";
                    formTitleText.Text = "Редактирование партнёра";
                    saveButton.Content = "Сохранить изменения";
                }
                else
                {
                    Title = "Добавление партнёра";
                    formTitleText.Text = "Добавление партнёра";

                    ratingTextBox.Text = "0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось открыть форму партнёра.\n\n" +
                    "Проверьте подключение к базе MasterFloor.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка открытия формы",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                throw;
            }
        }

        // Загружаем существующие данные в поля формы.
        private void LoadPartner(int partnerId)
        {
            Partner partner =
                PartnerService.GetPartnerById(partnerId);

            nameTextBox.Text = partner.Name;

            partnerTypeComboBox.SelectedValue =
                partner.PartnerTypeId;

            ratingTextBox.Text =
                partner.Rating.ToString(
                    CultureInfo.InvariantCulture);

            addressTextBox.Text = partner.LegalAddress;

            directorTextBox.Text = partner.Director;

            phoneTextBox.Text = partner.Phone;

            emailTextBox.Text = partner.Email;

            innTextBox.Text = partner.INN;
        }

        // Проверка электронной почты.
        private bool IsValidEmail(string email)
        {
            try
            {
                MailAddress address = new MailAddress(email);

                return string.Equals(
                    address.Address,
                    email,
                    StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        // Сохранение нового или изменённого партнёра.
        private void SaveButton_Click(
            object sender, RoutedEventArgs e)
        {
            string name = nameTextBox.Text.Trim();

            string director = directorTextBox.Text.Trim();

            string phone = phoneTextBox.Text.Trim();

            string email = emailTextBox.Text.Trim();

            string address = addressTextBox.Text.Trim();

            string inn = innTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowValidationError(
                    "Введите наименование партнёра.",
                    nameTextBox);

                return;
            }

            if (partnerTypeComboBox.SelectedValue is not int partnerTypeId)
            {
                ShowValidationError(
                    "Выберите тип партнёра.",
                    partnerTypeComboBox);

                return;
            }

            if (!int.TryParse(
                    ratingTextBox.Text.Trim(),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int rating) ||
                rating < 0)
            {
                ShowValidationError(
                    "Рейтинг должен быть целым неотрицательным числом. " +
                    "Например: 0, 5 или 10.",
                    ratingTextBox);

                return;
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                ShowValidationError(
                    "Введите юридический адрес партнёра.",
                    addressTextBox);

                return;
            }

            if (string.IsNullOrWhiteSpace(director))
            {
                ShowValidationError(
                    "Введите ФИО директора.",
                    directorTextBox);

                return;
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowValidationError(
                    "Введите контактный телефон.",
                    phoneTextBox);

                return;
            }

            if (string.IsNullOrWhiteSpace(email) ||
                !IsValidEmail(email))
            {
                ShowValidationError(
                    "Введите корректный Email компании. " +
                    "Пример: company@example.ru.",
                    emailTextBox);

                return;
            }

            if (string.IsNullOrWhiteSpace(inn))
            {
                ShowValidationError(
                    "Введите ИНН партнёра.",
                    innTextBox);

                return;
            }

            Partner partner = new Partner
            {
                Id = _partnerId ?? 0,
                PartnerTypeId = partnerTypeId,
                Name = name,
                Director = director,
                Email = email,
                Phone = phone,
                LegalAddress = address,
                INN = inn,
                Rating = rating
            };

            try
            {
                if (_partnerId.HasValue)
                {
                    PartnerService.UpdatePartner(partner);

                    MessageBox.Show(
                        "Данные партнёра успешно обновлены.",
                        "Редактирование завершено",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    PartnerService.AddPartner(partner);

                    MessageBox.Show(
                        "Новый партнёр успешно добавлен.",
                        "Добавление завершено",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                DialogResult = true;
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось сохранить данные партнёра.\n\n" +
                    "Проверьте введённые данные и подключение к базе.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка сохранения",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Единое отображение ошибок ввода.
        private void ShowValidationError(
            string message,
            System.Windows.Controls.Control control)
        {
            MessageBox.Show(
                message,
                "Проверка данных",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            control.Focus();
        }
    }
}
