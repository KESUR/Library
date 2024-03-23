using System;
using System.Windows;

namespace Library
{
    public partial class NewWindow : Window
    {
        public User User { get; set; }

        public NewWindow()
        {
            InitializeComponent();
        }

        public NewWindow(User loggedUser, bool isEmployee)
        {
            InitializeComponent();
            User = loggedUser;

            if (User != null && isEmployee)
            {
                ShowEmployeeButtons();
            }
            else
            {
                HideEmployeeButtons();
            }
        }

        private void ShowEmployeeButtons()
        {
            btnNavigateToReaderPage.Visibility = Visibility.Visible;
            btnNavigateToBookPage.Visibility = Visibility.Visible;
            btnNavigateToAssignBookPage.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Visible;

            MainFrame.Navigate(new AvailabilityPage(User));
        }

        private void HideEmployeeButtons()
        {
            btnNavigateToReaderPage.Visibility = Visibility.Collapsed;
            btnNavigateToBookPage.Visibility = Visibility.Collapsed;
            btnNavigateToAssignBookPage.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;
        }

        private void NavigateToReaderPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReaderPage());
        }

        private void NavigateToBookPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BookPage());
        }

        private void NavigateToAssignBookPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AssignBookPage());
        }

        private void NavigateToAvailabilityPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AvailabilityPage(User));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
