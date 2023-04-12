using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Exam.Database;

namespace Exam
{
    /// <summary>
    /// Interaction logic for AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        public List<Country> Countries { get; set; }

        public Hotel Hotel { get; set; }

        public AddEditWindow(Hotel hotel)
        {
            InitializeComponent();

            using (var context = new TouristAgencyEntities())
            {
                Countries = context.Countries.ToList();
            }

            Hotel = hotel ?? new Hotel();
            DataContext = this;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrWhiteSpace(NameTextBox.Text.Trim()))
            {
                sb.AppendLine("Название отеля обязательно для заполнения");
            }

            if (!int.TryParse(CountOfStarsTextBox.Text, out var stars) || stars < 0 || stars > 5)
            {
                sb.AppendLine("Количество звёзд отеля - целое число от 0 до 5");
            }

            if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text.Trim()))
            {
                sb.AppendLine("Описание отеля обязательно для заполнения");
            }

            if (CountryComboBox.SelectedIndex == -1)
            {
                sb.AppendLine("Укажите страну отеля в выпадающем списке");
            }

            var errors = sb.ToString();

            if (errors.Length == 0)
            {
                Hotel.Country = Countries.Find(x => x.Code == Hotel.CountryCode);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show(errors, "При заполнении данных отеля возникли ошибки!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("^[^1-5]+$");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
