using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
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

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private Random rnd = new Random();

        private System.Windows.Threading.DispatcherTimer gameTickTimer = new System.Windows.Threading.DispatcherTimer();

        const int SnakeSize = 30;
        const int SnakeStartLength = 3;
        const int SnakeStartSpeed = 200;
        //const int SnakeSpeedThreshold = 30;

        private SolidColorBrush snakeBrush = Brushes.RoyalBlue;
        private List<Snake> snake = new List<Snake>();

        private UIElement snakeFood = null;
        private SolidColorBrush foodBrush = Brushes.Tomato;

        public enum SnakeDirection { Left, Right, Up, Down };
        private SnakeDirection snakeDirection = SnakeDirection.Right;
        private int snakeLength;
        public int Score = 0;
        public int maxScore = 0;
        public string name;

        public MainWindow()
        {
            InitializeComponent();
            gameTickTimer.Tick += GameTickTimer_Tick;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = (Visibility)1;
            Score = 0;
            txtScore.Text = Score.ToString();
            StartNewGame();
        }

        private void DrawSnake()
        {
            foreach(Snake sn in snake)
            {
                if(sn.UiElement == null)
                {
                    sn.UiElement = new Rectangle()
                    {
                        Width = SnakeSize,
                        Height = SnakeSize,
                        Fill = snakeBrush
                    };
                    Area.Children.Add(sn.UiElement);
                    Canvas.SetTop(sn.UiElement, sn.Position.Y);
                    Canvas.SetLeft(sn.UiElement, sn.Position.X);
                }
            }
        }

        private void MoveSnake()
        {
            while(snake.Count >= snakeLength) 
            {
                Area.Children.Remove(snake[0].UiElement);
                snake.RemoveAt(0);
            }

            foreach(Snake sn in snake)
            {
                (sn.UiElement as Rectangle).Fill = snakeBrush;
                sn.isHead = false;
            }

            Snake snakeHead = snake[snake.Count - 1];
            double nextX = snakeHead.Position.X;
            double nextY = snakeHead.Position.Y;

            switch (snakeDirection)
            {
                case SnakeDirection.Left:
                    nextX -= SnakeSize;
                    break;
                case SnakeDirection.Right:
                    nextX += SnakeSize;
                    break;
                case SnakeDirection.Up:
                    nextY -= SnakeSize;
                    break;
                case SnakeDirection.Down:
                    nextY += SnakeSize;
                    break;
            }

            snake.Add(new Snake()
            {
                Position = new Point(nextX, nextY),
                isHead = true
            });

            DrawSnake();
            DoCollisionCheck();
        }

        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void StartNewGame()
        {
            snakeLength = SnakeStartLength;
            snakeDirection = SnakeDirection.Right;
            snake.Add(new Snake() { Position = new Point(SnakeSize, SnakeSize) });
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(SnakeStartSpeed);
            DrawSnake();
            DrawSnakeFood();
            UpdateGameStatus();
            gameTickTimer.IsEnabled = true;
        }

        private void DrawSnakeFood()
        {
            Point foodPosition = GetNextFoodPosition();
            snakeFood = new Ellipse()
            {
                Width = SnakeSize,
                Height = SnakeSize,
                Fill = foodBrush
            };
            Area.Children.Add(snakeFood);
            Canvas.SetTop(snakeFood, foodPosition.Y);
            Canvas.SetLeft(snakeFood, foodPosition.X);
        }

        private Point GetNextFoodPosition()
        {
            int maxX = (int)(Area.ActualWidth / SnakeSize);
            int maxY = (int)(Area.ActualHeight / SnakeSize);
            int foodX = rnd.Next(0, maxX) * SnakeSize;
            int foodY = rnd.Next(0, maxY) * SnakeSize;

            foreach (Snake sn in snake)
            {
                if ((sn.Position.X == foodX) && (sn.Position.Y == foodY))
                    return GetNextFoodPosition();
            }

            return new Point(foodX, foodY);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            SnakeDirection originalSnakeDirection = snakeDirection;
            switch (e.Key)
            {
                case Key.Up:
                    if (snakeDirection != SnakeDirection.Down)
                        snakeDirection = SnakeDirection.Up;
                    break;
                case Key.Down:
                    if (snakeDirection != SnakeDirection.Up)
                        snakeDirection = SnakeDirection.Down;
                    break;
                case Key.Left:
                    if (snakeDirection != SnakeDirection.Right)
                        snakeDirection = SnakeDirection.Left;
                    break;
                case Key.Right:
                    if (snakeDirection != SnakeDirection.Left)
                        snakeDirection = SnakeDirection.Right;
                    break;
                case Key.Space:
                    StartNewGame();
                    break;
            }
            if (snakeDirection != originalSnakeDirection)
                MoveSnake();
        }

        private void DoCollisionCheck()
        {
            Snake snakeHead = snake[snake.Count - 1];

            if ((snakeHead.Position.X == Canvas.GetLeft(snakeFood)) && (snakeHead.Position.Y == Canvas.GetTop(snakeFood)))
            {
                EatFood();
                return;
            }

            if ((snakeHead.Position.Y < 0) || (snakeHead.Position.Y >= Area.ActualHeight) ||
            (snakeHead.Position.X < 0) || (snakeHead.Position.X >= Area.ActualWidth))
            {
                EndGame();
            }

            foreach (Snake snakeBodyPart in snake.Take(snake.Count - 1))
            {
                if ((snakeHead.Position.X == snakeBodyPart.Position.X) && (snakeHead.Position.Y == snakeBodyPart.Position.Y))
                    EndGame();
            }
        }

        private void EatFood()
        {
            snakeLength++;
            Score++;
            int timerInterval = (int)gameTickTimer.Interval.TotalMilliseconds - 2;
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            Area.Children.Remove(snakeFood);
            DrawSnakeFood();
            UpdateGameStatus();
        }

        private void UpdateGameStatus()
        {
            txtScore.Text = Score.ToString();
        }

        private void EndGame()
        {
            gameTickTimer.IsEnabled = false;
            MessageBox.Show($"Игра окончена!\nВаш счёт - {Score}\nРекорд - {maxScore}", "Упс!", MessageBoxButton.OK, MessageBoxImage.Information);
            
            if (Score > maxScore)
            {
                maxScore = Score;
                txtScore.Text = "0";
                txtMaxScore.Text = maxScore.ToString();
                txtMaxScore.Visibility = Visibility.Visible;
                txtBlockMaxScore.Visibility = Visibility.Visible;
            }

            btnStart.Visibility = Visibility.Visible;
            Area.Children.Remove(snakeFood);
            foreach (var sn in snake)
                Area.Children.Remove(sn.UiElement);

            NewPlayer newplayer;
            newplayer = new NewPlayer(this);
            newplayer.Show();

        }
    }

    public class Snake
    {
        public UIElement UiElement { get; set; }
        public Point Position { get; set; }
        public bool isHead { get; set; }
    }

}
