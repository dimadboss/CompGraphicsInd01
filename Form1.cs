using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CG_ind1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }
        private List<PointF> points = new List<PointF>();
        private PointF S0 = new PointF(0, 0);
        private Bitmap bmp;

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            PointF cur = e.Location;
            if (e.Location.Y > S0.Y)
                S0 = e.Location;
            points.Add(e.Location);

            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Black, 2);
            g.DrawEllipse(pen, cur.X - 1, cur.Y - 1, 2, 2);

            pictureBox1.Image = bmp;
        }

        /// <summary>
        /// Возвращает полярный угол в диапазоне [0; 2pi) относительно точки S0
        /// </summary>
        private double polarAngle(PointF S0, PointF S)
        {
            double res = Math.Atan2(S.Y - S0.Y, S.X - S0.X);
            if (res < 0)
                res += 2 * Math.PI;
            return res;
        }
        /// <summary>
        ///  Находит радиус в полярной системе
        /// </summary>
        private double radius(PointF S0, PointF S)
        {
            return Math.Sqrt((S0.X - S.X) * (S0.X - S.X) + (S0.Y - S.Y) * (S0.Y - S.Y));
        }
        enum Position { Left, Right, Undefined }
        /// <summary>
        /// Определяет положение точки относительно направленного ребра
        /// </summary>
        private Position PointPosition(Tuple<PointF, PointF> edge, PointF b)
        {
            PointF O = edge.Item1;
            PointF a = edge.Item2;
            float sign = (b.X - O.X) * (a.Y - O.Y) - (b.Y - O.Y) * (a.X - O.X);

            if (sign > 0)
                return Position.Left;
            if (sign < 0)
                return Position.Right;
            return Position.Undefined;
        }
        private void Shell(List<PointF> points)
        {
            if (points.Count < 3)
                return;
            points = points.OrderBy(x => polarAngle(S0, x)).ThenBy(x => radius(S0, x)).ToList();


            Graphics g = Graphics.FromImage(bmp);
            Pen pen = new Pen(Color.Black, 2);
            int n = 0;
            foreach (var x in points)
            {
                g.DrawString($"{n++}", new Font(FontFamily.GenericSerif, 9), Brushes.Black, x.X - 15, x.Y - 15);
            }

            List<PointF> shell = new List<PointF>
            {
                points[0],
                points[1]
            };

            for (int i = 2; i < points.Count; ++i)
            {
                // Пока в списке не останется одна точка, не считая текущую
                // Проверяем все предшествующие лучи, что точка слева от них
                while (shell.Count >= 2)
                {
                    PointF p1 = shell[shell.Count - 1];
                    PointF p2 = shell[shell.Count - 2];

                    Tuple<PointF, PointF> edge =
                      Tuple.Create(p1, p2);
                    // Удаляем точки до тех пор, пока точка не станет слева от луча
                    if (PointPosition(edge, points[i]) != Position.Left)
                        shell.RemoveAt(shell.Count - 1);
                    else // Если точка стала слева от луча, то переходим к след. точке
                        break;
                }
                // Добавляем в конец списка
                shell.Add(points[i]);
            }
      
            pen = new Pen(Color.DarkRed, 1);
            for (int i = 0; i < shell.Count - 1; ++i)
            {
                g.DrawLine(pen, shell[i], shell[i + 1]);
            }
            g.DrawLine(pen, shell[shell.Count - 1], shell[0]);
            button1.Enabled = false;
            pictureBox1.Image = bmp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Shell(points);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
            points.Clear();
            S0 = new PointF(0, 0);
            button1.Enabled = true;
        }
    }
}
