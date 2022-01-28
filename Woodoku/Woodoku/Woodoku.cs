using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Woodoku
{
    public partial class Form1 : Form
    {
        int createChoicesAttempts = 0;
        int score = 0;
        int spaceScore;
        int gameSize = 40;
        int windowHeight = 600, windowWidth = 500;
        Button gameOver;
        int round = 1,  streak = 0;
        bool canPlace;
        int Lo_i, Lo_j;
        bool[,] board = new bool[9,9];
        List<Button> buttons = new List<Button>();
        Label[,] boardDisplay = new Label[9, 9];
        List<Shape> shapes = new List<Shape>();
        List<Shape> choices = new List<Shape>(); // contains all choices from that round
        List<Shape> lessChoices = new List<Shape>(); // contains only unused choices
        Random rand = new Random();
        int selectedButton;
        Shape selectedShape = new Shape();
        GameState game;
        public Form1()
        {
            InitializeComponent();
            init();
        }
        public void init()
        {
            clearEventHandlers();
            score = 0;
            round = 1;
            lblRound.Text = "Round: " + round;
            lblScore.Text = "Score: " + score;
            initShapes();
            //Initializes the boardDisplay as a 2D array of labels, this will be used to display blocks on the board.
            //Also sets all values in the board 2D array to false. if true than a block is there and that spot will be displayed.
            for (int i = 0; i < boardDisplay.GetLength(0); i++)
            {
                for (int j = 0; j < boardDisplay.GetLength(1); j++)
                {
                    Piece p = new Piece(i * gameSize + windowWidth / 2 - boardDisplay.GetLength(0) * gameSize / 2, j * gameSize + windowHeight / 2 - boardDisplay.GetLength(1) * gameSize / 2 - 60);
                    boardDisplay[i, j] = p;
                    boardDisplay[i,j].Click += new EventHandler(setSelectedLocation_OnClick);
                    boardDisplay[i, j].MouseMove += new MouseEventHandler(createView_WhenMouseOver);
                    boardDisplay[i, j].MouseEnter += new EventHandler(clearView_WhenMouseLeave);
                    Controls.Add(boardDisplay[i, j]);
                    board[i, j] = false;
                }
            }
            createChoices();
            buttons.Add(button1);
            buttons.Add(button2);
            buttons.Add(button3);
            button1.Click += new EventHandler(setSelectedPiece_OnPress);
            button2.Click += new EventHandler(setSelectedPiece_OnPress);
            button3.Click += new EventHandler(setSelectedPiece_OnPress);
            spaceScore = calculateSpaceScore();
            lblSpaceScore.Text = "Space Score: " + spaceScore;
            //Sets main game
            game = new GameState(board, activeButtons(), choices, score, spaceScore, true);
        }
        private void clearView_WhenMouseLeave(object sender, EventArgs e)
        {
            clearView();
        }//Clears the past views whenever the mouse moves over a new label
        private void createView_WhenMouseOver(object sender, EventArgs e)//Creates a "greyed" view of where your block would be placed
        {
            Piece p = sender as Piece;
            Lo_i = (p.getX() - windowWidth / 2 + boardDisplay.GetLength(0) * gameSize / 2) / gameSize;
            Lo_j = (p.getY() - windowHeight / 2 + boardDisplay.GetLength(1) * gameSize / 2 + 60) / gameSize;
            changeBoard(selectedShape, Lo_i, Lo_j);
        }
        private void setSelectedLocation_OnClick(object sender, EventArgs e)//places the blocks on the board by changing board values to true
        {
            Piece p = sender as Piece;
            Lo_i = (p.getX() - windowWidth / 2 + boardDisplay.GetLength(0) * gameSize / 2) / gameSize;
            Lo_j = (p.getY() - windowHeight / 2 + boardDisplay.GetLength(1) * gameSize / 2 + 60) / gameSize;
            placeShape(Lo_i, Lo_j, game);
        }
        private void gameOverButton(object sender, EventArgs e)
        {
            for (int i = 0; i < boardDisplay.GetLength(0); i++)//Removes all the labels of the board from the Display since a new ones will be created
            {
                for (int j = 0; j < boardDisplay.GetLength(1); j++)
                {
                    Controls.Remove(boardDisplay[i, j]);
                }
            }
            Controls.Remove(gameOver);
            init();
        }
        private void setSelectedPiece_OnPress(object sender, EventArgs e)
        {
            Button b = sender as Button;
            String bname = b.Name;
            if (bname == "button1")
            {
                selectedShape = choices[0];
                selectedButton = 1;
            }
            else if (bname == "button2")
            {
                selectedShape = choices[1];
                selectedButton = 2;
            }
            else
            {
                selectedShape = choices[2];
                selectedButton = 3;
            }
        }//Set selected piece when clicking a button
        public void initShapes()//Initializes the array of shapes
        {
            int r = 10000;
            shapes = new List<Shape>();
            bool[,] one = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("One Piece", convertForGame(one), Properties.Resources.one, 3, 8)); //1
            bool[,] cross = new bool[,]
            {
                { false, false, false, false, false },
                { false, false, true, false, false },
                { false, true, true, true, false },
                { false, false, true, false, false },
                { false, false, false, false, false },
            };
            shapes.Add(new Shape("Joe Piece", convertForGame(cross), Properties.Resources.Joe_Piece, 4, r));//2
            bool[,] TwoStepLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("2 Step Left", convertForGame(TwoStepLeft), Properties.Resources._2_Step_Left, 8, r));//3
            bool[,] TwoStepRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, true, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("2 Step Right", convertForGame(TwoStepRight), Properties.Resources._2_Step_Right, 8, r));//4
            bool[,] TwoxTwo = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Two x Two", convertForGame(TwoxTwo), Properties.Resources._2x2, 1, r));//5
            bool[,] ThreeStepLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, false, false},
                {false, false, true, false, false},
                {false, false, false, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("3 Step Left", convertForGame(ThreeStepLeft), Properties.Resources._3_Step_Left, 8, r));//6
            bool[,] ThreeStepRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, true, false},
                {false, false, true, false, false},
                {false, true, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("3 Step Right", convertForGame(ThreeStepRight), Properties.Resources._3_Step_Right, 8, r));//7
            bool[,] FourStepLeft = new bool[,]
            {
                {true, false, false, false, false},
                {false, true, false, false, false},
                {false, false, true, false, false},
                {false, false, false, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("4 Step Right", convertForGame(FourStepLeft), Properties.Resources._4_Step_Left, 8, r));//8
            bool[,] FourStepRight = new bool[,]
            {
                {false, false, false, false, true},
                {false, false, false, true, false},
                {false, false, true, false, false},
                {false, true, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("4 Step Right", convertForGame(FourStepRight), Properties.Resources._4_Step_Right, 8, r));//9
            bool[,] DownLeftStair = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Down Left Stair", convertForGame(DownLeftStair), Properties.Resources.Down_Left_Stair, 1, r));//10
            bool[,] DownRightStair = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Down Right Stair", convertForGame(DownRightStair), Properties.Resources.Down_Right_Stair, 1, r));//11
            bool[,] UpRightStair = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Up Right Stair", convertForGame(UpRightStair), Properties.Resources.Up_Right_Stair, 1, r));//12
            bool[,] UpLeftStair = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, true, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Up Left Stair", convertForGame(UpLeftStair), Properties.Resources.Up_Left_Stair, 1, r));//13
            bool[,] FerbDown = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Ferb Down", convertForGame(FerbDown), Properties.Resources.Ferb_Down, 1, r));//14
            bool[,] FerbLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, true, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Ferb Left", convertForGame(FerbLeft), Properties.Resources.Ferb_Left, 1, r));//15
            bool[,] FerbRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, true, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Ferb Right", convertForGame(FerbRight), Properties.Resources.Ferb_Right,1 , r));//16
            bool[,] FerbUp = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Ferb Up", convertForGame(FerbUp), Properties.Resources.Ferb_Up, 1, r));//17
            bool[,] HFive = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {true, true, true, true, true},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Five Horizontal", convertForGame(HFive), Properties.Resources.H_Five, 1, r));//18
            bool[,] HFour = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, true},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Four Horizontal", convertForGame(HFour), Properties.Resources.H_Four, 1, r));//19
            bool[,] HThree = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Four Three", convertForGame(HThree), Properties.Resources.H_Three, 1, r));//20
            bool[,] HTwo = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Four Two", convertForGame(HTwo), Properties.Resources.H_two, 1, 8));//21
            bool[,] Helmet = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, true, false, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Helmet", convertForGame(Helmet), Properties.Resources.Helmet, 8, r));//22
            bool[,] UpSideDownHelmet = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, true, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Upside Down Helmet", convertForGame(UpSideDownHelmet), Properties.Resources.UpSideDown_Helmet, 8, r));//23
            bool[,] LeftHelmet = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, true, false, false},
                {false, false, true, false, false},
                {false, true, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Left Helmet", convertForGame(LeftHelmet), Properties.Resources.Helmet_Left, 8, r));//24
            bool[,] RightHelmet = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, false, true, false, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Right Helmet", convertForGame(RightHelmet), Properties.Resources.Helmet_Right, 8, r));//24
            bool[,] InvertedLDown = new bool[,]
{
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, false, false, true, false},
                {false, false, false, false, false},
};
            shapes.Add(new Shape("Inverted L Down", convertForGame(InvertedLDown), Properties.Resources.Inverted_L_Down, 1, r));//25
            bool[,] InvertedLLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, true, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Inverted L Left", convertForGame(InvertedLLeft), Properties.Resources.Inverted_L_Left, 1, r));//26
            bool[,] InvertedLRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Inverted L Right", convertForGame(InvertedLRight), Properties.Resources.Inverted_L_Right, 1, r));//27
            bool[,] InvertedLUp = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, false, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Inverted L Up", convertForGame(InvertedLUp), Properties.Resources.Inverted_L_Up, 1, r));//28
            bool[,] LDown = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, true, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("L Down", convertForGame(LDown), Properties.Resources.L_Down, 1, r));//29
            bool[,] LLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("L Left", convertForGame(LLeft), Properties.Resources.L_Left, 1, r));//30
            bool[,] L = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("L Right", convertForGame(L), Properties.Resources.L_Right, 1, r));//31
            bool[,] LUp = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, true, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("L Up", convertForGame(LUp), Properties.Resources.L_Up, 1, r));//32
            bool[,] Quadrant1 = new bool[,]
            {
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, true, true},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Quadrant 1", convertForGame(Quadrant1), Properties.Resources.Quadrant_1, 1, r));//33
            bool[,] Quadrant2 = new bool[,]
            {
                {false, false, true, false, false},
                {false, false, true, false, false},
                {true, true, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Quadrant 2", convertForGame(Quadrant2), Properties.Resources.Quadrant_2, 1, r));//34
            bool[,] Quadrant3 = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {true, true, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
            };
            shapes.Add(new Shape("Quadrant 3", convertForGame(Quadrant3), Properties.Resources.Quandrant_3, 1, r));//35
            bool[,] Quadrant4 = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, false, false},
                {false, false, true, true, true},
                {false, false, true, false, false},
                {false, false, true, false, false},
            };
            shapes.Add(new Shape("Quadrant 4", convertForGame(Quadrant4), Properties.Resources.Quadrant_4, 1, r));//36
            bool[,] SliderDown = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, true, false},
                {false, false, true, true, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Slider Down", convertForGame(SliderDown), Properties.Resources.Slider_Down, 1, r));//37
            bool[,] SliderLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, true, false, false},
                {false, false, true, true, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Slider Left", convertForGame(SliderLeft), Properties.Resources.Slider_Left, 1, r));//38
            bool[,] SliderUp = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, false, false},
                {false, true, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Slider Up", convertForGame(SliderUp), Properties.Resources.Slider_Up, 1, r));//39
            bool[,] sliderRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, true, false},
                {false, true, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Slider Right", convertForGame(sliderRight), Properties.Resources.Slider_Right, 1, r));//40
            bool[,] T = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, true, true, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("T", convertForGame(T), Properties.Resources.T, 1, r));//41
            bool[,] TDown = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, true, true, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("T Down", convertForGame(TDown), Properties.Resources.T_Down, 1, r));//42
            bool[,] TLeft = new bool[,]
            {
                {false, false, false, false, false},
                {false, true, false, false, false},
                {false, true, true, true, false},
                {false, true, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("T Left", convertForGame(TLeft), Properties.Resources.T_Left, 1, r));//43
            bool[,] TRight = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, false, true, false},
                {false, true, true, true, false},
                {false, false, false, true, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("T Right", convertForGame(TRight), Properties.Resources.T_Right, 1, r));//44
            bool[,] VFive = new bool[,]
            {
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
            };
            shapes.Add(new Shape("Vertical Five", convertForGame(VFive), Properties.Resources.V_Five, 1, r));//45
            bool[,] VFour = new bool[,]
            {
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Vertical Four", convertForGame(VFour), Properties.Resources.V_Four, 1, r));//46
            bool[,] VThree = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Vertical Three", convertForGame(VThree), Properties.Resources.V_Three, 1, r));//47
            bool[,] VTwo = new bool[,]
            {
                {false, false, false, false, false},
                {false, false, true, false, false},
                {false, false, true, false, false},
                {false, false, false, false, false},
                {false, false, false, false, false},
            };
            shapes.Add(new Shape("Vertical Two", convertForGame(VTwo), Properties.Resources.V_Two, 1, r));//48 should be 49, I had two 24s, its all there
        }
        private void createChoices()//Assigns shapes to the three buttons
        {
            List<Shape> shapesIWant = ShapesIWant();
            //Clear the list
            choices = new List<Shape>();
            lessChoices = new List<Shape>();
            //Button 1
            int num = rand.Next(shapesIWant.Count);
            choices.Add(shapesIWant[num]);
            lessChoices.Add(shapesIWant[num]);
            button1.Enabled = true;
            button1.BackgroundImage = shapesIWant[num].getBitmap();
            button1.BackgroundImageLayout = ImageLayout.Stretch;
            //Button 2
            num = rand.Next(shapesIWant.Count);
            choices.Add(shapesIWant[num]);
            lessChoices.Add(shapesIWant[num]);
            button2.Enabled = true;
            button2.BackgroundImage = shapesIWant[num].getBitmap();
            button2.BackgroundImageLayout = ImageLayout.Stretch;
            //Button 3
            num = rand.Next(shapesIWant.Count);
            choices.Add(shapesIWant[num]);
            lessChoices.Add(shapesIWant[num]);
            button3.Enabled = true;
            button3.BackgroundImage = shapesIWant[num].getBitmap();
            button3.BackgroundImageLayout = ImageLayout.Stretch;

            if (!canPlaceAnywhere(choices[0]) && !canPlaceAnywhere(choices[1]) && !canPlaceAnywhere(choices[2]))
            {
                createChoicesAttempts++;
                if (createChoicesAttempts >= 2)
                {
                    choices[0] = shapes[0];
                    lessChoices[0] = shapes[0];
                    button1.BackgroundImage = shapes[0].getBitmap();
                    button1.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    createChoices();
                }
            }
            createChoicesAttempts = 0;
        }
        private void changeBoard(Shape s, int x, int y)//Just controls the light brown draw and drop feature
        {
            int[] changes = new int[9];
            for(int i = 0; i < s.getShape().GetLength(0); i++)
            {
                for (int j = 0; j < s.getShape().GetLength(1); j++)
                {
                    if (s.getShape()[i,j] == true)
                    {
                        if (x + i - 2 < 0 || x + i - 2 >= boardDisplay.GetLength(0) || y + j - 2 < 0 || y + j - 2 >= boardDisplay.GetLength(1))
                        {
                            clearView();
                            canPlace = false;
                            return;
                        }
                        else if (boardDisplay[x + i - 2, y + j - 2].BackColor == Color.SaddleBrown)
                        {
                            clearView();
                            canPlace = false;
                            return;
                        }
                        else
                        {
                            canPlace = true;
                            if (boardDisplay[x + i - 2, y + j - 2].BackColor != Color.SaddleBrown)
                            {
                                boardDisplay[x + i - 2, y + j - 2].BackColor = Color.SandyBrown;
                            }

                        }
                    }
                }
            }
        }
        private void updateBoardDisplay()
        {
            for(int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i,j] == true)
                    {
                        boardDisplay[i, j].BackColor = Color.SaddleBrown;
                    }
                    if (board[i, j] == false)
                    {
                        boardDisplay[i, j].BackColor = Color.White;
                    }
                }
            }
        }
        private void clearView()
        {
            for (int i = 0; i < boardDisplay.GetLength(0); i++)
            {
                for (int j = 0; j < boardDisplay.GetLength(1); j++)
                {
                    if (boardDisplay[i, j].BackColor == Color.SandyBrown)
                    {
                        boardDisplay[i, j].BackColor = Color.White;
                    }
                }
            }
        }
        private void updateChoices()//Updates the buttons
        {
            lessChoices.Remove(selectedShape);
            selectedShape = new Shape();
            if (selectedButton == 1)
            {
                button1.BackgroundImage = new Bitmap(1, 1);
                button1.Enabled = false;
            }
            else if (selectedButton == 2)
            {
                button2.BackgroundImage = new Bitmap(1, 1);
                button2.Enabled = false;
            }
            else if (selectedButton == 3)
            {
                button3.BackgroundImage = new Bitmap(1, 1);
                button3.Enabled = false;
            }
        }
        private void clearIfNeeded(GameState g)//clears the board when necessary and updates the display
        {
            int numCleared = 0;
            List<int> rows = checkRows();
            List<int> column = checkColumns();
            List<int> squares = checkSquares();
            numCleared += clearRow(rows);
            numCleared += clearColumn(column);
            numCleared += clearSquare(squares);
            g.setScore(g.getScore() + numCleared*18);
            if (numCleared >= 1)
            {
                streak++;
                if(streak > 1)
                {
                    g.setScore(g.getScore() + (streak - 1) * 10);
                }
            }
            else
            {
                streak = 0;
            }
            if (numCleared > 1)
            {
                g.setScore(g.getScore() + (numCleared-1) * 10); 
            }
            if (g.getBool() == true)
            {
                updateBoardDisplay();
            }
        }
        private List<int> checkRows()//actually checks columns since the set selected location gives [col, row] values :/
        {
            List<int> rows = new List<int>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                bool[] array = new bool[board.GetLength(1)];
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    array[j] = board[i, j];
                }
                if (allEqualTrue(array))
                {
                    rows.Add(i);
                }
            }
            return rows;
        }
        private List<int> checkColumns()//actually checks rows
        {
            List<int> rows = new List<int>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                bool[] array = new bool[board.GetLength(1)];
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    array[j] = board[j, i];
                }
                if (allEqualTrue(array))
                {
                    rows.Add(i);
                }
            }
            return rows;
        }
        private List<int> checkSquares()//just loops through and checks all sqaures
        {
            List<int> list = new List<int>();
            for (int n = 0; n < board.GetLength(0); n++)
            {
                if (checkSquare(n) == true)
                {
                    list.Add(n);
                }
            }
            return list;
        }
        private int clearRow(List<int> num)//clears a row of the board and updates Display
        {
            for (int n = 0; n < num.Count; n++)
            {
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    board[num[n], i] = false;
                }
            }
            return num.Count;
        }
        private int clearColumn(List<int> num)//clears a column of the board and updates the Display
        {
            for (int n = 0; n < num.Count; n++)
            {
                for (int i = 0; i < board.GetLength(1); i++)
                {
                    board[i, num[n]] = false;
                }
            }
            return num.Count;
        }
        private int clearSquare(List<int> num)//clears a sqaure of the board and updates the Display
        {
            foreach (int value in num)
            {
                int x = value % 3;
                int y = value / 3;
                int n = 0;
                for (int i = x * 3; i < (x + 1) * 3; i++)
                {
                    for (int j = y * 3; j < (y + 1) * 3; j++)
                    {
                        board[i, j] = false;
                        n++;
                    }
                }
            }
            return num.Count;
        }
        private bool allEqualTrue(bool[] array)//sees if an array of bools all equal true.
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == false)
                {
                    return false;
                }
            }
            return true;
        }
        private bool checkSquare(int num)//checks if one sqaure has all true values
        {
            int x = num % 3;
            int y = num / 3;
            bool[] array = new bool[board.GetLength(0)];
            int n = 0;
            for (int i = x * 3; i < (x+1)*3; i++)
            {
                for (int j = y * 3; j < (y+1) * 3; j++)
                {
                    array[n] = board[i, j];
                    n++;
                }
            }
            if (allEqualTrue(array))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool isGameOver(List<Shape> c)//Seems to be working, if any glitches occur, its because of lessChoices list and how removing shapes works
        {
            foreach (Shape s in c)
            {
                if (canPlaceAnywhere(s) == true)
                {
                    return false;
                }
            }
            return true;       
        }
        private bool canPlaceAtLocation(Shape s, int x, int y)
        {
            for (int i = 0; i < s.getShape().GetLength(0); i++)
            {
                for (int j = 0; j < s.getShape().GetLength(1); j++)
                {
                    if (s.getShape()[i, j] == true)
                    {
                        if (x + i - 2 < 0 || x + i - 2 >= boardDisplay.GetLength(0) || y + j - 2 < 0 || y + j - 2 >= boardDisplay.GetLength(1))
                        {
                            return false;
                        }
                        else if (boardDisplay[x + i - 2, y + j - 2].BackColor == Color.SaddleBrown)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private int scoreValue(Shape s)
        {
            int value = 0;
            for (int i = 0; i < s.getShape().GetLength(0); i++)
            {
                for (int j = 0; j < s.getShape().GetLength(1); j++)
                {
                    if (s.getShape()[i, j] == true)
                    {
                        value++;
                    }
                }
            }
            return value;
        }
        private bool[,] convertForGame(bool[,] og)//changes the row and column values of the shape array since the game reads them backwards (which i did on accident)
        {
            bool[,] temp = new bool[og.GetLength(0), og.GetLength(1)];
            for (int i = 0; i < og.GetLength(0); i++)
            {
                for (int j = 0; j < og.GetLength(1); j++)
                {
                    temp[i, j] = og[j, i];
                }
            }
            return temp;
        }
        private bool canPlaceAnywhere(Shape s)//checks if a shape can be placed anywhere
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (canPlaceAtLocation(s, i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private List<Shape> ShapesIWant()//gives a list of only the possible shapes i want based on the round
        {
            List<Shape> temp = new List<Shape>();
            foreach (Shape s in shapes)
            {
                if (s.getEarliestRound() <= round && s.getLatestRound() >= round)
                {
                    temp.Add(s);
                }
            }
            return temp;
        }
        private int calculateSpaceScore()//calculates spacescore of the board
        {
            int s = 0;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    int temp = getSpaceScoreForLocation(i, j);
                    s += temp;
                }
            }
            return s;
        }
        private int getSpaceScoreForLocation(int x, int y)//gives spacescore for one location
        {
            int temp = 0;
            if (board[x, y])
            {
                return 0;
            }
            else
            {
                temp += isSpaceAround(x, y);
            }
            return temp;
        }
        private int isSpaceAround(int x, int y)//Checks if there is space around a spot for space score calculations
        {
            int clearRings = 0;
            for (int rings = 1; rings <= 5; rings++)
            {
                if (!(x-rings < 0 || y-rings < 0 || x + rings >= board.GetLength(0) || y + rings >= board.GetLength(1)))
                {
                    for (int i = x - rings; i <= x + rings; i++)
                    {
                        for (int j = y - rings; j <= y + rings; j++)
                        {
                            if (board[i, j])
                            {
                                return clearRings + 1;
                            }
                        }
                    }
                    clearRings++;
                }
                else
                {
                    return clearRings + 1;
                }
            }
            return clearRings + 1;
        }
        private void placeShape(int Lo_i, int Lo_j, GameState g)
        {
            if (canPlace == false || selectedShape.getName() == "")
            {
                return;
            }
            //Updates the board array with Trues where the blocks are placed
            for (int i = 0; i < selectedShape.getShape().GetLength(0); i++)
            {
                for (int j = 0; j < selectedShape.getShape().GetLength(1); j++)
                {
                    if (selectedShape.getShape()[i, j] == true)
                    {
                        if (Lo_i + i - 2 < 0 || Lo_i + i - 2 >= g.getBoard().GetLength(0) || Lo_j + j - 2 < 0 || Lo_j + j - 2 >= boardDisplay.GetLength(1))
                        {
                            return;
                        }
                        else
                        {
                            g.getBoard()[Lo_i + i - 2, Lo_j + j - 2] = true;
                        }
                    }
                }
            }
            //Updates the board display with the real block placements
            clearIfNeeded(g);
            g.setScore(g.getScore() + scoreValue(selectedShape));//done before update choices because update choices clears selected shape
            g.setSpaceScore(calculateSpaceScore());
            updateChoices();
            if (button1.Enabled == false && button2.Enabled == false && button3.Enabled == false)
            {
                round++;
                lblRound.Text = "Round: " + round;
                createChoices();
            }
            g.setActiveButtons(activeButtons());
            if (g.getBool() == true)
            {
                lblSpaceScore.Text = "Space Score: " + game.getSpaceScore();
                lblScore.Text = "Score: " + g.getScore();
                if (isGameOver(lessChoices))
                {
                    gameOver = new Button();
                    gameOver.Size = new System.Drawing.Size(200, 100);
                    gameOver.Text = "Restart";
                    gameOver.Click += new EventHandler(gameOverButton);
                    gameOver.Location = new System.Drawing.Point(Size.Width / 2 - gameOver.Size.Width / 2, Size.Height / 2 - gameOver.Size.Height / 2 - 100);
                    Controls.Add(gameOver);
                    Controls.SetChildIndex(gameOver, 1);
                }
            }
        }
        private List<int> activeButtons()
        {
            List<int> temp = new List<int>();
            foreach (Button b in buttons)
            {
                if (b.Enabled == true)
                {
                    if(b.Name == "button1")
                    {
                        if (canPlaceAnywhere(choices[0]))
                        {
                            temp.Add(0);
                        }
                    }
                    else if (b.Name == "button2")
                    {
                        if (canPlaceAnywhere(choices[1]))
                        {
                            temp.Add(1);
                        }
                    }
                    else if (b.Name == "button3")
                    {
                        if (canPlaceAnywhere(choices[2]))
                        {
                            temp.Add(2);
                        }
                    }
                }
            }
            return temp;
        }
        private void clearEventHandlers()
        {
            button1.Click -= new EventHandler(setSelectedPiece_OnPress);
            button2.Click -= new EventHandler(setSelectedPiece_OnPress);
            button3.Click -= new EventHandler(setSelectedPiece_OnPress);
        }
    }
}
