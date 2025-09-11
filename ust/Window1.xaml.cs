using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ust
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Users.SelectedItem is Users selectedUser)
            {
                using (var context = new hotel_managementEntities())
                {
                    var users = await context.Users.FindAsync(selectedUser.id);
                    if (users != null)
{
                        users.IsLocked = false;
                        users.LastLoginDate = null;
                        await context.SaveChangesAsync();
                        LoadUsers();
                        MessageBox.Show("Пользователь разблокирован", "Ура!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show ("Выберите пользователя", "грусть", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
