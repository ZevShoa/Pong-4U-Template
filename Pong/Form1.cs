/*
 * Description:     A basic PONG simulator
 * Author: Zev Shoag              
 * Date:   Febuary 8 2016         
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using WMPLib;


#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //paddle position variables
        int paddle1Y, paddle2Y;

        //movment of the paddle
        bool paddle1Up, paddle2Up;
        //ball position variables
        int ballX, ballY;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball directions
        Boolean ballMoveRight = false;
        Boolean ballMoveDown = false;

        //constants used to set size and speed of paddles 
        const int PADDLE_LENGTH = 40;
        const int PADDLE_WIDTH = 10;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle
        const int PADDLE_SPEED = 4;

        //constants used to set size and speed of ball 
        const int BALL_SIZE = 10;
        const int BALL_SPEED = 4;

        //player scores
        int player1Score = 0;
        int player2Score = 0;

        //determines whether a key is being pressed or not
        Boolean aKeyDown, zKeyDown, jKeyDown, mKeyDown;

        //game winning score
        int gameWinScore = 2;

        //brush for paint method
        SolidBrush drawBrush = new SolidBrush(Color.White);

        //random number generator to make it a bit tripy
        Random randNum = new Random();

        int colourChange1 = 0;
        int colourChange2 = 0;
        int colourChange3 = 0;

        WindowsMediaPlayer mp3Player;
       
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetParameters();        
        }

        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = true;
                    break;
                case Keys.Z:
                    zKeyDown = true;
                    break;
                case Keys.J:
                    jKeyDown = true;
                    break;
                case Keys.M:
                    mKeyDown = true;
                    break;
                case Keys.Y:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                case Keys.N:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
                case Keys.Space:
                    if (newGameOk)
                    {
                        GameStart();
                    }
                    break;
                default:
                    break;
            }
        }
        
        // -- YOU DO NOT NEED TO MAKE CHANGES TO THIS METHOD
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.A:
                    aKeyDown = false;
                    break;
                case Keys.Z:
                    zKeyDown = false;
                    break;
                case Keys.J:
                    jKeyDown = false;
                    break;
                case Keys.M:
                    mKeyDown = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets up the game objects in their start position, resets the scores, displays a 
        /// countdown, and then starts the game timer.
        /// </summary>
        private void GameStart()
        {
            mp3Player = new WindowsMediaPlayer();
            mp3Player.URL = "The Many Sounds of Mr. Trump.mp3";
            mp3Player.settings.setMode("loop", true);
            newGameOk = true;
            SetParameters();

            
            //  create code to make a graphics object, a brush, and a font to display the countdown

            startLabel.Visible = false;
            Refresh();

            //countdown to start of game
            Graphics formGraphics = this.CreateGraphics();
            Font drawFont = new Font("Courier New", 82, FontStyle.Regular);
            SolidBrush drawBrush = new SolidBrush(Color.White);
            for (int i = 3; i > 0; i--) //  create conditions for a for loop that counts down from 3 
            {
                
                formGraphics.DrawString(Convert.ToString(i), drawFont, drawBrush, 250, 170);
                Thread.Sleep(1000);
                Refresh();

                // --- create code using DrawString to display the countdown in the appropriate area.  
                // --- sleep for 1 second
                // --- refresh the screen
            }

            //  start the gameUpdateLoop timer
            newGameOk = false;
            gameUpdateLoop.Enabled = true;
        }

        

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                // sets location of score labels to the middle of the screen
                player1Label.Location = new Point(this.Width / 2 - player1Label.Size.Width - 10, player1Label.Location.Y);
                player2Label.Location = new Point(this.Width / 2 + 10, player2Label.Location.Y);

                //set label, score variables, and ball position
                player1Score = player2Score = 0;
                player1Label.Text = "Player 1:  " + player1Score;
                player2Label.Text = "Player 2:  " + player2Score;

                paddle1Y = paddle2Y = this.Height / 2 - PADDLE_LENGTH / 2;

            }

            //  set starting X position for ball to middle of screen, (use this.Width and BALL_SIZE)
            ballX = this.Width/2-BALL_SIZE;
            //  set starting Y position for ball to middle of screen, (use this.Height and BALL_SIZE)
            ballY = this.Height / 2 - BALL_SIZE;
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            colourChange1 = randNum.Next(1, 255);
            colourChange2 = randNum.Next(1, 255);
            colourChange3 = randNum.Next(1, 255);
            //sound player to be used for all in game sounds initially set to collision sound
            SoundPlayer player = new SoundPlayer();
            player = new SoundPlayer(Properties.Resources.collision);

            #region update ball position

            //  create code to move ball either left or right based on ballMoveRight and BALL_SPEED
            if (ballMoveRight == true)
            {
                ballX += BALL_SPEED;
            }
           else if(ballMoveRight == false)
            {
                ballX -= BALL_SPEED;
            }
                //  create code move ball either down or up based on ballMoveDown and BALL_SPEED
            if (ballMoveDown == true)
            {
                ballY += BALL_SPEED;
            }
            else if (ballMoveDown == false)
            {
                ballY -= BALL_SPEED;
            }
            #endregion

            #region update paddle positions

            if (aKeyDown == true && paddle1Y > 0)
            {
                //  create code to move player 1 paddle up using paddle1Y and PADDLE_SPEED
                paddle1Y -= PADDLE_SPEED;
                paddle1Up = true;
            }
            //  create an if statement and code to move player 1 paddle down using paddle1Y and PADDLE_SPEED
            if (zKeyDown == true&& paddle1Y <this.Height - PADDLE_LENGTH)
            {
                paddle1Y += PADDLE_SPEED;
                paddle1Up = false;
            }

            //  create an if statement and code to move player 2 paddle up using paddle2Y and PADDLE_SPEED
            if (jKeyDown == true && paddle2Y > 0)
            {
                paddle2Y -= PADDLE_SPEED;
                paddle2Up = true;
            }
            //  create an if statement and code to move player 2 paddle down using paddle2Y and PADDLE_SPEED
            if (mKeyDown == true && paddle2Y < this.Height - PADDLE_LENGTH)
            {
                paddle2Y += PADDLE_SPEED;
                paddle2Up = false;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ballY <= 0) // if ball hits top line
            {
                //  use ballMoveDown boolean to change direction
                player.Play();
                ballMoveDown = true;
                //  play a collision sound
            }
            //  In an else if statement use ballY, this.Height, and BALL_SIZE to check for collision with bottom line
            // If true use ballMoveDown down boolean to change direction
            if(ballY + BALL_SIZE>= this.Height)
            {
                player.Play();
                ballMoveDown = false;
            }

            #endregion

            #region ball collision with paddles
             

            if (ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH && ballX < PADDLE_EDGE + PADDLE_WIDTH && paddle1Up == true && ballMoveDown == true) // left paddle collision
            {
                player.Play();
                ballMoveRight = true;
                ballMoveDown = false;
            }
            else if(ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH && ballX < PADDLE_EDGE + PADDLE_WIDTH && paddle1Up == false && ballMoveDown == false)
            {
                player.Play();
                ballMoveRight = true;
                ballMoveDown = true;
            }
            else if (ballY > paddle1Y && ballY < paddle1Y + PADDLE_LENGTH && ballX < PADDLE_EDGE + PADDLE_WIDTH)
            {
                //  play a "paddle hit" sound 
                player.Play();
                //  use ballMoveRight boolean to change direction
                ballMoveRight = true;
            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2 && paddle2Up == true && ballMoveDown == true)
            {
                player.Play();
                ballMoveRight = false;
                ballMoveDown = false;
            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2 && paddle2Up == false && ballMoveDown == false)
            {
                player.Play();
                ballMoveRight = false;
                ballMoveDown = true;
            }
            else if (ballY > paddle2Y && ballY < paddle2Y + PADDLE_LENGTH && ballX + BALL_SIZE > this.Width - PADDLE_EDGE - PADDLE_WIDTH / 2) // right paddle collision
            {
                //  play a "paddle hit" sound
                player.Play(); 
                //  use ballMoveRight boolean to change direction
                ballMoveRight = false;
            }


            #endregion

            #region ball collision with side walls (point scored)

            player = new SoundPlayer(Properties.Resources.score);

            if (ballX < 0)  //  ball hits left wall logic
            {
                // --- play score sound
                player.Play();
                // --- update player 2 score
                player2Score++;
                // --- update player2Label with new score
                player2Label.Text = Convert.ToString(player2Score);
                // --- refresh
                Refresh();

                // --- use if statement to check to see if player 2 has won the game. If true run
                // gameWinScore method. Else call SetParameters method to reset ball position.
                if(player2Score == gameWinScore)
                {
                    GameOver(Name = "Player 2");
                }
                else if (player2Score == 1)
                {
                    SetParameters();
                }

            }

            //  same as above but this time check for collision with the right wall
            if (ballX + BALL_SIZE > this.Width)  //  ball hits right wall logic
            {
                // --- play score sound
                player.Play();
                // --- update player 2 score
                player1Score++;
                // --- update player2Label with new score
                player1Label.Text = Convert.ToString(player1Score);
                // --- refresh
                Refresh();

                // --- use if statement to check to see if player 1 has won the game. If true run 
                // gameWinScore method. Else call SetParameters method to reset ball position.
                if (player1Score == gameWinScore)
                {
                    GameOver(Name = "Player 1");
                }
                else if (player1Score == 1)
                {
                    SetParameters();
                }

            }
            #endregion

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        private void GameOver(string winner)
        {
           
            newGameOk = true;

            //  create game over logic
            // --- stop the gameUpdateLoop
            gameUpdateLoop.Enabled = false;
            // --- show a message on the startLabel to indicate a winner
            startLabel.Visible = true;
            startLabel.Text = Name + " Is winner";
            Refresh();
            // --- pause for two seconds 
            Thread.Sleep(2000);
            Refresh();
            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text = "Would you like to play again? (y/n)";

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //  draw paddles using FillRectangle
            drawBrush.Color = Color.FromArgb(colourChange1, colourChange2, colourChange3);
            e.Graphics.FillRectangle(drawBrush, PADDLE_EDGE, paddle1Y, PADDLE_WIDTH, PADDLE_LENGTH);
            e.Graphics.FillRectangle(drawBrush, this.Width-PADDLE_WIDTH-PADDLE_EDGE, paddle2Y, PADDLE_WIDTH, PADDLE_LENGTH);
            //  draw ball using FillRectangle
            e.Graphics.FillRectangle(drawBrush, ballX, ballY, BALL_SIZE, BALL_SIZE);
        }

    }
}
