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

namespace SnakeGame
{
    /// <summary>
    /// Логика взаимодействия для NewPlayer.xaml
    /// </summary>
    public partial class NewPlayer : Window
    {
        private Records _currentPlayer = new Records();

        MainWindow mainWindow;
        public NewPlayer(MainWindow main)
        {
            InitializeComponent();
            DataContext = _currentPlayer;
            mainWindow = main;
        }



        private void btnRecords_Click(object sender, RoutedEventArgs e)
        {
            RecordListWindow reclist = new RecordListWindow();
            reclist.Show();
        }

        private void btnNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPlayer.Text) || txtNewPlayer.Text == "Новый игрок")
            {
                MessageBox.Show("Введите никнейм!");
                txtNewPlayer.Text = "";
            }
            
            else
            {
                /*if (_currentPlayer.id == 0)
                {
                    _currentPlayer.Record = mainWindow.maxScore;
                    Entities.GetContext().Records.Add(_currentPlayer);
                }

                else if (_currentPlayer.id == 0 && txtNewPlayer.Text != mainWindow.name)
                {
                    _currentPlayer.Record = mainWindow.Score;
                    Entities.GetContext().Records.Add(_currentPlayer);
                    mainWindow.maxScore = 0;
                }*/


                try
                {
                    _currentPlayer.Record = mainWindow.Score;
                    mainWindow.maxScore = mainWindow.Score;
                    foreach (var player in Entities.GetContext().Records)
                    {
                        if (player.Name == txtNewPlayer.Text && player.Record < _currentPlayer.Record)
                        {
                            Entities.GetContext().Records.Remove(player);
                            Entities.GetContext().Records.Add(_currentPlayer);
                        }

                        else if (player.Name == txtNewPlayer.Text && player.Record >= _currentPlayer.Record)
                            break;
                    }
                    
                    Entities.GetContext().SaveChanges();
                    mainWindow.name = txtNewPlayer.Text;
                    MessageBox.Show($"Пользователь {_currentPlayer.Name} добавлен с рекордом {_currentPlayer.Record}");
                    this.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка добавления пользователя");
                }
            }

            


        }

        
    }
}
