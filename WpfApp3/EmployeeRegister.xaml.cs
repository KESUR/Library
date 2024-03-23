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
    public partial class EmployeeRegister : Window
    {
        string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";
        public EmployeeRegister()
        {
            InitializeComponent();
        }

        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        public void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("Wpisz login i hasło!");
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                    {
                        conn.Open();
                        string query = "SELECT username, password FROM pracownik WHERE username=@username AND password=@password LIMIT 1";
                        MySqlDataAdapter ada = new MySqlDataAdapter(query, conn);
                        ada.SelectCommand.Parameters.AddWithValue("@username", username);
                        ada.SelectCommand.Parameters.AddWithValue("@password", password);

                        DataTable table = new DataTable();
                        ada.Fill(table);

                        if (table.Rows.Count > 0)
                        {
                            MessageBox.Show("Zalogowano pomyślnie");
                            conn.Close();
                            NewWindow newWindow = new NewWindow();
                            newWindow.Show();
                            this.Close();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Niepoprawne dane!");
                            conn.Close();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtLogin.Text == "Username")
            {
                txtLogin.Text = "";
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtPassword.Password == "Password")
            {
                txtPassword.Password = "";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}