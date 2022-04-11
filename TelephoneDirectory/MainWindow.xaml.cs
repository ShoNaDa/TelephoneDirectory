using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TelephoneDirectory
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List
        List<string> info = new List<string>();
        //Dictionary
        Dictionary<int, int> infoWithCode = new Dictionary<int, int>();

        TelephoneDirectoryEntities4 telephoneDirectoryEntities = new TelephoneDirectoryEntities4();

        public MainWindow()
        {
            InitializeComponent();

            Directory directory = telephoneDirectoryEntities.Directory.FirstOrDefault();

            //заполнение списка
            foreach(var item in telephoneDirectoryEntities.Directory)
            {
                directory = item;

                if (directory == null)
                {
                    break;
                }

                //программно создаем строку
                //это текст
                Label label = new Label
                {
                    Content = directory.NameOrg + " | " + directory.Phone + " | " + directory.AddressOrg + " | " +
                        directory.TimeWork + " | " + directory.sphereActivity
                };

                //это кнопка скопировать
                Button button = new Button
                {
                    Background = new ImageBrush(new BitmapImage(new Uri(@"C:\Users\vovab\Candy shop\Images\copy.png"))),
                    Margin = new Thickness(5, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    MinWidth = 50,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2)
                };
                button.Click += new RoutedEventHandler(CopyButton_Click);

                //соединяем
                StackPanel stackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(button);

                telephoneDirectoryListBox.Items.Add(stackPanel);

                info.Add((string)label.Content);
                infoWithCode.Add(directory.Code, info.Count - 1);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (StringNotEmpty(nameTextBox.Text) && StringNotEmpty(phoneTextBox.Text) && StringNotEmpty(addressTextBox.Text)
                && StringNotEmpty(timeTextBox.Text) && StringNotEmpty(sphereActivityTextBox.Text))
            {
                bool isPhone = true;
                foreach (char item in phoneTextBox.Text)
                {
                    if (!CheckingForNumbers(item.ToString()))
                    {
                        isPhone = false;
                    }
                }

                if (isPhone)
                {
                    //добавляем в БД
                    telephoneDirectoryEntities.Directory.Add(new Directory
                    {
                        NameOrg = nameTextBox.Text,
                        Phone = phoneTextBox.Text,
                        AddressOrg = addressTextBox.Text,
                        TimeWork = timeTextBox.Text,
                        sphereActivity = sphereActivityTextBox.Text
                    });

                    telephoneDirectoryEntities.SaveChanges();

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Телефон должен содержать только цифры");
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (telephoneDirectoryListBox.SelectedIndex != -1)
            {
                int index = telephoneDirectoryListBox.SelectedIndex;

                if (StringNotEmpty(nameTextBox.Text) && StringNotEmpty(phoneTextBox.Text) && StringNotEmpty(addressTextBox.Text)
                && StringNotEmpty(timeTextBox.Text) && StringNotEmpty(sphereActivityTextBox.Text))
                {
                    bool isPhone = true;
                    foreach (char item in phoneTextBox.Text)
                    {
                        if (!CheckingForNumbers(item.ToString()))
                        {
                            isPhone = false;
                        }
                    }

                    if (isPhone)
                    {
                        //изменение в БД
                        //тут я узнаю код
                        int code = 0;
                        foreach (var item in infoWithCode)
                        {
                            if (item.Value == index)
                            {
                                code = item.Key;
                            }
                        }

                        //тут изменяю
                        var directory = telephoneDirectoryEntities.Directory
                            .Where(c => c.Code == code)
                            .FirstOrDefault();

                        directory.NameOrg = nameTextBox.Text;
                        directory.Phone = phoneTextBox.Text;
                        directory.AddressOrg = addressTextBox.Text;
                        directory.TimeWork = timeTextBox.Text;
                        directory.sphereActivity = sphereActivityTextBox.Text;

                        telephoneDirectoryEntities.SaveChanges();

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Телефон должен содержать только цифры");
                    }
                }
                else
                {
                    MessageBox.Show("Все поля должны быть заполнены");
                }
            }
            else
            {
                MessageBox.Show("Выберите строку из списка");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (telephoneDirectoryListBox.SelectedIndex != -1)
            {
                int index = telephoneDirectoryListBox.SelectedIndex;

                //тут я узнаю код
                int code = 0;
                foreach (var item in infoWithCode)
                {
                    if (item.Value == index)
                    {
                        code = item.Key;
                    }
                }

                //тут удаляю
                Directory directory = telephoneDirectoryEntities.Directory
                    .Where(o => o.Code == code)
                    .FirstOrDefault();
                telephoneDirectoryEntities.Directory.Remove(directory);
                telephoneDirectoryEntities.SaveChanges();

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Выберите строку из списка");
            }
        }

        //функция проверки на заполнение
        static internal bool StringNotEmpty(string _str)
        {
            return _str != string.Empty && _str != "" && _str.Trim() != "";
        }

        //функция проверки на цифры
        private bool CheckingForNumbers(string _str)
        {
            return Regex.IsMatch(_str, @"[0-9]");
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (telephoneDirectoryListBox.SelectedIndex != -1)
            {
                Clipboard.SetText(info[telephoneDirectoryListBox.SelectedIndex].Split('|')[1].Trim());
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var direct = telephoneDirectoryEntities.Directory.OrderBy(p => p.sphereActivity);

            int index = 0;

            foreach(var item in direct)
            {
                //тут что-то непонятное
                int code = 0;
                int i = 0;
                foreach (var it in infoWithCode)
                {
                    if(i == index)
                    {
                        code = it.Key;
                        index++;
                        break;
                    }
                    i++;
                }

                var directory = telephoneDirectoryEntities.Directory
                    .Where(o => o.Code == code)
                    .FirstOrDefault();

                directory.NameOrg = item.NameOrg;
                directory.Phone = item.Phone;
                directory.AddressOrg = item.AddressOrg;
                directory.TimeWork = item.TimeWork;
                directory.sphereActivity = item.sphereActivity;
            }

            telephoneDirectoryEntities.SaveChanges();

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}