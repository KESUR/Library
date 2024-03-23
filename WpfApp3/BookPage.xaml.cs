using MySql.Data.MySqlClient;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Library
{
        public partial class BookPage : Page
        {
            private string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

            public ObservableCollection<Book> BooksList { get; set; }

            public BookPage()
            {
                InitializeComponent();
                BooksList = new ObservableCollection<Book>();
                LoadData();
            }

            public class Book
            {
                public int Id { get; set; }
                public string ?Tytul { get; set; }
                public string ?Autor { get; set; }
                public string ?Isbn { get; set; }
            }

            private void LoadData()
            {
                try
                {
                    using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                    {
                        conn.Open();

                    string query = "SELECT książki.ID, książki.title AS Tytul, CONCAT(autorzy.firstName, ' ', autorzy.lastName) AS Autor, książki.ISBN " +
                                    "FROM książki " +
                                    "INNER JOIN autorzy ON książki.authorID = autorzy.ID";


                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                            {
                                DataTable table = new DataTable();
                                adapter.Fill(table);

                                BooksList.Clear();

                                foreach (DataRow row in table.Rows)
                                {
                                    BooksList.Add(new Book
                                    {
                                        Id = Convert.ToInt32(row["Id"]),
                                        Tytul = row["Tytul"].ToString(),
                                        Autor = row["Autor"].ToString(),
                                        Isbn = row["Isbn"].ToString()
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

                lstBooks.ItemsSource = BooksList;
            }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            AddBook addbook = new AddBook();
            this.NavigationService.Navigate(addbook);
        }

        private void DelBook_Click(object sender, RoutedEventArgs e)
        {
            if (lstBooks.SelectedItem != null && lstBooks.SelectedItem is Book selectedBook)
            {
                MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz usunąć książkę {selectedBook.Tytul}?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                        {
                            conn.Open();

                            string checkBorrowingsQuery = "SELECT COUNT(*) FROM książki WHERE title = @title";
                            using (MySqlCommand checkBorrowingsCmd = new MySqlCommand(checkBorrowingsQuery, conn))
                            {
                                checkBorrowingsCmd.Parameters.AddWithValue("@title", selectedBook.Id);

                                int borrowingCount = Convert.ToInt32(checkBorrowingsCmd.ExecuteScalar());

                                if (borrowingCount > 0)
                                {
                                    MessageBox.Show("Nie można usunąć książki, ponieważ istnieją zależne wypożyczenia.");
                                    return;
                                }
                            }

                            string deleteBookQuery = "DELETE FROM książki WHERE id = @title";
                            using (MySqlCommand deleteCmd = new MySqlCommand(deleteBookQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@title", selectedBook.Id);

                                int rowsAffected = deleteCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Książka została pomyślnie usunięta.");
                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("Nie udało się usunąć książki.");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Wybierz książkę do usunięcia.");
            }
        }

        private void KsiazkaTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (SearchBooktxt.Text == "Wyszukaj...")
            {
                SearchBooktxt.Text = "";
            }
        }

        private void KsiazkaTextBox(string searchTerm)
        {
            if (BooksList == null || BooksList.Count == 0)
            {
                // Jeśli lista czytelników jest pusta, nie ma potrzeby kontynuowania wyszukiwania
                return;
            }

            string lowerSearchTerm = searchTerm.ToLower();

            List<Book> searchResults = BooksList
                .Where(book =>
                    (book.Tytul != null && book.Tytul.ToLower().Contains(lowerSearchTerm)))
                .ToList();

            lstBooks.ItemsSource = new ObservableCollection<Book>(searchResults);
        }

        private void KsiazkaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchBooktxt.Text;

            // Sprawdź, czy wpisano jakieś dane
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Jeśli pole wyszukiwania jest puste, załaduj wszystkich czytelników
                lstBooks.ItemsSource = BooksList;
            }
            else
            {
                // W przeciwnym razie, przeprowadź wyszukiwanie po nazwie tytule
                KsiazkaTextBox(searchTerm);
            }
        }
    }
}

