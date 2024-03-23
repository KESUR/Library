using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Library
{
    public partial class ReaderPage : Page
    {
        private string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

        public ObservableCollection<Czytelnik> CzytelnicyList { get; set; }

        public ReaderPage()
        {
            InitializeComponent();
            CzytelnicyList = new ObservableCollection<Czytelnik>();
            LoadData();
        }

        public class Czytelnik
        {
            public int Id { get; set; }
            public string? ImieNazwisko { get; set; }
            public string? NazwaUzytkownika { get; set; }
            public string? NumerTelefonu { get; set; }
            public string? WypozyczonaKsiazka { get; set; }
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                {
                    conn.Open();

                    string query = "SELECT użytkownik.id, userName, fullName, phoneNumber, książki.title AS BorrowedBookTitle " +
                                   "FROM użytkownik " +
                                   "LEFT JOIN wypożyczenia ON użytkownik.id = wypożyczenia.borrowerID " +
                                   "LEFT JOIN książki ON wypożyczenia.bookID = książki.id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            CzytelnicyList.Clear();

                            foreach (DataRow row in table.Rows)
                            {
                                CzytelnicyList.Add(new Czytelnik
                                {
                                    Id = Convert.ToInt32(row["id"]),
                                    ImieNazwisko = row["fullName"].ToString(),
                                    NazwaUzytkownika = row["userName"].ToString(),
                                    NumerTelefonu = row["phoneNumber"].ToString(),
                                    WypozyczonaKsiazka = row["BorrowedBookTitle"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            CzytelnicyListView.ItemsSource = CzytelnicyList;
        }

        private void AddReader_Click(object sender, RoutedEventArgs e)
        {
            bool isEmployee = true;
            UserRegister addReader = new UserRegister(isEmployee);
            addReader.ShowDialog();
            LoadData();
        }

        private void DelReader_Click(object sender, RoutedEventArgs e)
        {
            if (CzytelnicyListView.SelectedItem != null && CzytelnicyListView.SelectedItem is Czytelnik selectedUser)
            {
                MessageBoxResult result = MessageBox.Show($"Czy na pewno chcesz usunąć czytelnika {selectedUser.ImieNazwisko}?", "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                        {
                            conn.Open();

                            string checkBorrowingsQuery = "SELECT COUNT(*) FROM wypożyczenia WHERE borrowerID = @userId";
                            using (MySqlCommand checkBorrowingsCmd = new MySqlCommand(checkBorrowingsQuery, conn))
                            {
                                checkBorrowingsCmd.Parameters.AddWithValue("@userId", selectedUser.Id);

                                int borrowingCount = Convert.ToInt32(checkBorrowingsCmd.ExecuteScalar());

                                if (borrowingCount > 0)
                                {
                                    MessageBox.Show("Nie można usunąć czytelnika, ponieważ istnieją zależne wypożyczenia.");
                                    return;
                                }
                            }

                            string deleteUserQuery = "DELETE FROM użytkownik WHERE id = @userId";
                            using (MySqlCommand deleteCmd = new MySqlCommand(deleteUserQuery, conn))
                            {
                                deleteCmd.Parameters.AddWithValue("@userId", selectedUser.Id);

                                int rowsAffected = deleteCmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Czytelnik został pomyślnie usunięty.");
                                    LoadData();
                                }
                                else
                                {
                                    MessageBox.Show("Nie udało się usunąć czytelnika.");
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
                MessageBox.Show("Wybierz czytelnika do usunięcia.");
            }
        }

        private void SearchBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (SearchBox.Text == "Wyszukaj...")
            {
                SearchBox.Text = "";
            }
        }

        private void SearchByUserName(string searchTerm)
        {
            if (CzytelnicyList == null || CzytelnicyList.Count == 0)
            {
                // Jeśli lista czytelników jest pusta, nie ma potrzeby kontynuowania wyszukiwania
                return;
            }

            string lowerSearchTerm = searchTerm.ToLower();

            List<Czytelnik> searchResults = CzytelnicyList
                .Where(reader =>
                    (reader.NazwaUzytkownika != null && reader.NazwaUzytkownika.ToLower().Contains(lowerSearchTerm)))
                .ToList();

            CzytelnicyListView.ItemsSource = new ObservableCollection<Czytelnik>(searchResults);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = SearchBox.Text;

            // Sprawdź, czy wpisano jakieś dane
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Jeśli pole wyszukiwania jest puste, załaduj wszystkich czytelników
                CzytelnicyListView.ItemsSource = CzytelnicyList;
            }
            else
            {
                // W przeciwnym razie, przeprowadź wyszukiwanie po nazwie użytkownika
                SearchByUserName(searchTerm);
            }
        }
    }
}
