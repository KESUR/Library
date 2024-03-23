 using MySql.Data.MySqlClient;
using System;
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
    /// <summary>
    /// Interaction logic for AvailabilityPage.xaml
    /// </summary>
    public partial class AvailabilityPage : Page
    {
        private string mysqlCon = "server=127.0.0.1; user=root; database=pracownicy; password=";

        public ObservableCollection<Ksiazka> KsiazkiList { get; set; }

        public User User { get; set; }

        public AvailabilityPage(User user)
        {
            InitializeComponent();
            User = user;
            KsiazkiList = new ObservableCollection<Ksiazka>();
            LoadData();
        }
        public class Ksiazka
        {
            public string ?Isbn { get; set; }
            public string ?Tytul { get; set; }
            public string ?Autor { get; set; }
            public bool Dostepnosc { get; set; }
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(mysqlCon))
                {
                    conn.Open();

                    string query = "SELECT książki.ISBN, książki.title AS Tytul, CONCAT(autorzy.firstName, ' ', autorzy.lastName) AS Autor, książki.availability AS Dostepnosc " +
                                   "FROM książki " +
                                   "INNER JOIN autorzy ON książki.authorID = autorzy.ID";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            KsiazkiList.Clear();

                            foreach (DataRow row in table.Rows)
                            {
                                KsiazkiList.Add(new Ksiazka
                                {
                                    Isbn = row["ISBN"].ToString(),
                                    Tytul = row["Tytul"].ToString(),
                                    Autor = row["Autor"].ToString(),
                                    Dostepnosc = Convert.ToBoolean(row["Dostepnosc"])
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

            
            lstAvailability.ItemsSource = KsiazkiList;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (txtSearch.Text == "Wyszukaj...")
            {
                txtSearch.Text = "";
            }
        }

        private void SearchByTitleOrAuthor(string searchTerm)
        {
            if (KsiazkiList == null || KsiazkiList.Count == 0)
            {
                return;
            }

            string lowerSearchTerm = searchTerm.ToLower();

            List<Ksiazka> searchResults = KsiazkiList
                .Where(ksiazka =>
                    (ksiazka.Tytul != null && ksiazka.Tytul.ToLower().Contains(lowerSearchTerm)) ||
                    (ksiazka.Autor != null && ksiazka.Autor.ToLower().Contains(lowerSearchTerm)))
                .ToList();

            lstAvailability.ItemsSource = new ObservableCollection<Ksiazka>(searchResults);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTerm = txtSearch.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                lstAvailability.ItemsSource = KsiazkiList;
            }
            else
            {
                SearchByTitleOrAuthor(searchTerm);
            }
        }

    }
}
