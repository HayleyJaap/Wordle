using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaapHA2
{
    class GameLogic
    {
        private List<string> wordList = new List<string>();
        private string answer;
        private int guessCount = 0;
        private bool won = false;
        private bool firstGame = true;

        /// <summary>
        /// Control hub to set up game at the start
        /// </summary>
        /// <param name="letterArray"></param>
        public void Run(System.Windows.Forms.TextBox[,] letterArray)
        {
            ResetGame(letterArray);
            ChooseWord(wordList);
        }

        /// <summary>
        /// Selects answer from a list of words
        /// </summary>
        /// <param name="wordList"></param>
        public void ChooseWord(List<string> wordList)
        {
            var random = new Random(DateTime.Now.Millisecond);

            //checks if file needs to be read to fill wordList
            if (firstGame)
            {
                ReadFile(wordList);
                firstGame = false;
            }

            //randomly select an answer from the word list and set to uppercase
            answer = wordList[random.Next(0, wordList.Count)];
            answer = answer.ToUpper();
        }

        /// <summary>
        /// Reads the file containing answers for Wordle
        /// </summary>
        /// <param name="wordList"></param>
        public void ReadFile(List<string> wordList)
        {
            try
            {
                //open file
                StreamReader reader = new StreamReader("wordle-answers-alphabetical.txt");

                //read data into the word list
                while(!reader.EndOfStream)
                {
                    wordList.Add(reader.ReadLine());
                }
            }
            //file not found
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Method acts as a control hub to testing each guess
        /// </summary>
        /// <param name="letterArray"></param>
        /// <param name="guessBtn"></param>
        public void Guess(System.Windows.Forms.TextBox[,] letterArray, System.Windows.Forms.Button guessBtn)
        {
            char[] answerChar = answer.ToCharArray();
            char[] guess = new char[5];
            bool validWord = false;

            //checks if each textbox in the guess row is full
            bool full = CheckGuessLength(letterArray);

            if(full)
            {
                //hold the current guess in a char array for easy testing
                for (int i = 0; i < letterArray.GetLength(1); i++)
                {
                    guess[i] = char.Parse(letterArray[guessCount, i].Text.ToUpper());
                }

                //check if guess is a valid word
                validWord = checkGuessWord(letterArray, guess);

                if (validWord)
                {
                    guessCount++;

                    //enables next row of text boxes if not the end of the game
                    if (guessCount < 6)
                    {
                        OpenNextRow(letterArray);
                    }

                    //checks the guess for equality to the answer
                    CheckGuess(letterArray, answerChar, guess, guessBtn);

                    //Inform user they lost and calls method to disable the game
                    if (guessCount == 6 && !won)
                    {
                        System.Windows.Forms.MessageBox.Show($"You lost! The correct answer was {answer}");
                        DisableGame(letterArray, guessBtn);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Guess must be a valid word");
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Guess must be 5 letters long");
            }  
        }

        /// <summary>
        /// Checks to see if guess is equal to the answer and updates results to the screen
        /// </summary>
        /// <param name="letterArray"></param>
        /// <param name="answerChar"></param>
        /// <param name="guess"></param>
        /// <param name="guessBtn"></param>
        public void CheckGuess(TextBox[,] letterArray, char[] answerChar, char[] guess, Button guessBtn)
        {
            if (answerChar.SequenceEqual(guess))
            {
                //update background color of each text box in the correct guess to green
                for (int i = 0; i < letterArray.GetLength(1); i++)
                {
                    letterArray[guessCount - 1, i].BackColor = System.Drawing.Color.LimeGreen;
                }
                //inform user they won and disable the game
                won = true;
                System.Windows.Forms.MessageBox.Show("You guessed correctly!");
                DisableGame(letterArray, guessBtn);
            }
            else
            {
                //save answer in temp variable so it can be altered
                string temp = answerChar.ToString();
                //Guess was not correct so now we check each individual letter and update text box colors
                for (int i = 0; i < letterArray.GetLength(1); i++)
                {
                    //correct letter in correct spot
                    if (answerChar[i].Equals(guess[i]))
                    {
                        letterArray[guessCount - 1, i].BackColor = System.Drawing.Color.LimeGreen;
                    }
                    //correct letter but in the wrong spot
                    else if (temp.Contains(guess[i]))
                    {
                        //remove the letter that was found from temp to prevent duplicates from being colored
                        temp.Remove(temp.IndexOf(guess[i]), 1);

                        letterArray[guessCount - 1, i].BackColor = System.Drawing.Color.Yellow;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the guess is a valid word
        /// </summary>
        /// <param name="letterArray"></param>
        /// <param name="guess"></param>
        /// <returns></returns>
        public bool checkGuessWord(System.Windows.Forms.TextBox[,] letterArray, char[] guess)
        {
            bool validWord = false;

            //compares the guess to the word list to see if it is a valid word
            for (int i = 0; i < wordList.Count; i++)
            {
                if (guess.SequenceEqual(wordList[i].ToUpper()))
                {
                    validWord = true;
                }
            }

            return validWord;
        }

        /// <summary>
        /// Checks if the guess was 5 letters long
        /// </summary>
        /// <param name="letterArray"></param>
        /// <returns></returns>
        public bool CheckGuessLength(System.Windows.Forms.TextBox[,] letterArray)
        {
            bool full = true;

            //checks if each text box contains exactly 1 character
            for (int i = 0; i < letterArray.GetLength(1); i++)
            {
                if(letterArray[guessCount, i].TextLength < 1)
                {
                    full = false;
                }
            }

            return full;
        }

        /// <summary>
        /// Enables the next row of text boxes for each guess
        /// </summary>
        /// <param name="letterArray"></param>
        public void OpenNextRow(System.Windows.Forms.TextBox[,] letterArray)
        {
            for (int i = 0; i < letterArray.GetLength(1); i++)
            {
                letterArray[guessCount, i].Enabled = true;
            }
        }

        /// <summary>
        /// Resets the game for replayability
        /// </summary>
        /// <param name="letterArray"></param>
        public void ResetGame(System.Windows.Forms.TextBox[,] letterArray)
        {
            //reset variables
            guessCount = 0;
            won = false;

            //reset textboxes
            for (int row = 0; row < letterArray.GetLength(0); row++)
            {
                for (int col = 0; col < letterArray.GetLength(1); col++)
                {
                    letterArray[row, col].Text = null;
                    letterArray[row, col].Enabled = false;
                    letterArray[row, col].BackColor = System.Drawing.Color.Silver;
                }
            }
        }

        /// <summary>
        /// Disables guess button and text boxes when game is finished
        /// </summary>
        /// <param name="letterArray"></param>
        /// <param name="guessBtn"></param>
        public void DisableGame(System.Windows.Forms.TextBox[,] letterArray, Button guessBtn)
        {
            guessBtn.Enabled = false;
            for (int row = 0; row < letterArray.GetLength(0); row++)
            {
                for (int col = 0; col < letterArray.GetLength(1); col++)
                {
                    letterArray[row, col].Enabled = false;
                }
            }
        }
       
    }
}
