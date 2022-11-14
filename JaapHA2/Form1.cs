using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaapHA2
{
    public partial class Form1 : Form
    {
        private TextBox[,] letterArray = new TextBox[6, 5];

        //create instance of the game logic class
        GameLogic game = new GameLogic();

        /// <summary>
        /// main method for the GUI
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            //auto sizes the window to fit the wordle controls
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.AutoSize = true;

            InstantiateArray();
        }

        /// <summary>
        /// Initializes the text boxes and adds them to an array of text boxes
        /// </summary>
        public void InstantiateArray()
        {
            for (int row = 0; row < letterArray.GetLength(0); row++)
            {
                for (int col = 0; col < letterArray.GetLength(1); col++)
                {
                    //create the new textbox and initialize all of the important features
                    TextBox temp = new TextBox();
                    temp.Size = new Size(32, 32);
                    temp.BackColor = Color.Silver;
                    temp.MaxLength = 1;
                    temp.TabIndex = row * letterArray.GetLength(1) + col;
                    temp.Enabled = false;
                    temp.Location = new Point(col * (temp.Size.Width + 4), 48 + row * (temp.Size.Height + 10));

                    //add to the array and to the controls
                    letterArray[row, col] = temp;
                    this.Controls.Add(temp);
                }
            }
        }

        /// <summary>
        /// Displays message box with game rules when the menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void HowToPlayMenuItemClicked(object sender, EventArgs e)
        {
            MessageBox.Show("Objective: Correctly guess the word\n\nRules:\n1.You have six guesses\n2.Each guess must be 5 letters long\n3.Each guess must be a real word\n\n " +
                "For every guess submitted:\n -If the letter is in the word and in the right spot the background will turn green\n " +
                "-If the letter is in the word and in the wrong spot the background will turn yellow" +
                "\n -If the letter is not present in the word its background color will not change");

        }

        /// <summary>
        /// Call guess method in game logic when guess button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void GuessBtnClicked(object sender, EventArgs e)
        {
            game.Guess(letterArray, guessBtn);
        }

        /// <summary>
        /// Show message box with about information when the menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Programmed for CS3020, Assignment #2\n\nThis game is legally distinct from Wordle and thus not a copyright infringement.");
        }

        /// <summary>
        /// Starts the game by enabling guess button and first row of text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMenuItem_Click(object sender, EventArgs e)
        {
            guessBtn.Enabled = true;
            game.Run(letterArray);

            for (int i = 0; i < letterArray.GetLength(1); i++)
            {
                letterArray[0, i].Enabled = true;
            }
        }
    }
}
