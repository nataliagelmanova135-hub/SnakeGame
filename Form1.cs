using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SnakeGame
{
    public partial class Form1 : Form
    {
        // Игровые объекты
        private Panel gamePanel;
        private Label scoreLabel;
        private Button startButton;
        private Button pauseButton;

        // Игровые переменные
        private Timer gameTimer;
        private List<Point> snake = new List<Point>();
        private Point food;
        private int directionX = 1;
        private int directionY = 0;
        private int nextDirectionX = 1;
        private int nextDirectionY = 0;
        private int score = 0;
        private bool isGameRunning = false;
        private bool isPaused = false;

        // Параметры поля
        private int cellSize = 20;
        private int gridWidth = 30;
        private int gridHeight = 25;
        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();  // Вызов конструктора из Designer.cs

            // Создаём игровые элементы управления
            CreateGameUI();
            InitializeGame();
            SetupKeyboard();
        }

        private void CreateGameUI()
        {
            // Игровая панель
            gamePanel = new Panel();
            gamePanel.Location = new Point(10, 10);
            gamePanel.Size = new Size(600, 500);
            gamePanel.BackColor = Color.FromArgb(30, 30, 30);
            gamePanel.BorderStyle = BorderStyle.FixedSingle;
            gamePanel.Paint += GamePanel_Paint;

            // Кнопка старта
            startButton = new Button();
            startButton.Text = "Новая игра";
            startButton.Location = new Point(630, 10);
            startButton.Size = new Size(130, 40);
            startButton.BackColor = Color.Green;
            startButton.ForeColor = Color.White;
            startButton.FlatStyle = FlatStyle.Flat;
            startButton.Click += StartButton_Click;

            // Кнопка паузы
            pauseButton = new Button();
            pauseButton.Text = "Пауза";
            pauseButton.Location = new Point(630, 60);
            pauseButton.Size = new Size(130, 40);
            pauseButton.BackColor = Color.Orange;
            pauseButton.ForeColor = Color.White;
            pauseButton.FlatStyle = FlatStyle.Flat;
            pauseButton.Enabled = false;
            pauseButton.Click += PauseButton_Click;

            // Метка счёта
            scoreLabel = new Label();
            scoreLabel.Text = "Счет: 0";
            scoreLabel.Location = new Point(630, 120);
            scoreLabel.Size = new Size(130, 30);
            scoreLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.BackColor = Color.Black;

            // Добавляем элементы на форму
            this.Controls.Add(gamePanel);
            this.Controls.Add(startButton);
            this.Controls.Add(pauseButton);
            this.Controls.Add(scoreLabel);
        }

        private void InitializeGame()
        {
            snake.Clear();
            snake.Add(new Point(15, 12));
            snake.Add(new Point(14, 12));
            snake.Add(new Point(13, 12));

            directionX = 1;
            directionY = 0;
            nextDirectionX = 1;
            nextDirectionY = 0;

            score = 0;
            UpdateScore();

            GenerateFood();

            gameTimer = new Timer();
            gameTimer.Interval = 150;
            gameTimer.Tick += GameTimer_Tick;
        }

        private void SetupKeyboard()
        {
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isGameRunning) return;

            if (isPaused)
            {
                if (e.KeyCode == Keys.Space)
                {
                    TogglePause();
                    e.SuppressKeyPress = true;
                }
                return;
            }

            if (e.KeyCode == Keys.W)
            {
                if (directionY != 1)
                {
                    nextDirectionX = 0;
                    nextDirectionY = -1;
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                if (directionY != -1)
                {
                    nextDirectionX = 0;
                    nextDirectionY = 1;
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                if (directionX != 1)
                {
                    nextDirectionX = -1;
                    nextDirectionY = 0;
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.D)
            {
                if (directionX != -1)
                {
                    nextDirectionX = 1;
                    nextDirectionY = 0;
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Space)
            {
                TogglePause();
                e.SuppressKeyPress = true;
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!isGameRunning || isPaused) return;

            directionX = nextDirectionX;
            directionY = nextDirectionY;

            MoveSnake();

            if (CheckCollision())
            {
                GameOver();
                return;
            }

            if (snake[0].X == food.X && snake[0].Y == food.Y)
            {
                EatFood();
            }

            gamePanel.Invalidate();
        }

        private void MoveSnake()
        {
            Point newHead = new Point(snake[0].X + directionX, snake[0].Y + directionY);
            snake.Insert(0, newHead);

            if (!(newHead.X == food.X && newHead.Y == food.Y))
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private bool CheckCollision()
        {
            Point head = snake[0];

            if (head.X < 0 || head.X >= gridWidth || head.Y < 0 || head.Y >= gridHeight)
                return true;

            for (int i = 1; i < snake.Count; i++)
            {
                if (head.X == snake[i].X && head.Y == snake[i].Y)
                    return true;
            }

            return false;
        }

        private void EatFood()
        {
            score++;
            UpdateScore();
            GenerateFood();
        }

        private void GenerateFood()
        {
            do
            {
                food = new Point(random.Next(0, gridWidth), random.Next(0, gridHeight));
            } while (snake.Contains(food));
        }

        private void UpdateScore()
        {
            scoreLabel.Text = $"Счет: {score}";
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            InitializeGame();
            isGameRunning = true;
            isPaused = false;
            gameTimer.Start();
            startButton.Enabled = false;
            pauseButton.Enabled = true;
            pauseButton.Text = "Пауза";
            this.Focus();
            gamePanel.Invalidate();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            TogglePause();
        }

        private void TogglePause()
        {
            if (!isGameRunning) return;

            isPaused = !isPaused;
            pauseButton.Text = isPaused ? "Продолжить" : "Пауза";
            gamePanel.Invalidate();
            this.Focus();
        }

        private void GameOver()
        {
            gameTimer.Stop();
            isGameRunning = false;
            isPaused = false;
            startButton.Enabled = true;
            pauseButton.Enabled = false;
            pauseButton.Text = "Пауза";
            MessageBox.Show($"Игра окончена!\nВаш счет: {score}", "Game Over",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            gamePanel.Invalidate();
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Pen gridPen = new Pen(Color.FromArgb(60, 60, 60));
            for (int x = 0; x <= gridWidth; x++)
            {
                g.DrawLine(gridPen, x * cellSize, 0, x * cellSize, gridHeight * cellSize);
            }
            for (int y = 0; y <= gridHeight; y++)
            {
                g.DrawLine(gridPen, 0, y * cellSize, gridWidth * cellSize, y * cellSize);
            }

            g.FillEllipse(Brushes.Red,
                food.X * cellSize, food.Y * cellSize,
                cellSize - 1, cellSize - 1);

            for (int i = 0; i < snake.Count; i++)
            {
                Brush brush = (i == 0) ? Brushes.LimeGreen : Brushes.Green;
                g.FillRectangle(brush,
                    snake[i].X * cellSize, snake[i].Y * cellSize,
                    cellSize - 1, cellSize - 1);
            }

            if (isPaused && isGameRunning)
            {
                string pauseText = "ПАУЗА";
                Font font = new Font("Arial", 24, FontStyle.Bold);
                SizeF textSize = g.MeasureString(pauseText, font);
                g.DrawString(pauseText, font, Brushes.White,
                    (gamePanel.Width - textSize.Width) / 2,
                    (gamePanel.Height - textSize.Height) / 2);
                font.Dispose();
            }

            gridPen.Dispose();
        }
    }
}