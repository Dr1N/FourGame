using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FourGame
{
    /// <summary>
    /// Game Logic and View together,
    /// they killed MCV =(
    /// </summary>
    public partial class GameBoard : UserControl
    {
        #region Game Parameters

        private readonly int columns = 7;
        
        private readonly int rows = 6;

        private Color firstColor = Color.Yellow;

        private Color secondColor = Color.LightBlue;

        #endregion

        #region Game State

        public event EventHandler<StateChangeEventArg> StateChanged;

        private Cell[,] cells;

        private bool isWon;

        private Color currentColor;

        private Cell currentCell;

        #endregion

        #region Animation

        private Timer animationTimer;

        private bool isAnimate;

        private Rectangle startAnimateRectangle;

        private Rectangle endAnimateRectangle;

        #endregion

        public GameBoard()
        {
            InitializeComponent();
            Width = columns * Cell.CellWidth;
            Height = rows * Cell.CellHeight;
            DoubleBuffered = true;
            animationTimer = new Timer();
            animationTimer.Interval = 10;
            animationTimer.Tick += AnimationTimer_Tick;
        }
        
        /// <summary>
        /// Start new game
        /// </summary>
        public void Start()
        {
            isWon = false;
            isAnimate = false;
            currentColor = firstColor;
            cells = new Cell[columns, rows];
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    Rectangle rectangle = new Rectangle(col * Cell.CellWidth, row * Cell.CellHeight, Cell.CellWidth, Cell.CellHeight);
                    cells[col, row] = new Cell(rectangle);
                }
            }    
            RaiseStateChanged();
            Invalidate();
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Draw(e.Graphics);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            //Check args and left mouse button
            MouseEventArgs args = e as MouseEventArgs;
            if (args == null || args.Button != MouseButtons.Left)
            {
                return;
            }

            DoMove(args.X, args.Y);
        }

        /// <summary>
        /// Draw game
        /// </summary>
        /// <param name="g">Graphics for drawing</param>
        private void Draw(Graphics g)
        {
            g.Clear(Color.White);
            foreach (var cell in cells)
            {
                cell.Draw(g);
            }

            if (isAnimate == true)
            {
                Rectangle animatedRectangle = new Rectangle(startAnimateRectangle.Left + Cell.Delta, startAnimateRectangle.Top + Cell.Delta, startAnimateRectangle.Width - 2 * Cell.Delta, startAnimateRectangle.Height - 2 * Cell.Delta);
                g.FillEllipse(new SolidBrush(currentColor), animatedRectangle);
            }
        }

        /// <summary>
        /// Make a move
        /// </summary>
        /// <param name="x">X click coordinate</param>
        /// <param name="y">Y click coordinate</param>
        private void DoMove(int x, int y)
        {
            //Сan make a move
            if (isAnimate == true || isWon == true || IsFirstRow(x, y) == false)
            {
                return;
            }
            currentCell = GetFreeCellInColumn(GetColumnByCoords(x, y));

            if (currentCell != null)
            {
                Animate(currentCell);
            }
        }

        /// <summary>
        /// Check first row on board by coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <returns>true - if first row, false - otherwise</returns>
        private bool IsFirstRow(int x, int y)
        {
            for (int col = 0; col < columns; col++)
            {
                if (cells[col, 0].Rectangle.Contains(x, y) == true)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Get column number by coordinates
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coodinate</param>
        /// <returns>Index of column</returns>
        private int GetColumnByCoords(int x, int y)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (cells[col, row].Rectangle.Contains(x, y))
                    {
                        return col;
                    }
                }
            }

            throw new ArgumentException(String.Format("No column with coordinates x = {0} y = {1}", x, y));
        }
        
        /// <summary>
        /// Get first free cell in column for move
        /// </summary>
        /// <param name="col">Index of column</param>
        /// <returns>Free cell</returns>
        private Cell GetFreeCellInColumn(int col)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (cells[col, row].IsEmpty == true)
                {
                    return cells[col, row];
                }
            }

            return null;
        }

        /// <summary>
        /// Togle current color
        /// </summary>
        private void Toggle()
        {
            if (isAnimate == true)
            {
                return;
            }
            if (currentColor == firstColor)
            {
                currentColor = secondColor;
            }
            else
            {
                currentColor = firstColor;
            }
        }

        /// <summary>
        /// Check if there is a winner
        /// </summary>
        /// <param name="cell">Last filled cell</param>
        /// <returns>true - if there is a winner, false - otherwise</returns>
        private bool IsWinner(Cell cell)
        {
            //Optimization(?), is no winner until the 7th move
            if (GetMovesCount() < 7)
            {
                return false;
            }
            var indexes = GetCellIndexes(cell);

            return IsHorizontal(indexes.Item1, indexes.Item2)
                || IsVertical(indexes.Item1, indexes.Item2)
                || IsDiagonal(indexes.Item1, indexes.Item2);
        }

        /// <summary>
        /// Get moves count
        /// </summary>
        /// <returns>Moves count</returns>
        private int GetMovesCount()
        {
            var moves = from c in cells.Cast<Cell>()
                        where c.ForeColor == firstColor || c.ForeColor == secondColor
                        select c;

            return moves.Count();
        }

        /// <summary>
        /// Get cell indexes in array
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>Column and row of cell</returns>
        private Tuple<int, int> GetCellIndexes(Cell cell)
        {
            for (int col = 0; col < columns; col++)
            {
                for (int row = 0; row < rows; row++)
                {
                    if (cells[col, row].Equals(cell))
                        return Tuple.Create(col, row);
                }
            }

            return Tuple.Create(-1, -1);
        }

        /// <summary>
        /// Check horizontally for cell 
        /// </summary>
        /// <param name="c">Cell column</param>
        /// <param name="r">Cell row</param>
        /// <returns>true - if there are four cells, false - otherwise</returns>
        private bool IsHorizontal(int c, int r)
        {
            int cnt = 0;
            Cell currentCell = cells[c, r];
            for (int col = 0; col < columns; col++)
            {
                if (cells[col, r].ForeColor == currentCell.ForeColor)
                {
                    if (++cnt >= 4)
                    {
                        return true;
                    }
                }
                else
                {
                    cnt = 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Check verticaly for cell 
        /// </summary>
        /// <param name="c">Cell column</param>
        /// <param name="r">Cell row</param>
        /// <returns>true - if there are four cells, false - otherwise</returns>
        private bool IsVertical(int c, int r)
        {
            int cnt = 0;
            Cell currentCell = cells[c, r];
            for (int row = rows - 1; row >= 0; row--)
            {
                if (cells[c, row].ForeColor == currentCell.ForeColor)
                {
                    if (++cnt >= 4)
                    {
                        return true;
                    }
                }
                else
                {
                    cnt = 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Check diagonales for cell 
        /// </summary>
        /// <param name="c">Cell column</param>
        /// <param name="r">Cell row</param>
        /// <returns></returns>
        private bool IsDiagonal(int c, int r)
        {
            int cnt = 0;
            int curCol;
            int curRow;
            
            //Main Diagonal

            //NW
            curCol = c;
            curRow = r;
            while (curCol >= 0 && curRow >= 0)
            {
                if (cells[curCol, curRow].ForeColor == currentColor)
                {
                    cnt++;
                    if (cnt >= 4)
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
                curCol--;
                curRow--;
            }

            //SE
            curCol = c + 1;
            curRow = r + 1;
            while (curCol < columns && curRow < rows)
            {
                if (cells[curCol, curRow].ForeColor == currentColor)
                {
                    cnt++;
                    if (cnt >= 4)
                    {
                        return true;
                    }
                }
                curCol++;
                curRow++;
            }

            //Second diagonal
            //NE
            cnt = 0;
            curCol = c;
            curRow = r;
            while (curCol < columns && curRow >= 0)
            {
                if (cells[curCol, curRow].ForeColor == currentColor)
                {
                    cnt++;
                    if (cnt >= 4)
                    {
                        return true;
                    }
                }
                curCol++;
                curRow--;
            }

            //SW
            curCol = c - 1;
            curRow = r + 1;
            while (curCol >= 0 && curRow < rows)
            {
                if (cells[curCol, curRow].ForeColor == currentColor)
                {
                    cnt++;
                    if (cnt >= 4)
                    {
                        return true;
                    }
                }
                curCol--;
                curRow++;
            }

            return false;
        }

        /// <summary>
        /// Raise state change event
        /// </summary>
        private void RaiseStateChanged()
        {
            if (StateChanged == null || isAnimate == true)
            {
                return;
            }

            StateChangeEventArg arg = new StateChangeEventArg()
            {
                CurrentColor = currentColor.Name,
                Moves = GetMovesCount(),
            };
            if (isWon == true)
            {
                arg.Winner = (currentColor == firstColor ? secondColor.Name : firstColor.Name);
            }
            else
            {
                arg.Winner = String.Empty;
            }
          
            StateChanged(this, arg);
        }

        /// <summary>
        /// Animate move
        /// </summary>
        /// <param name="cell">Current filled cell</param>
        private void Animate(Cell cell)
        {
            isAnimate = true;
            startAnimateRectangle = new Rectangle(cell.Left, 0, cell.Width, cell.Height);
            endAnimateRectangle = new Rectangle(cell.Left, cell.Top, cell.Width, cell.Height);

            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            startAnimateRectangle = new Rectangle(startAnimateRectangle.Left, startAnimateRectangle.Top + 15, startAnimateRectangle.Width, startAnimateRectangle.Height);
            if (startAnimateRectangle.Top >= endAnimateRectangle.Top + 15)
            {
                isAnimate = false;
                currentCell.ForeColor = currentColor;
                if (IsWinner(currentCell))
                {
                    isWon = true;
                }
                animationTimer.Stop();
                Toggle();
                RaiseStateChanged();
            }
            Invalidate();
        }
    }
}
