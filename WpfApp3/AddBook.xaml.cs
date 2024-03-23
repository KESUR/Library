using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Library
{
    public partial class AddBook : Page
    {
        private string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

        public AddBook()
        {
            InitializeComponent();
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            string author = AuthorTextBox.Text;
            string isbn = ISBNTextBox.Text;
            string title = TitleTextBox.Text;
            bool availability = AvailabilityCheckBox.IsChecked ?? false;

            if (string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Wszystkie pola są wymagane.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                {
                    conn.Open();

                    // Sprawdź, czy książka o podanym ISBN już istnieje
                    string checkBookQuery = "SELECT COUNT(*) FROM książki WHERE ISBN = @isbn";
                    using (MySqlCommand checkBookCmd = new MySqlCommand(checkBookQuery, conn))
                    {
                        checkBookCmd.Parameters.AddWithValue("@isbn", isbn);

                        int existingBooksCount = Convert.ToInt32(checkBookCmd.ExecuteScalar());

                        if (existingBooksCount > 0)
                        {
                            MessageBox.Show("Książka o podanym ISBN już istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    int authorID;

                    // Dodaj nowego autora jeśli nie istnieje
                    string checkAuthorQuery = "SELECT ID FROM autorzy WHERE firstName = @author";
                    using (MySqlCommand checkAuthorCmd = new MySqlCommand(checkAuthorQuery, conn))
                    {
                        checkAuthorCmd.Parameters.AddWithValue("@author", author);

                        object result = checkAuthorCmd.ExecuteScalar();
                        if (result == null)
                        {
                            // Autor nie istnieje, dodaj nowego
                            string addAuthorQuery = "INSERT INTO autorzy (firstName, lastName) VALUES (@firstName, @lastName)";
                            using (MySqlCommand addAuthorCmd = new MySqlCommand(addAuthorQuery, conn))
                            {
                                string[] authorNames = author.Split(' ');
                                addAuthorCmd.Parameters.AddWithValue("@firstName", authorNames[0]);
                                addAuthorCmd.Parameters.AddWithValue("@lastName", authorNames.Length > 1 ? authorNames[1] : "");

                                addAuthorCmd.ExecuteNonQuery();

                                // Pobierz ID dodanego autora
                                authorID = (int)addAuthorCmd.LastInsertedId;
                            }
                        }
                        else
                        {
                            // Autor istnieje, pobierz jego ID
                            authorID = (int)result;
                        }
                    }

                    // Dodaj nową książkę
                    string addBookQuery = "INSERT INTO książki (ISBN, title, authorID, availability) VALUES (@isbn, @title, @authorID, @availability)";
                    using (MySqlCommand addBookCmd = new MySqlCommand(addBookQuery, conn))
                    {
                        addBookCmd.Parameters.AddWithValue("@isbn", isbn);
                        addBookCmd.Parameters.AddWithValue("@title", title);
                        addBookCmd.Parameters.AddWithValue("@authorID", authorID);
                        addBookCmd.Parameters.AddWithValue("@availability", availability);

                        int rowsAffected = addBookCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Książka została pomyślnie dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                            ClearFields();
                        }
                        else
                        {
                            MessageBox.Show("Nie udało się dodać książki.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            AuthorTextBox.Clear();
            ISBNTextBox.Clear();
            TitleTextBox.Clear();
            AvailabilityCheckBox.IsChecked = false;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private const int MaxISBNLength = 13;

        private void ISBNTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Pobierz aktualny tekst z TextBox
            string currentText = ISBNTextBox.Text;

            // Usuń wszystkie znaki niebędące cyframi
            string numericText = new string(currentText.Where(char.IsDigit).ToArray());

            // Jeśli liczba cyfr wynosi dokładnie 13
            if (numericText.Length == MaxISBNLength)
            {
                // Dezaktywuj możliwość wpisywania dalszych znaków
                ISBNTextBox.AcceptsReturn = true;
            }
            else
            {
                // Aktywuj możliwość wpisywania dalszych znaków
                ISBNTextBox.IsReadOnly = false;

                // Dodaj myslniki w odpowiednich miejscach
                StringBuilder formattedText = new StringBuilder();

                for (int i = 0; i < numericText.Length; i++)
                {
                    if (i == 2 || i == 5 || i == 9 || i == 11)
                    {
                        formattedText.Append("-");
                    }

                    formattedText.Append(numericText[i]);
                }

                // Ustaw sformatowany tekst z powrotem w TextBox
                ISBNTextBox.Text = formattedText.ToString();

                // Ustaw kursor na koniec TextBox
                ISBNTextBox.CaretIndex = ISBNTextBox.Text.Length;
            }
        }
    }
}
