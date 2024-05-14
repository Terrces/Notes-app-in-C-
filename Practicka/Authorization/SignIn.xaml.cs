using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Diagnostics;

namespace Practicka.Authorization
{
    public partial class Login : Window
    {

        MainWindow mainWindow;

        public Login(MainWindow main)
        {
            InitializeComponent();
            mainWindow = main;

        }

        private void GoToRegistration_Click_1(object sender, RoutedEventArgs e)
        {
            Authorization.Registration registration = new Authorization.Registration();
            registration.ShowDialog();
        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void loginbutton_Click(object sender, RoutedEventArgs e)
        {
            Modules.DataBaseModule DataBase = new Modules.DataBaseModule();

            StringBuilder builder = new StringBuilder();
            using (SHA256 hash256 = SHA256.Create())
            {
                byte[] hash = hash256.ComputeHash(Encoding.UTF8.GetBytes($"{login.Text}{password.Password}"));

                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
            }

            using (SqlConnection connection = new SqlConnection(DataBase.connectionString))
            {
                string query = $"SELECT * FROM Users WHERE Login LIKE '{login.Text}' AND Password LIKE '{builder}'";
                SqlCommand command = new SqlCommand(query,connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        mainWindow.GetLogin(login.Text);
                        this.Close();
                    }
                    else
                    {
                        var result = MessageBox.Show("Не верный Логин или Пароль!","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    reader.Close();
                }
                catch(SqlException ex)
                {
                    Console.WriteLine("Ошибка при выполнении запроса: " + ex.Message);
                }
            }

            
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                mainWindow.Close(sender , null);
            }
        }

        private void login_GotFocus(object sender, RoutedEventArgs e)
        {
            if (login.Text == "Логин")
            {
                login.Text = "";
            }
        }

        private void login_LostFocus(object sender, RoutedEventArgs e)
        {
            if (login.Text == "")
            {
                login.Text = "Логин";
            }
        }

        private void password_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordText.Visibility = Visibility.Hidden;
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (password.Password.Length == 0)
            {
                PasswordText.Visibility = Visibility.Visible;
            }

        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
