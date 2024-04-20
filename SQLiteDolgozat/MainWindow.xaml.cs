using System.Collections.ObjectModel;
using System.IO;
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
using Microsoft.Data.Sqlite;

namespace SQLiteDolgozat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqliteConnection connection;
        ObservableCollection<Tanulo> adatok = new ObservableCollection<Tanulo>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLetrehoz_Click(object sender, RoutedEventArgs e)
        {
            connection = new($"Filename=adatok.db");
            connection.Open();

            string createTableCommand = "CREATE TABLE IF NOT EXISTS tanulok(nev VARCHAR(256), nem VARCHAR(4), pontszam INTEGER, szak VARCHAR(64))";
            SqliteCommand command = new(createTableCommand, connection);
            command.ExecuteNonQuery();

            foreach (var item in File.ReadAllLines("felvi.csv", Encoding.UTF8).Skip(1))
            {
                string[] felvagott = item.Split(";");
                string nem = felvagott[1] == "f" ? "fiú" : "lány";

                string insertCommand = $"INSERT INTO tanulok(nev, nem, pontszam, szak) VALUES ('{felvagott[0]}', '{nem}', {Convert.ToInt32(felvagott[2])}, '{felvagott[3]}')";

                command = new(insertCommand, connection);
                command.ExecuteNonQuery();
            }
            connection.Close();
            MessageBox.Show("Az adatbázis elkészült!");
        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            connection = new($"Filename=adatok.db");
            connection.Open();

            string sqlCommand = "SELECT * FROM tanulok";

            SqliteCommand command = new(sqlCommand, connection);
            SqliteDataReader reader = command.ExecuteReader();

            adatok = new();

            while (reader.Read())
            {
                string nev = reader.GetString(0);
                string nem = reader.GetString(1);
                int pontszam = reader.GetInt32(2);
                string szak = reader.GetString(3);

                Tanulo tanulo = new(nev, nem, pontszam, szak);

                adatok.Add(tanulo);
            }

            dgAdatok.ItemsSource = adatok;
            reader.Close();
        }

        private void txtSzakszures_TextChanged(object sender, TextChangedEventArgs e)
        {
            string szak = txtSzakszures.Text.ToLower();

            if (szak.Trim() == "")
            {
                dgAdatok.ItemsSource = adatok;
                return;
            }

            var szurtadatok = adatok.Where(tanulo => tanulo.Szak.ToLower().Contains(szak)).ToList();

            dgAdatok.ItemsSource = szurtadatok;
        }

        private void txtLegjobbtanulok_TextChanged(object sender, TextChangedEventArgs e)
        {
            string szak = txtLegjobbtanulok.Text;

            var talalatok = adatok.Where(tanulo => tanulo.Szak == szak).ToList();

            if (talalatok.Count > 0)
            {
                int maxpont = talalatok.Max(tanulo => tanulo.Pontszam);

                var legjobbak = talalatok.Where(tanulo => tanulo.Pontszam == maxpont).Select(tanulo => tanulo.Nev);

                lbTanulok.ItemsSource = legjobbak;
            }
            else
            {
                MessageBox.Show("Nincs ilyen szak");
            }
        }

        private void btnTorles_Click(object sender, RoutedEventArgs e)
        {
            if (dgAdatok.SelectedIndex > -1)
            {
                connection = new($"Filename=adatok.db");
                connection.Open();

                string sqlCommand = $"DELETE FROM tanulok WHERE nev='{adatok[dgAdatok.SelectedIndex].Nev}' AND szak='{adatok[dgAdatok.SelectedIndex].Szak}' AND pontszam={adatok[dgAdatok.SelectedIndex].Pontszam}";

                SqliteCommand command = new(sqlCommand, connection);
                command.ExecuteNonQuery();

                adatok.RemoveAt(dgAdatok.SelectedIndex);
                dgAdatok.ItemsSource = adatok;
            }
            else
            {
                MessageBox.Show("Törléshez válasszon ki egy sort!");
            }
        }

        private void btnNemek_Click(object sender, RoutedEventArgs e)
        {
            var szakok = adatok.Where(tanulo => tanulo.Szak.ToLower().Contains("informatika")).ToList();

            int lanyokszama = szakok.Count(tanulo => tanulo.Nem == "lány");
            int fiukszama = szakok.Count(tanulo => tanulo.Nem == "fiú");

            Label fiuk = new();
            fiuk.Margin = new Thickness(10);
            fiuk.Content = $"Fiúk száma: {fiukszama}";

            Label lanyok = new();
            lanyok.Margin = new Thickness(10);
            lanyok.Content = $"Lányok száma: {lanyokszama}";

            spMenu.Children.Add(fiuk);
            spMenu.Children.Add(lanyok);
        }
    }
}