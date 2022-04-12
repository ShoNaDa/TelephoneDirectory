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
        static readonly List<string> info = new List<string>();

        readonly TelephoneDirectoryEntities4 telephoneDirectoryEntities = new TelephoneDirectoryEntities4();

        public MainWindow()
        {
            InitializeComponent();

            info.Clear();

            //заполнение списка
            foreach (Directory item in telephoneDirectoryEntities.Directory.ToList())
            {
                MakeElements(item);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (AddToDB(nameTextBox.Text, phoneTextBox.Text, addressTextBox.Text, timeTextBox.Text, sphereActivityTextBox.Text))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                Close();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (telephoneDirectoryListBox.SelectedIndex != -1)
            {
                int code = Convert.ToInt32(telephoneDirectoryListBox.Items[telephoneDirectoryListBox.SelectedIndex - 1]
                        .ToString().Split(' ')[1].Trim());

                if (EditToDB(nameTextBox.Text, phoneTextBox.Text, addressTextBox.Text,
                    timeTextBox.Text, sphereActivityTextBox.Text, code))
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
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
                int code = Convert.ToInt32(telephoneDirectoryListBox.Items[telephoneDirectoryListBox.SelectedIndex - 1]
                            .ToString().Split(' ')[1].Trim());

                if (DeleteToDB(code))
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Выберите строку из списка");
            }
        }

        //функция проверки на заполнение
        public bool StringNotEmpty(string _str)
        {
            return _str != string.Empty && _str != "" && _str.Trim() != "";
        }

        //функция проверки на цифры
        public bool CheckingForNumbers(string _str)
        {
            return Regex.IsMatch(_str, @"[0-9]");
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (telephoneDirectoryListBox.SelectedIndex != -1)
            {
                Clipboard.SetText(info[(telephoneDirectoryListBox.SelectedIndex - 1) / 2].ToString().Split('|')[1].Trim());
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            //очищаем листбокс
            telephoneDirectoryListBox.Items.Clear();

            List<Directory> sortDirect = SortList();

            info.Clear();

            //заполнение списка
            foreach (Directory item in sortDirect)
            {
                MakeElements(item);
            }
        }

        private void MakeElements(Directory _tableDir)
        {
            //программно создаем строку
            //это кнопка скопировать
            Button copy = new Button
            {
                Background = new ImageBrush(new BitmapImage(new Uri(@"C:\Users\vovab\Candy shop\Images\copy.png"))),
                Margin = new Thickness(5, 0, 0, 0),
                Cursor = Cursors.Hand,
                MinWidth = 50,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(2),
                ToolTip = new ToolTip { Content = "Нажмите, чтобы скопировать номер телефона" } //подсказка
            };
            copy.Click += new RoutedEventHandler(CopyButton_Click);

            //соединяем
            StackPanel line = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            //это скрытый код
            Label codeLbl = new Label
            {
                Content = _tableDir.Code,
                Visibility = Visibility.Collapsed
            };

            //это текст
            line.Children.Add(new Label
            {
                Content = _tableDir.NameOrg + " | " + _tableDir.Phone + " | " + _tableDir.AddressOrg + " | " +
                    _tableDir.TimeWork + " | " + _tableDir.sphereActivity
            });
            line.Children.Add(copy);

            telephoneDirectoryListBox.Items.Add(new ListBoxItem
            {
                IsHitTestVisible = false,
                Content = codeLbl
            }); //на экран 'это скрытый код'
            telephoneDirectoryListBox.Items.Add(line); //на экран 'это текст весь'

            info.Add(_tableDir.NameOrg + " | " + _tableDir.Phone + " | " + _tableDir.AddressOrg + " | " +
                    _tableDir.TimeWork + " | " + _tableDir.sphereActivity);
        }

        public bool AddToDB(string _name, string _phone, string _address, string _time, string _sphere)
        {
            if (StringNotEmpty(_name) && StringNotEmpty(_phone) && StringNotEmpty(_address) && StringNotEmpty(_time)
                && StringNotEmpty(_phone) && StringNotEmpty(_sphere))
            {
                bool isPhone = true;
                foreach (char item in _phone)
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
                        NameOrg = _name,
                        Phone = _phone,
                        AddressOrg = _address,
                        TimeWork = _time,
                        sphereActivity = _sphere
                    });

                    telephoneDirectoryEntities.SaveChanges();

                    return true;
                }
                else
                {
                    MessageBox.Show("Телефон должен содержать только цифры");

                    return false;
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");

                return false;
            }
        }
        public bool EditToDB(string _name, string _phone, string _address, string _time, string _sphere, int _code)
        {
            if (StringNotEmpty(_name) && StringNotEmpty(_phone) && StringNotEmpty(_address) && StringNotEmpty(_time)
                && StringNotEmpty(_phone) && StringNotEmpty(_sphere))
            {
                bool isPhone = true;
                foreach (char item in _phone)
                {
                    if (!CheckingForNumbers(item.ToString()))
                    {
                        isPhone = false;
                    }
                }

                if (isPhone)
                {
                    //тут изменяю
                    var directory = telephoneDirectoryEntities.Directory
                        .Where(c => c.Code == _code)
                        .FirstOrDefault();

                    directory.NameOrg = _name;
                    directory.Phone = _phone;
                    directory.AddressOrg = _address;
                    directory.TimeWork = _time;
                    directory.sphereActivity = _sphere;

                    telephoneDirectoryEntities.SaveChanges();

                    return true;
                }
                else
                {
                    MessageBox.Show("Телефон должен содержать только цифры");

                    return false;
                }
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены");

                return false;
            }
        }

        public bool DeleteToDB(int _code)
        {
            //тут удаляю
            Directory directory = telephoneDirectoryEntities.Directory
                .Where(o => o.Code == _code)
                .FirstOrDefault();

            telephoneDirectoryEntities.Directory.Remove(directory);
            telephoneDirectoryEntities.SaveChanges();

            return true;
        }

        public List<Directory> SortList()
        {
            //получаем отсортированную таблицу
            List<Directory> sortDirect = telephoneDirectoryEntities.Directory.OrderBy(p => p.sphereActivity).ToList();
            
            return sortDirect;
        }
    }
}