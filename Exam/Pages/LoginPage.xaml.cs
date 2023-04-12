using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Exam.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private string captcha = string.Empty;
        private int failedLoginCounter = 0;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void CaptchaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CaptchaPlaceholder.Visibility = CaptchaTextBox.Text != string.Empty ? Visibility.Hidden : Visibility.Visible;
        }

        private void LoadCaptcha()
        {
            var line = new StringBuilder();
            var rand = new Random();

            const string chars = "ACBDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            for (var i = 0; i <= 6; i++)
            {
                line.Append(chars[rand.Next(chars.Length)]);
            }
            captcha = line.ToString();

            var image = new Bitmap(160, 60);
            var font = new Font("Calibri", 30, System.Drawing.FontStyle.Italic, GraphicsUnit.Pixel);
            var graphics = Graphics.FromImage(image);
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            var brushStyle = RandomEnum<HatchStyle>();
            var brush = new HatchBrush(brushStyle, Color.LightGray);
            graphics.FillRectangle(brush, new Rectangle(0, 0, 160, 60));
            var rotate = new[] { 6, -6, 7, -7, 8, -8 };
            graphics.RotateTransform(rotate[rand.Next(rotate.Length)]);
            graphics.DrawString(captcha, font, Brushes.LightYellow, new System.Drawing.Point(10, 10));
            for (var i = 0; i < 10; i++)
            {
                graphics.DrawLine(Pens.DarkSlateBlue, new System.Drawing.Point(rand.Next(0, 200), rand.Next(0, 60)), new System.Drawing.Point(rand.Next(0, 200), rand.Next(0, 60)));
            }

            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();

                CaptchaImage.Source = img;
            }

        }

        public static T RandomEnum<T>()
        {
            T[] values = (T[])Enum.GetValues(typeof(T));
            return values[new Random().Next(0, values.Length)];
        }

        private void UpdateCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCaptcha();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (captcha != CaptchaTextBox.Text.ToUpper())
            {
                MessageBox.Show("Неверно введена капча", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                failedLoginCounter++;

                if (failedLoginCounter == 3)
                {
                    BlockButton();
                    failedLoginCounter = 0;
                }
                return;
            }

            //Проверки на логин и пароль

            Manager.MainFrame.Navigate(new ItemsTablePage());
        }

        async private void BlockButton()
        {
            LoginButton.IsEnabled = false;
            await Task.Delay(2000);
            LoginButton.IsEnabled = true;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoginTextBox.Text = string.Empty;
            PasswordTextBox.Text = string.Empty;
            CaptchaTextBox.Text = string.Empty;
            LoadCaptcha();
        }
    }
}
