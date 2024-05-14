using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;

namespace Practicka
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Authorization.Login login;
       // private string loginName;

        bool Logined = false;
        string UserLogin = "";
        int CurrentButton;
        public string Login;
        public string[] placholders = new string[] { "Заголовок", "Категории", "Здесь вы можете написать заметку", "URL Картинки для иконки заметки", "Поиск" };

        public MainWindow()
        {
            InitializeComponent();
            login = new Authorization.Login(this);
            //login.Owner = this;
            login.ShowDialog();

            /*Login = Convert.ToString(this.Title);*/
            ControlLeftBarContent();
            toggleVisible(null, "Append");
            toggleVisible(SaveChanges, "SaveChanges");
        }

        private void toggleVisible(Button button, string Type)
        {
            switch (Type)
            {
                case "SaveChanges":
                    Modules.DataBaseModule dataBase = new Modules.DataBaseModule();
                    string connectionString = dataBase.connectionString;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = $"SELECT * FROM Notes WHERE Title LIKE '{NoteTitle.Text}' AND Category LIKE '{Tag.Text}' AND Note LIKE '{Note.Text}'";
                        SqlCommand command = new SqlCommand(query,connection);
                        try
                        {
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                button.Visibility = Visibility.Hidden;
                                button.IsEnabled = false;
                            }
                            else
                            {
                                Console.WriteLine(reader.Read());
                                button.Visibility = Visibility.Visible;
                                button.IsEnabled = true;
                            }
                        }
                        catch(SqlException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    break;
                case "Append":

                    if (AppendWindow.IsEnabled == true)
                    {

                        NewTitle.Text = placholders[0];
                        NewTag.Text = placholders[1];
                        NewNote.Text = placholders[2];
                        NewImage.Text = placholders[3];
                        AppendWindow.Visibility = Visibility.Hidden;
                        AppendWindow.IsEnabled = false;

                    }
                    else
                    {

                        NoteTitle.Text = "";
                        Tag.Text = "";
                        Note.Text = "";
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("https://i.pinimg");
                        bitmapImage.EndInit();
                        NoteImage.Source = bitmapImage;
                        AppendWindow.Visibility = Visibility.Visible;
                        AppendWindow.IsEnabled = true;
                        SaveChanges.Visibility = Visibility.Hidden;
                        SaveChanges.IsEnabled = false;

                    }

                    break;
            }
        }

        public void GetLogin(string loginName)
        {
            UserLogin = loginName;
            AuthorizationBTN.Content = $"Пользователь: {loginName}";
        }

        public void Close(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены что хотите закрыть окно ?", "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                //this.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }
        
        public void ControlLeftBarContent()
        {
            LeftBar.Children.Clear();
            Modules.DataBaseModule DataBase = new Modules.DataBaseModule();

            BitmapImage bitmapImage = new BitmapImage();
            Image image = new Image();
            StackPanel ButtonContainer = new StackPanel();
            Button buttons = new Button();
            LinearGradientBrush gradientBrush = new LinearGradientBrush();

            gradientBrush.StartPoint = new Point(0.5, 0);
            gradientBrush.EndPoint = new Point(0.5, 1);
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Transparent, 0.7));
            gradientBrush.GradientStops.Add(new GradientStop(Colors.Black, 1.0));

            Modules.DataBaseModule dataBase = new Modules.DataBaseModule();

            using (SqlConnection connection = new SqlConnection(dataBase.connectionString))
            {
                LeftBar.Children.Clear();
                // Открываем соединение
                connection.Open();

                // SQL-запрос для выборки данных из базы данных
                string sqlQuery = $"SELECT ID, Icon, Title FROM Notes WHERE Belongs LIKE '{UserLogin}'";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Проходим по каждой строке результата запроса
                        for (int i = 0; reader.Read(); i++)
                        {
                            // Получаем данные из столбцов ImagePath и Title
                            string imagePath = reader["Icon"].ToString();
                            string title = reader["Title"].ToString();

                            // Создаем элементы и добавляем их в UI
                            image = new Image();
                            bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.UriSource = new Uri(imagePath);
                            bitmapImage.EndInit();

                            StackPanel buttonContainer = new StackPanel();
                            Button button = new Button();

                            buttonContainer.Orientation = Orientation.Horizontal;
                            buttonContainer.Margin = new Thickness(0,15,0,0);
                            image.Source = bitmapImage;
                            image.Margin = new Thickness(0, 0, 5, 0);
                            image.MaxHeight = 64;
                            image.MaxWidth = 64;
                            button.Content = title;
                            button.Cursor = Cursors.Hand;
                            button.Tag = reader["ID"];
                            button.FontSize = 16;
                            button.Margin = new Thickness(0, 5, 0, 5);
                            button.Padding = new Thickness(8, 0, 8, 0);
                            button.Background = Brushes.Transparent;
                            button.BorderBrush = gradientBrush;

                            button.Click += Button_Click;

                            LeftBar.Children.Add(buttonContainer);
                            buttonContainer.Children.Add(image);
                            buttonContainer.Children.Add(button);
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            toggleVisible(SaveChanges, "SaveChanges");
            if (sender is Button button)
            {
                if (AppendWindow.IsEnabled == true) toggleVisible(null, "Append");

                int index = (int)button.Tag;

                CurrentButton = index;
                Modules.DataBaseModule module = new Modules.DataBaseModule();
                // Предполагается, что у вас есть строка подключения к базе данных
                string connectionString = module.connectionString;

                // SQL-запрос для выборки данных из базы данных
                string sqlQuery = "SELECT Icon, Title, Category, Note FROM Notes WHERE Id = @Id";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Открываем соединение
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        // Добавляем параметр @Id для предотвращения SQL-инъекций
                        command.Parameters.AddWithValue("@Id", index);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Если есть результаты запроса
                            if (reader.Read())
                            {
                                // Получаем данные из столбцов ImagePath, Title, Tag и Text
                                string imagePath = reader["Icon"].ToString();
                                string title = reader["Title"].ToString();
                                string tag = reader["Category"].ToString();
                                string text = reader["Note"].ToString();

                                // Устанавливаем данные в соответствующие элементы интерфейса
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.UriSource = new Uri(imagePath);
                                bitmapImage.EndInit();
                                NoteImage.Source = bitmapImage;
                                NoteTitle.Text = title;
                                Tag.Text = tag;
                                Note.Text = text;
                                Note.Tag = index;
                            }
                        }
                    }
                }
            }
        }
        private void AuthorizationBTN_click(object sender, RoutedEventArgs e)
        {
            UserLogin = "Не авторизован";
            login = new Authorization.Login(this);
            //login.Owner = this;
            login.ShowDialog();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainWindows.Settings settings = new MainWindows.Settings();
            settings.ShowDialog();
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            Modules.DataBaseModule mod = new Modules.DataBaseModule();

            string sqlDeleteQuery = "DELETE FROM Notes WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(mod.connectionString))
            {
                // Открываем соединение
                connection.Open();

                using (SqlCommand command = new SqlCommand(sqlDeleteQuery, connection))
                {
                    // Добавляем параметр @Id для предотвращения SQL-инъекций
                    command.Parameters.AddWithValue("@Id", CurrentButton);

                    // Выполняем команду удаления
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        CurrentButton = 0;
                        NoteTitle.Text = "";
                        Tag.Text = "";
                        Note.Text = "";
                        ControlLeftBarContent();
                        // Успешно удалено
                        Console.WriteLine("Строка успешно удалена из базы данных.");
                    }
                    else
                    {
                        // Строка с указанным ID не найдена
                        Console.WriteLine("Строка с указанным ID не найдена в базе данных.");
                    }
                }
            }
        }

        private void append_Click(object sender, RoutedEventArgs e)
        {
            if (NewNote.Text != placholders[2])
            {
                if (NewTitle.Text == placholders[0])
                {
                    string text = NewNote.Text.Trim(); // Удаляем лишние пробелы в начале и конце строки

                    // Ищем первый пробел
                    int spaceIndex = text.IndexOf(' ');

                    // Если пробел найден, берем подстроку до него, иначе берем всю строку
                    string firstWord = spaceIndex != -1 ? text.Substring(0, spaceIndex) : text;

                    // Используйте переменную firstWord, содержащую первое слово
                    Console.WriteLine(firstWord);
                }

                if (NewTitle.Text.Trim() != placholders[0])
                {
                    Modules.DataBaseModule DataBase = new Modules.DataBaseModule();
                    using (SqlConnection connection = new SqlConnection(DataBase.connectionString))
                    {
                        string insertQuery = $"Insert INTO Notes (Title,Category,Icon,Note,Belongs) VALUES  ('{NewTitle.Text}', '{NewTag.Text}', '{Convert.ToString(NewImage.Text)}','{NewNote.Text}','{UserLogin}')";
                        SqlCommand command = new SqlCommand(insertQuery, connection);
                        connection.Open();
                        try
                        {
                            // Выполняем запрос на вставку данных
                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine("Добавлено записей: " + rowsAffected);
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine("Ошибка при добавлении данных: " + ex.Message);
                        }
                    }
                }

                if (NewImage.Text == placholders[4])
                {
                }

                toggleVisible(null, "Append");
                ControlLeftBarContent();
            }
        }

        private void ChoiseIcon_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Задаем фильтр для типов файлов, которые пользователь может выбирать
            openFileDialog.Filter = "Все файлы (*.*)|*.*";

            // Открываем диалоговое окно выбора файла и проверяем результат
            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к выбранному файлу
                string selectedFilePath = openFileDialog.FileName;

                // Здесь можно выполнить нужные действия с выбранным файлом
                // Например, вывести его путь в текстовом поле или выполнить чтение файла и т.д.

                // Пример вывода пути выбранного файла в MessageBox
                NewImage.Text = selectedFilePath;
            }
        }


        public void TextPlaceholderLogic(TextBox textBox, int placeholder)
        {
            if (textBox.Text == placholders[placeholder])
            {
                textBox.Text = "";
            }
            else if (textBox.Text == "")
            {
                textBox.Text = placholders[placeholder];
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Close(sender, e); }

        private void AddNote_Click(object sender, RoutedEventArgs e) => toggleVisible(null, "Append");

        private void NoteTitle_KeyUp(object sender, KeyEventArgs e) => toggleVisible(SaveChanges, "SaveChanges");

        private void Tag_KeyUp(object sender, KeyEventArgs e) => toggleVisible(SaveChanges, "SaveChanges");

        private void Note_KeyUp(object sender, KeyEventArgs e) => toggleVisible(SaveChanges, "SaveChanges");

        private void NewTitle_GotFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewTitle, 0);

        private void NewTag_GotFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewTag, 1);

        private void NewNote_GotFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewNote, 2);

        private void NewImage_GotFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewImage, 3);

        private void SearchNote_GotFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(SearchNote, 4);

        private void NewTitle_LostFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewTitle, 0);

        private void NewTag_LostFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewTag, 1);

        private void NewNote_LostFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewNote, 2);

        private void NewImage_LostFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(NewImage, 3);

        private void SearchNote_LostFocus(object sender, RoutedEventArgs e) => TextPlaceholderLogic(SearchNote, 4);

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            Modules.DataBaseModule dataBase = new Modules.DataBaseModule();
            string connectionString = dataBase.connectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string Query = $"UPDATE Notes SET Title = '{NoteTitle.Text}', Category = '{Tag.Text}', Note = '{Note.Text}' WHERE Id = {CurrentButton}";
                SqlCommand command = new SqlCommand(Query,connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    toggleVisible(SaveChanges, "SaveChanges");
                    LeftBar.Children.Clear();
                    ControlLeftBarContent();
                }
                catch
                {

                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LeftBar.Children.Clear();
            ControlLeftBarContent();
        }
    }
}