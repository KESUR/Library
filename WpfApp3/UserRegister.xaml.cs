using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Security.Cryptography;

namespace Library
{
    public partial class UserRegister : Window
    {
        string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

        public UserRegister()
        {
            InitializeComponent();
        }

        private bool isEmployee = false;

        public UserRegister(bool isEmp)
        {
            isEmployee = isEmp;
            InitializeComponent();
            txtLogin.IsEnabled = false;
            txtPassword.IsEnabled = false;
            LoginBtn.IsEnabled = false;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtRegisterUsername.Text.Trim();
            string password = txtRegisterPassword.Password.Trim();
            string prefixNumber = PrefixNumberCB.Text.Trim();
            string phoneNumber = prefixNumber + " " + txtPhoneNumber.Text.Trim();

            if (fullName == "" || phoneNumber == "" || username == "" || password == "")
            {
                MessageBox.Show("Wpisz wszystkie potrzebne dane!");
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                    {
                        conn.Open();

                        // Sprawdzenie, czy użytkownik o podanej nazwie już istnieje
                        string checkUserQuery = "SELECT COUNT(*) FROM użytkownik WHERE username = @username";
                        using (MySqlCommand checkUserCmd = new MySqlCommand(checkUserQuery, conn))
                        {
                            checkUserCmd.Parameters.AddWithValue("@username", username);

                            int userCount = Convert.ToInt32(checkUserCmd.ExecuteScalar());

                            if (userCount > 0)
                            {
                                MessageBox.Show("Użytkownik o podanej nazwie już istnieje!");
                                return;
                            }
                        }

                        // Wprowadzenie oryginalnego hasła do bazy danych (bez haszowania)
                        string insertQuery = "INSERT INTO użytkownik (fullName, phoneNumber, username, password) VALUES (@fullName, @phoneNumber, @username, @password)";
                        using (MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@fullName", fullName);
                            insertCmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                            insertCmd.Parameters.AddWithValue("@username", username);
                            insertCmd.Parameters.AddWithValue("@password", password);

                            int rowsAffected = insertCmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Zarejestrowano pomyślnie");
                                conn.Close();
                                if (isEmployee == false)
                                {
                                    NewWindow newWindow = new NewWindow();
                                    newWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    this.Close();
                                }
                                return;
                            }
                            else
                            {
                                MessageBox.Show("Nie udało się zarejestrować użytkownika!");
                                conn.Close();
                                return;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
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
                        string fullName = "";
                        string phoneNumber = "";
                        string query = "SELECT username, password, fullName, phoneNumber FROM użytkownik WHERE username=@username LIMIT 1";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@username", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader.GetString("password");
                                string hashedPassword = HashPassword(password);

                                if (storedPasswordHash == hashedPassword)
                                {
                                    fullName = reader.GetString("fullName");
                                    phoneNumber = reader.GetString("phoneNumber");
                                    MessageBox.Show("Zalogowano pomyślnie");
                                    conn.Close();
                                    User user = new User()
                                    {
                                        userName = username,
                                        password = password,
                                        phoneNumber = phoneNumber,
                                        fullName = fullName
                                    };
                                    NewWindow newWindow = new NewWindow(user, isEmployee);
                                    newWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Niepoprawne hasło!");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Użytkownik o podanej nazwie nie istnieje!");
                            }
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

        private void txtFullName_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtFullName.Text == "Full Name")
            {
                txtFullName.Text = "";
            }
        }

        private void txtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtPhoneNumber.Text == "Phone number")
            {
                txtPhoneNumber.Text = "";
            }
        }

        private void txtRegisterUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtRegisterUsername.Text == "Username")
            {
                txtRegisterUsername.Text = "";
            }
        }

        private void txtRegisterPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtRegisterPassword.Password == "Password")
            {
                txtRegisterPassword.Password = "";
            }
        }

        private void txtRegisterUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtLogin_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
