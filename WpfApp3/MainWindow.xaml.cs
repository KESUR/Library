using MySql.Data.MySqlClient;
using MySql.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp3;
using System.Data;

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserRegister userRegister = new UserRegister();
            userRegister.Show();
            this.Close();
        }

        private void EmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeRegister employeeRegister = new EmployeeRegister();
            employeeRegister.Show();
            this.Close();
        }
    }
}