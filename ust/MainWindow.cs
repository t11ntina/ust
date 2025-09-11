using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ust;

namespace ust
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private async Task Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {

                MessageBox.Show("Пожалуйста, введите логин и пароль.");
            }
            using (var context = new УстюжанинаEntities())
            {
                var user = await context.users
                .Where(u => u.username == username)
                .FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Неправильный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.IsLocked.HasValue && user.IsLocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь, пожалуйста, к администратору.", "Доступ запрещен.", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.LastLoginDate.HasValue && (DateTime.Now - user.LastLoginDate.Value).TotalDays > 30 && user.role != "Admin")
                {
                    user.IsLocked = true;
                    await ContextBoundObject.SaveChangeAsync();
                    MessageBox.Show("Ваша учетная запись заблокрована из-за длительного отсутствия", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (username.password == password)
                {
                    user.LastLoginDate = DateTime.Now;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы успешно авторизовались", "Добро пожаловать!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (user.firstlogin.HasValue && user.firstlogin.Value)
                {
                    ChangePassword changePasswordWindow = new ChangePassword(user.id);
                    changePasswordWindow Owner = this;
                    changePasswordWindow.ShowDialog();
                }
                else
                {
                    if (user.role == "Admin")
                    {
                        Admin adminWindow = new Admin();
                        adminWindow.Show;
                    }
                    else
                    {
                        MainWindow userWindow = new MainWindow();
                        userWindow.Show;
                    }
                    this.Close();
                }
            
            else
            {
                user.FailedLoginAttempts++;
                if(user.FailedLoginAttempts == 3)
                {
                    user.IsLocked = true;
                    MessageBox.Show("Вы заблокированы после 3 неудачных попыток входа", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                await context.SaveChangesAsync();
            }
        }

    }
}

