using System;
using System.Threading;
using System.Windows.Forms;

namespace FourGame
{
    public partial class frmMain : Form
    {
        private GameBoard game = new GameBoard();

        public frmMain()
        {
            InitializeComponent();
            pnMain.Controls.Add(game);

            game.StateChanged += Game_StateChanged;
        }

        private void Game_StateChanged(object sender, StateChangeEventArg e)
        {
            stbCurrent.Text = String.Format("Current Color: {0}", e.CurrentColor);
            stbMoves.Text = String.Format("Moves: {0}", e.Moves);
            if (!String.IsNullOrEmpty(e.Winner))
            {
                Text = Text + " [ Winner: " + e.Winner + " ]";
                MessageBox.Show(String.Format("Winner: {0}", e.Winner), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            game.Start();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.Start();
            Text = "Four Game";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
