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
                        int code = Convert.ToInt32(telephoneDirectoryListBox.Items[telephoneDirectoryListBox.SelectedIndex - 1]
                            .ToString().Split(' ')[1].Trim());

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
                int code = Convert.ToInt32(telephoneDirectoryListBox.Items[telephoneDirectoryListBox.SelectedIndex - 1]
                            .ToString().Split(' ')[1].Trim());

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
                Clipboard.SetText(info[(telephoneDirectoryListBox.SelectedIndex - 1) / 2].ToString().Split('|')[1].Trim());
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            //очищаем листбокс
            telephoneDirectoryListBox.Items.Clear();

            //получаем отсортированную таблицу
            List<Directory> sortDirect = telephoneDirectoryEntities.Directory.OrderBy(p => p.sphereActivity).ToList();

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
    }
}