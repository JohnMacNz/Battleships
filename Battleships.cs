/* Author: John R. McLaren
 * Created: 15/3/2016
 * Rev: 1.3.4
 * 
 * Source code for Battleships.cs child of Battleships.sln
 *
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Battleships
{
    public partial class frmBattleships : Form
    {
        // initialised at compile
        static Random rng = new Random(); // new Random object
        static int gridSize = 11; // size of grid
        int sqSize = 40; // size of squares within grid
        int marginX = 10; // horizontal margin between the edge of form and grid
        int marginY = 20; // vertical margin between the edge of form and grid
        int[,] grid = new int[gridSize, gridSize]; // rectangular array for holding grid values
        bool gameStarted = false; // flag for checking if game has initialised

        // uninitialised
        int hitScore; // hit value
        int missScore; // miss value
        int winScore; // value needed to finish game
        int shipsRemaining; // number of ships
        int mouseX, mouseY; // horizontal/vertical mouse position
        bool[,] shipPresent; // rectangular array holds true/false value for ship existing
        bool[,] wasSelected; // rectangular array holds true/false value for selection on grid

        // initialises form component **Do not edit**
        public frmBattleships()
        {
            InitializeComponent();
        }

        // called when the start button is clicked
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartGame(); // initialise data and methods
        }

        // called when mouse is moved, gets mouse position and passes mouse coordinates to label
        private void frmBattleships_MouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X; // horizontal mouse coordinate
            mouseY = e.Y; // vertical mouse coordinate

            lblMouseTrack.Text = string.Format("X: {0} Y: {1}", mouseX, mouseY); // change label text to coordinates
        }

        // initialises variables, label text, and methods
        private void StartGame()
        {
            //initialise variables
            gameStarted = true;
            shipPresent = new bool[gridSize, gridSize];
            wasSelected = new bool[gridSize, gridSize];
            hitScore = 0;
            missScore = 0;
            winScore = 0;

            //set control properties
            lblResult.Text = "";
            btnStart.Text = "Start Over";

            //call methods
            ShowLabels();
            GenerateGrid();
            DrawShips();
            UpdateScore();
        }

        // mouse click event logic, called each time the mouse is clicked
        private void frmBattleships_MouseClick(object sender, MouseEventArgs e)
        {
            mouseX = e.X; // horizontal mouse coordinate
            mouseY = e.Y; // vertical mouse coordinate

            int[] coordinates = ReturnMouseCoodinates(mouseX, mouseY); // gets mouse coordinates and stores in array

            if (gameStarted == true) // if game has started
            {
                if (mouseX >= sqSize + marginX && mouseX <= sqSize * gridSize + marginX) // and if between left and right grid border
                {
                    if (mouseY >= sqSize + marginY && mouseY <= sqSize * gridSize + marginY) // and if between top and bottom grid border
                    {
                        if (wasSelected[coordinates[0], coordinates[1]] == false) // and if hasnt been selected yet
                        {
                            if (shipPresent[coordinates[0], coordinates[1]] == true) // and if ship at this location
                            {
                                hitScore++; // increase hitscore by 1
                                lblResult.Text = "Hit!"; // change label text to hit
                                wasSelected[coordinates[0], coordinates[1]] = true; // set selection to true
                                DrawSquare(coordinates[0], coordinates[1], Color.Red); // change square to red

                                if (hitScore == winScore) // and if hit score is equivalent to winscore
                                {
                                    UpdateScore(); // update score data
                                    gameStarted = false; // set game to not started
                                    MessageBox.Show(string.Format("Congratulations! You found all the ships. Your score was {0} hits, and {1} misses.", hitScore, missScore)); // display message box
                                }
                            }
                            else // if ship not at this location
                            {
                                missScore++; // increase miss score by 1
                                lblResult.Text = "Miss"; // change label text to miss
                                wasSelected[coordinates[0], coordinates[1]] = true; // set selection to true
                                DrawSquare(coordinates[0], coordinates[1], Color.White); // change square to white
                            }
                        }
                        else // if previously selected
                        {
                            if (shipPresent[coordinates[0], coordinates[1]] == true) // and if ship present
                            {
                                DrawSquare(coordinates[0], coordinates[1], Color.Red); // update square color
                                lblResult.Text = ""; // change label text to blank
                            }
                            else // if ship not present
                            {
                                DrawSquare(coordinates[0], coordinates[1], Color.White); // update square color
                                lblResult.Text = ""; // change label text to blank
                            }
                        }
                    }
                    else // if not inside grid
                    {
                        lblResult.Text = "";
                    }
                }
                else // if not inside grid
                {
                    lblResult.Text = "";
                }
            }
            UpdateScore(); // update score/hit/miss values
        }

        // update label text to current scores
        private void UpdateScore()
        {
            lblHitScore.Text = "     " + hitScore + "     ";
            lblMissScore.Text = "     " + missScore + "     ";
            lblShipScore.Text = "     " + shipsRemaining + "     ";
        }

        // finds all labels within the form and makes them visible if they are enabled
        private void ShowLabels()
        {
            foreach (Control c in Controls)
            {
                Label lbl = c as Label; // label object
                if (lbl != null && lbl.Enabled == true) // if exists and enabled
                {
                    lbl.Visible = true; // set to visible
                }
            }
        }

        // gets an input and returns coordinates that correspond to a square on the grid
        private int[] ReturnMouseCoodinates(int mouseX, int mouseY)
        {
            int[] values = new int[2]; // array holding 2 values

            for (int x = 1; x <= 10; x++) // loop 10 iterations
            {
                for (int y = 1; y <= 10; y++) // loop another 10 iterations per parent iteration
                {
                    if (mouseX >= sqSize * x + marginX && mouseX <= sqSize * x + marginX + sqSize) // if mouseX is between coord x min/max
                    {
                        if (mouseY >= sqSize * y + marginY && mouseY <= sqSize * y + marginY + sqSize) // and if mouseY is between coord y min/max
                        {
                            values[0] = x; // set array index 0 to x
                            values[1] = y; // set array index 1 to y
                        }
                    }
                }
            }
            return values;
        }

        // generates random value for the number of ships that will be present then draws them
        private void DrawShips()
        {
            int shipTotal = rng.Next(2, 6); // random int 2 <-> 5
            shipsRemaining = shipTotal; // ship remaining value set to ship total value
            
            for (int x = 0; x < shipTotal; x++) // loop 2 <-> 5 iterations
            {
                GenerateShip(); // call generate ship method
            }
        }

        // ship rendering logic, draws ships
        private void GenerateShip()
        {
            int posX = 0; // horizontal coordinate
            int posY = 0; // vertical coordinate

            int[] shipData = GenerateShipData(); // set index values to ship data values

            foreach (int i in grid) // loop through grid
            {
                if (posX == shipData[0] && posY == shipData[1]) // if coord X and Y equal to square on grid
                {
                    if (!shipPresent[posX, posY]) // and if ship not present
                    {
                        DrawSquare(posX, posY, Color.Transparent);  // draw head of ship
                        winScore++;                         // increase win score by 1

                        shipPresent[posX, posY] = true; // store location
                    }

                    for (int j = 1; j < shipData[2]; j++) // while iteration count is less than ship length draw rest of ship
                    {
                        if ((posY + j) * sqSize < sqSize * gridSize && posX + j < gridSize && posX - j >= 1) // stop from rendering outside grid
                        {
                            if (shipData[3] == 1)       //  if vertical : top to bottom
                            {
                                if (!shipPresent[posX, posY + j])
                                {
                                    DrawSquare(posX, posY + j, Color.Transparent); // draw square
                                    shipPresent[posX, posY + j] = true; // set ship present to true at this location
                                    winScore++; // increase win score by 1
                                }
                            }
                            else if (shipData[3] == 2)  // horizontal : left to right
                            {
                                if (!shipPresent[posX + j, posY])
                                {
                                    DrawSquare(posX + j, posY, Color.Transparent);
                                    shipPresent[posX + j, posY] = true;
                                    winScore++;
                                }
                            }
                            else if (shipData[3] == 3)  // diagonal : SouthEast
                            {
                                if(!shipPresent[posX + j, posY + j])
                                {
                                    DrawSquare(posX + j, posY + j, Color.Transparent);
                                    shipPresent[posX + j, posY + j] = true;
                                    winScore++;
                                }
                            }
                            else                        // diagonal : SouthWest
                            {
                                if (!shipPresent[posX - j, posY + j])
                                {
                                    DrawSquare(posX - j, posY + j, Color.Transparent);
                                    shipPresent[posX - j, posY + j] = true;
                                    winScore++;
                                }
                            }
                        }
                    }
                }
                posX++; // next square

                if (posX == gridSize) // if end of row
                {
                    posX = 0; // reset posX
                    posY++; // next row
                }
            }
        }

        // generates randomised coordinates, size, and orientation of a ship
        private int[] GenerateShipData()
        {
            int shipLocX = rng.Next(1, 11);  // 1 <-> 10
            int shipLocY = rng.Next(1, 11);  // 1 <-> 10
            int shipSize = rng.Next(2, 6);   // 2 <-> 5
            int shipPos = rng.Next(1, 5);  // 1 <-> 4

            int[] shipData = { shipLocX, shipLocY, shipSize, shipPos }; // assign array values

            return shipData; // return array values
        }

        // grid rendering logic, draws grid
        private void GenerateGrid()
        {
            int posX = 0; // horizontal position
            int posY = 0; // vertical position

            foreach (int i in grid) // loop through grid
            {
                if (posX == 0 && posY == 0) // if first row AND column
                {
                    // do nothing
                }
                else if (posX == 0 || posY == 0) // if first row OR column
                {
                    if (posX == 0) // and if first column
                    {
                        DrawText(posX, posY, posY - 1); // draw text at given position
                    }
                    else // first row
                    {
                        DrawText(posX, posY, posX - 1); // draw text at given position
                    }
                }
                else // if not first row or column draw square
                {
                    DrawSquare(posX, posY, Color.Cyan);
                }

                posX++; // next square

                if (posX == gridSize) // if reached end of row
                {
                    posX = 0; // reset posX
                    posY++; // next row
                }
            } // end loop
        }

        // draws square at given location
        private void DrawSquare(int posX, int posY, Color rgb)
        {
            Graphics graphicObj = CreateGraphics(); // create graphics object
            Pen myPen = new Pen(rgb); // create pen object
            Rectangle myRectangle = new Rectangle(posX * sqSize + marginX, posY * sqSize + marginY, sqSize, sqSize); // create square object

            graphicObj.DrawRectangle(myPen, myRectangle); // draw square

            myPen.Dispose(); // erase pen object
            graphicObj.Dispose(); // erase graphics object
        }

        // draws text at given location
        private void DrawText(int posX, int posY, int counter)
        {
            string drawString = counter.ToString(); // counter value stored as string

            Graphics graphicObj = this.CreateGraphics(); // create graphics object
            Font drawFont = new Font("Arial", 14); // create text font property
            SolidBrush drawBrush = new SolidBrush(Color.Cyan); // create brush object
            StringFormat drawFormat = new StringFormat(); // create text format

            graphicObj.DrawString(drawString, drawFont, drawBrush, posX * sqSize + marginX + 10, posY * sqSize + marginY + 10, drawFormat); // draw text

            drawFont.Dispose(); // erase font property
            drawBrush.Dispose(); // erase brush object
            graphicObj.Dispose(); // erase graphics object
            drawFormat.Dispose(); // erase text format
        }
    }
}