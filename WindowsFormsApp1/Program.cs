using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
 
namespace Example
{
    public class Program : Form
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new Program());
        }
        public class coord
        {
            public int X;
            public int Y;
            public coord(int x, int y)
            {
                X = x; Y = y;
            }
        }

        Timer timer = new Timer();
        Random rand = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        int W = 80, H = 60, S = 10;
        // собственно змея: список сегментов(нулевой индекс в списке - голова змеи)  
        List<coord> snake = new List<coord>();
        coord apple;
        int way = 0;
        int apples = 0;
        int stage = 1;
        int score = 0;

        Program()
        {
            this.Text = "Snake";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            int caption_size = SystemInformation.CaptionHeight;
            int frame_size = SystemInformation.FrameBorderSize.Height;
            this.Size = new Size(W * S + 2 * frame_size, H * S + 2 * frame_size + caption_size);
            this.Paint += new PaintEventHandler(Program_Paint);
            this.KeyDown += new KeyEventHandler(Program_KeyDown);
            timer.Interval = 200;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            snake.Add(new coord(W / 2, H - 3));
            snake.Add(new coord(W / 2, H - 2));
            snake.Add(new coord(W / 2, H - 1));
            apple = new coord(rand.Next(W), rand.Next(H));
        }

        void Program_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    if (way != 2)
                        way = 0;
                    break;
                case Keys.Right:
                    if (way != 3)
                        way = 1;
                    break;
                case Keys.Down:
                    if (way != 0)
                        way = 2;
                    break;
                case Keys.Left:
                    if (way != 1)
                        way = 3;
                    break;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int x = snake[0].X, y = snake[0].Y;

            switch (way)
            {
                case 0:
                    y--;
                    if (y < 0)
                        y = H - 1;
                    break;
                case 1:
                    x++;
                    if (x >= W)
                        x = 0;
                    break;
                case 2:
                    y++;
                    if (y >= H)
                        y = 0;
                    break;
                case 3:
                    x--;
                    if (x < 0)
                        x = W - 1;
                    break;
            }

            coord c = new coord(x, y);
            snake.Insert(0, c);
            if (snake[0].X == apple.X && snake[0].Y == apple.Y)
            {
                apple = new coord(rand.Next(W), rand.Next(H));
                apples++;
                score += stage;
                if (apples % 5 == 0)
                {
                    stage++;
                    timer.Interval -= 10;
                }
            }
            else
                snake.RemoveAt(snake.Count - 1);
            for (int i = 2; i < snake.Count; i++)
            {
                if (snake[0].X == snake[i].X && snake[0].Y == snake[i].Y)
                {
                    timer.Stop();
                    MessageBox.Show("Конец!");
                    Close();
                }
            }
            if (snake[0].X == W || snake[0].Y == H || snake[0].X == 0 || snake[0].Y == 0)
            {
                timer.Stop();
                MessageBox.Show("Конец!");
                Close();
            }
            Invalidate();
        }


        void Program_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.FillEllipse(Brushes.Red, new Rectangle(apple.X * S, apple.Y * S, S, S));
            e.Graphics.FillRectangle(Brushes.Yellow, new Rectangle(snake[0].X * S, snake[0].Y * S, S, S));
            for (int i = 1; i < snake.Count; i++)
                e.Graphics.FillRectangle(Brushes.Black, new Rectangle(snake[i].X * S, snake[i].Y * S, S, S));

            string state = "Apples:" + apples.ToString() + "\n" +
                "Stage:" + stage.ToString() + "\n" + "Score:" + score.ToString();

            e.Graphics.DrawString(state, new Font("Arial", 10, FontStyle.Italic), Brushes.Black, new Point(5, 5));
        }
    }
}