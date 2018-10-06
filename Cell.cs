using System;
using System.Drawing;

namespace FourGame
{
    public class Cell : IDrawable
    {
        private static int cellWidth = 60;

        public static int CellWidth
        {
            get
            {
                return cellWidth;
            }
        }

        private static int cellHeight = 60;

        public static int CellHeight
        {
            get
            {
                return cellHeight;
            }
        }

        private static readonly int delta = 5;

        public static int Delta
        {
            get
            {
                return delta;
            }
        }

        private static readonly float circleBorderWidth = 3.0f;

        public static float CirleBorder 
        {
            get
            {
                return circleBorderWidth;
            }
        }

        private static Color emptyColor = Color.Black;

        public static Color EmptyColor
        {
            get
            {
                return emptyColor;
            }
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Left, Top, Width, Height);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return ForeColor == emptyColor;
            }
        }

        public Color BackColor { get; set; }

        public Color ForeColor { get; set; }

        public Color BorderColor { get; set; }

        public Cell()
        {
            Width = Cell.CellWidth;
            Height = Cell.CellHeight;
            ForeColor = emptyColor;
            BackColor = Color.FromArgb(0x60, 0x67, 0xff);
            BorderColor = Color.FromArgb(0x0f, 0x1a, 0x93);
        }

        public Cell(Rectangle r) : this()
        {
            Top = r.Top;
            Left = r.Left;
        }

        public void Draw(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException("Graphics is null");
            }
            
            //Background
            Brush rectangleBrush = new SolidBrush(BackColor);
            g.FillRectangle(rectangleBrush, Left, Top, Width, Height);

            //Cirle
            Brush circleBrush = new SolidBrush(ForeColor);
            Rectangle circleRectangle = new Rectangle(Left + delta, Top + delta, Width - 2 * delta, Height - 2 * delta);
            g.FillEllipse(circleBrush, circleRectangle);

            //Circle Border
            Pen circlePen = new Pen(BorderColor, circleBorderWidth);
            g.DrawEllipse(circlePen, circleRectangle);
        }
        
        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, Color = {2}", Left, Top, ForeColor);
        }
    }
}
