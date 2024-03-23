using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Text;

namespace Library
{
    public partial class AssignBookPage : Page
    {
        private string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

        public ObservableCollection<Czytelnik> CzytelnicyList { get; set; }
        public ObservableCollection<Ksiazka> KsiazkiList { get; set; }
        public ObservableCollection<PrzypisanaKsiazka> PrzypisaneKsiazkiList { get; set; }

        public AssignBookPage()
        {
            InitializeComponent();
            CzytelnicyList = new ObservableCollection<Czytelnik>();
            KsiazkiList = new ObservableCollection<Ksiazka>();
            PrzypisaneKsiazkiList = new ObservableCollection<PrzypisanaKsiazka>();
            LoadData();
        }

        public class Czytelnik
        {
            public int Id { get; set; }
            public string? ImieNazwisko { get; set; }
        }

        public class Ksiazka
        {
            public string? Isbn { get; set; }
            public string? Tytul { get; set; }
        }

        public class PrzypisanaKsiazka
        {
            public int CzytelnikId { get; set; }
            public string? ImieNazwiskoCzytelnika { get; set; }
            public string? IsbnKsiazki { get; set; }
            public string? TytulKsiazki { get; set; }
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                {
                    conn.Open();

                    // Pobierz przypisane książki z bazy danych
                    string przypisaneKsiazkiQuery = "SELECT CzytelnikId, fullName, ISBN, Tytul FROM przypisane_ksiazki";
                    using (MySqlCommand przypisaneKsiazkiCmd = new MySqlCommand(przypisaneKsiazkiQuery, conn))
                    {
                        using (MySqlDataAdapter przypisaneKsiazkiAdapter = new MySqlDataAdapter(przypisaneKsiazkiCmd))
                        {
                            DataTable przypisaneKsiazkiTable = new DataTable();
                            przypisaneKsiazkiAdapter.Fill(przypisaneKsiazkiTable);

                            PrzypisaneKsiazkiList.Clear();

                            foreach (DataRow row in przypisaneKsiazkiTable.Rows)
                            {
                                PrzypisaneKsiazkiList.Add(new PrzypisanaKsiazka
                                {
                                    CzytelnikId = Convert.ToInt32(row["CzytelnikId"]),
                                    ImieNazwiskoCzytelnika = row["fullName"].ToString(),
                                    IsbnKsiazki = row["ISBN"].ToString(),
                                    TytulKsiazki = row["Tytul"].ToString()
                                });
                            }
                        }
                    }

                    // Pobierz czytelników
                    string uzytkownicyQuery = "SELECT ID, fullName FROM użytkownik";
                    using (MySqlCommand uzytkownicyCmd = new MySqlCommand(uzytkownicyQuery, conn))
                    {
                        using (MySqlDataAdapter uzytkownicyAdapter = new MySqlDataAdapter(uzytkownicyCmd))
                        {
                            DataTable uzytkownicyTable = new DataTable();
                            uzytkownicyAdapter.Fill(uzytkownicyTable);

                            CzytelnicyList.Clear();

                            foreach (DataRow row in uzytkownicyTable.Rows)
                            {
                                CzytelnicyList.Add(new Czytelnik
                                {
                                    Id = Convert.ToInt32(row["ID"]),
                                    ImieNazwisko = row["fullName"].ToString()
                                });
                            }
                        }
                    }

                    // Pobierz książki
                    string ksiazkiQuery = "SELECT ISBN, title FROM książki";
                    using (MySqlCommand ksiazkiCmd = new MySqlCommand(ksiazkiQuery, conn))
                    {
                        using (MySqlDataAdapter ksiazkiAdapter = new MySqlDataAdapter(ksiazkiCmd))
                        {
                            DataTable ksiazkiTable = new DataTable();
                            ksiazkiAdapter.Fill(ksiazkiTable);

                            KsiazkiList.Clear();

                            foreach (DataRow row in ksiazkiTable.Rows)
                            {
                                KsiazkiList.Add(new Ksiazka
                                {
                                    Isbn = row["ISBN"].ToString(),
                                    Tytul = row["title"].ToString()
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
            // Ustaw dane źródłowe dla ComboBoxów
            CzytelnicyComboBox.ItemsSource = CzytelnicyList;
            KsiazkiComboBox.ItemsSource = KsiazkiList;

            // Ustaw dane źródłowe dla ListView
            AssignedBooksListView.ItemsSource = PrzypisaneKsiazkiList;

            // Dodaj obsługę zdarzeń dla wpisywania tekstu w ComboBoxach
            CzytelnicyComboBox.IsTextSearchEnabled = true;
            KsiazkiComboBox.IsTextSearchEnabled = true;
        }

        private void CzytelnicyComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = CzytelnicyComboBox.Text;
            var filteredCzytelnicy = CzytelnicyList.Where(c => c.ImieNazwisko?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            CzytelnicyComboBox.ItemsSource = filteredCzytelnicy;
        }

        private void KsiazkiComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = KsiazkiComboBox.Text;
            var filteredKsiazki = KsiazkiList.Where(k => k.Tytul?.Contains(filterText, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            KsiazkiComboBox.ItemsSource = filteredKsiazki;
        }

        private void AssignBook_Click(object sender, RoutedEventArgs e)
        {
            if (CzytelnicyComboBox.SelectedItem != null && KsiazkiComboBox.SelectedItem != null)
            {
                Czytelnik selectedCzytelnik = (Czytelnik)CzytelnicyComboBox.SelectedItem;
                Ksiazka selectedKsiazka = (Ksiazka)KsiazkiComboBox.SelectedItem;

                PrzypisaneKsiazkiList.Add(new PrzypisanaKsiazka
                {
                    CzytelnikId = selectedCzytelnik.Id,
                    ImieNazwiskoCzytelnika = selectedCzytelnik.ImieNazwisko,
                    IsbnKsiazki = selectedKsiazka.Isbn,
                    TytulKsiazki = selectedKsiazka.Tytul
                });

                // Zapisz przypisaną książkę do bazy danych
                SaveAssignedBookToDatabase(selectedCzytelnik.Id, selectedKsiazka.Isbn, selectedCzytelnik.ImieNazwisko, selectedKsiazka.Tytul);

                // Odśwież widok ListView
                AssignedBooksListView.Items.Refresh();
            }
        }

        private void SaveAssignedBookToDatabase(int czytelnikId, string isbnKsiazki, string imieNazwiskoCzytelnika = "", string tytulKsiazki = "")
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                {
                    conn.Open();

                    // Wprowadź zapytanie SQL do zapisania przypisanej książki
                    string insertQuery = "INSERT INTO przypisane_ksiazki (CzytelnikId, fullName, ISBN, Tytul) VALUES (@czytelnikId, @imieNazwiskoCzytelnika, @isbnKsiazki, @tytulKsiazki)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@czytelnikId", czytelnikId);
                        cmd.Parameters.AddWithValue("@isbnKsiazki", isbnKsiazki);
                        cmd.Parameters.AddWithValue("@imieNazwiskoCzytelnika", string.IsNullOrWhiteSpace(imieNazwiskoCzytelnika) ? DBNull.Value : (object)imieNazwiskoCzytelnika);
                        cmd.Parameters.AddWithValue("@tytulKsiazki", string.IsNullOrWhiteSpace(tytulKsiazki) ? DBNull.Value : (object)tytulKsiazki);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CzytelnicyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void KsiazkiComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

