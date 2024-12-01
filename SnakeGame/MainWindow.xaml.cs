using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int CellSize = 20; // Размер одной клетки
        private const int Rows = 20;    // Количество строк
        private const int Columns = 20; // Количество столбцов

        private List<Rectangle> snake = new List<Rectangle>(); // Змейка
        private Rectangle food; // Еда
        private Point direction = new Point(1, 0); // Направление движения
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private bool isGameOver = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Таймер игры
            gameTimer.Interval = TimeSpan.FromMilliseconds(100);
            gameTimer.Tick += GameLoop;

            // Начальная змейка
            snake.Clear();
            GameCanvas.Children.Clear();

            // Создаем стартовый сегмент
            var head = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = Brushes.Green
            };
            GameCanvas.Children.Add(head);
            Canvas.SetLeft(head, CellSize * (Columns / 2));
            Canvas.SetTop(head, CellSize * (Rows / 2));
            snake.Add(head);

            // Добавляем еду
            GenerateFood();

            // Начинаем игру
            isGameOver = false;
            direction = new Point(1, 0);
            gameTimer.Start();
        }

        private void GenerateFood()
        {
            Random random = new Random();
            int x = random.Next(0, Columns) * CellSize;
            int y = random.Next(0, Rows) * CellSize;

            food = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = Brushes.Red
            };

            GameCanvas.Children.Add(food);
            Canvas.SetLeft(food, x);
            Canvas.SetTop(food, y);
        }

        private void UpdateSnakePosition()
        {
            double newLeft = Canvas.GetLeft(snake[0]) + direction.X * CellSize;
            double newTop = Canvas.GetTop(snake[0]) + direction.Y * CellSize;

            Canvas.SetLeft(snake[0], newLeft);
            Canvas.SetTop(snake[0], newTop);
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver)
                return;

            UpdateSnakePosition(); // Постоянное движение головы змейки
            MoveSnake();           // Перемещение остальных частей
            CheckCollisions();     // Проверка столкновений
        }


        private void MoveSnake()
        {
            Point previousPosition = new Point(Canvas.GetLeft(snake[0]), Canvas.GetTop(snake[0]));

            // Перемещаем остальные сегменты
            for (int i = 1; i < snake.Count; i++)
            {
                Point temp = new Point(Canvas.GetLeft(snake[i]), Canvas.GetTop(snake[i]));
                Canvas.SetLeft(snake[i], previousPosition.X);
                Canvas.SetTop(snake[i], previousPosition.Y);
                previousPosition = temp;
            }

            // Проверка съедания еды
            if (Math.Abs(Canvas.GetLeft(snake[0]) - Canvas.GetLeft(food)) < CellSize &&
                Math.Abs(Canvas.GetTop(snake[0]) - Canvas.GetTop(food)) < CellSize)
            {
                GrowSnake();
                GameCanvas.Children.Remove(food);
                GenerateFood();
            }
        }


        private void GrowSnake()
        {
            var tail = snake[snake.Count - 1];
            double tailLeft = Canvas.GetLeft(tail);
            double tailTop = Canvas.GetTop(tail);

            // Определяем позицию для нового сегмента
            double newLeft = tailLeft - direction.X * CellSize;
            double newTop = tailTop - direction.Y * CellSize;

            // Создаём новый сегмент
            var newSegment = new Rectangle
            {
                Width = CellSize,
                Height = CellSize,
                Fill = Brushes.Green
            };

            // Устанавливаем позицию и добавляем на поле
            Canvas.SetLeft(newSegment, newLeft);
            Canvas.SetTop(newSegment, newTop);
            GameCanvas.Children.Add(newSegment);

            // Добавляем в список сегментов змейки
            snake.Add(newSegment);
        }


        private void CheckCollisions()
        {
            // Проверка выхода за границы
            double headLeft = Canvas.GetLeft(snake[0]);
            double headTop = Canvas.GetTop(snake[0]);

            if (headLeft < 0 || headTop < 0 || headLeft >= Columns * CellSize || headTop >= Rows * CellSize)
            {
                GameOver();
            }

            // Проверка столкновений с самим собой
            for (int i = 1; i < snake.Count; i++)
            {
                if (Math.Abs(Canvas.GetLeft(snake[0]) - Canvas.GetLeft(snake[i])) < CellSize &&
                    Math.Abs(Canvas.GetTop(snake[0]) - Canvas.GetTop(snake[i])) < CellSize)
                {
                    GameOver();
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            isGameOver = true;
            MessageBox.Show("Вы проиграли.");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Изменение направления
            switch (e.Key)
            {
                case Key.Up:
                    if (direction.Y != 1) 
                        direction = new Point(0, -1);
                    break;
                case Key.Down:
                    if (direction.Y != -1) 
                        direction = new Point(0, 1);
                    break;
                case Key.Left:
                    if (direction.X != 1) 
                        direction = new Point(-1, 0);
                    break;
                case Key.Right:
                    if (direction.X != -1) 
                        direction = new Point(1, 0);
                    break;
                case Key.Enter:
                    if (isGameOver) InitializeGame();
                    break;
            }
        }
    }
}