using System;
using System.Drawing;
using System.Windows.Forms;

namespace Checkers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public Checkers checkers;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = this.Size;
            this.MaximumSize = this.Size;

            Color hoverColor = Color.FromArgb(20, 0, 0, 0);

            button1.FlatAppearance.MouseOverBackColor = hoverColor;
            button2.FlatAppearance.MouseOverBackColor = hoverColor;
            button3.FlatAppearance.MouseOverBackColor = hoverColor;

            button1.Visible = false;
            button1.Visible = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(checkers != null)
            {
                checkers.Render(e, pictureBox1);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //int relativeX = MousePosition.X - pictureBox1.Left;
            //int relativeY = MousePosition.Y - pictureBox1.Top;

            var relativePos = pictureBox1.PointToClient(new Point(MousePosition.X, MousePosition.Y));

            int relativeX = relativePos.X;
            int relativeY = relativePos.Y;

            bool IsBlack = true;

            for (int X = 0; X < 8; X++)
            {
                for (int Y = 0; Y < 8; Y++)
                {
                    Rectangle square = checkers.SquarePositions[X, Y];

                    // Only select a spot that has one of current turn's peices
                    CheckersPiece piece;
                    if (square.IntersectsWith(new Rectangle(relativeX, relativeY, 1, 1)))
                    {
                        bool hasPiece = checkers.TryGetPiece(X, Y, out piece);
                        if (checkers.SelectedSquare.HasValue && (!hasPiece || piece.IsCaptured) && IsBlack)
                        {
                            if(checkers.MovePiece(checkers.SelectedSquare.Value.X, checkers.SelectedSquare.Value.Y, X, Y))
                            {
                                checkers.IsBlackTurn = !checkers.IsBlackTurn;
                            }
                        }
                        else if (hasPiece && !piece.IsCaptured && piece.IsBlack == checkers.IsBlackTurn)
                        {
                            checkers.SelectedSquare = new Vector2() { X = X, Y = Y };
                            return;
                        }
                    }
                    IsBlack = !IsBlack;
                }
                IsBlack = !IsBlack;
            }
            checkers.SelectedSquare = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkers = null;
            checkers = new Checkers();
            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(0, 0);
            this.MaximumSize = new Size(0, 0);

            button3.Visible = false;

            button1.Visible = true;
            button1.Visible = true;

            button1_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }


    public class Checkers
    {
        public bool IsBlackTurn { get; set; } = true;

        public Rectangle[,] SquarePositions = new Rectangle[8,8];
        public Vector2? SelectedSquare = null;

        public bool MovePiece(int X, int Y, int newX, int newY)
        {
            CheckersPiece piece;
            bool movablePiece = TryGetPiece(X, Y, out piece);

            int xDist = X - newX;
            int yDist = Y - newY;

            bool makeAKing = false;

            if(movablePiece && !piece.IsKing)
            {
                // Make sure you cannot move backwards
                if (piece.IsBlack && newY < Y)
                {
                    return false;
                }
                else if(!piece.IsBlack && newY > Y)
                {
                    return false;
                }

                // Check if you are now a king
                if(newY == 0 || newY == 7)
                {
                    makeAKing = true;
                }
            }

            if (X == newX || Y == newY)
            {
                return false;
            }
            
            if(xDist == 2 || xDist == -2)
            {
                CheckersPiece jumpedPiece;

                int minusX = (xDist < 0) ? -1 : 1;
                int minusY = (yDist < 0) ? -1 : 1;

                bool jumpedAPiece = TryGetPiece(X - (xDist - minusX), Y - (yDist - minusY), out jumpedPiece);
                if (!jumpedAPiece || jumpedPiece.IsCaptured)
                {
                    return false;
                }
                else if(jumpedPiece != null && jumpedPiece.IsBlack != piece.IsBlack)
                {
                    jumpedPiece.IsCaptured = true;
                }
                else
                {
                    return false;
                }
            }

            if(movablePiece)
            {
                piece.Position.X = newX;
                piece.Position.Y = newY;

                if (makeAKing)
                {
                    piece.IsKing = true;
                }

                return true;
            }
            return false;
        }

        public bool TryGetPiece(int X, int Y, out CheckersPiece piece)
        {
            foreach (var p in Black)
            {
                if (p.Position.X == X && p.Position.Y == Y)
                {
                    piece = p;
                    return true;
                }
            }
            foreach (var p in White)
            {
                if (p.Position.X == X && p.Position.Y == Y)
                {
                    piece = p;
                    return true;
                }
            }
            piece = null;
            return false;
        }

        public CheckersPiece[] Black = new CheckersPiece[12]
        {
            new CheckersPiece(0, 0, true),
            new CheckersPiece(2, 0, true),
            new CheckersPiece(4, 0, true),
            new CheckersPiece(6, 0, true),

            new CheckersPiece(1, 1, true),
            new CheckersPiece(3, 1, true),
            new CheckersPiece(5, 1, true),
            new CheckersPiece(7, 1, true),

            new CheckersPiece(0, 2, true),
            new CheckersPiece(2, 2, true),
            new CheckersPiece(4, 2, true),
            new CheckersPiece(6, 2, true)
        };

        public CheckersPiece[] White = 
        {

            new CheckersPiece(1, 5, false),
            new CheckersPiece(3, 5, false),
            new CheckersPiece(5, 5, false),
            new CheckersPiece(7, 5, false),

            new CheckersPiece(0, 6, false),
            new CheckersPiece(2, 6, false),
            new CheckersPiece(4, 6, false),
            new CheckersPiece(6, 6, false),

            new CheckersPiece(1, 7, false),
            new CheckersPiece(3, 7, false),
            new CheckersPiece(5, 7, false),
            new CheckersPiece(7, 7, false)
        };

        public void Render(PaintEventArgs e, PictureBox control)
        {
            // --- Board

            int Spacing = 50;

            int Board_Y = Spacing;
            int Board_W = e.ClipRectangle.Width - Board_Y - Spacing;
            int Board_H = e.ClipRectangle.Height - Board_Y - Spacing;
            while(Board_W != Board_H)
            {
                if(Board_W > Board_H) { Board_W--; }
                if (Board_W < Board_H) { Board_H--; }
            }
            int Board_X = (e.ClipRectangle.Width / 2) - (Board_W / 2) + 50;

            // Background
            e.Graphics.DrawRectangle(Pens.White, Board_X, Board_Y, Board_W, Board_H);

            // Squares
            int SpaceApart = (Board_W / 8) / 10;
            int Square_Size = (Board_W / 8) - SpaceApart;

            bool IsBlack = true;

            for(int X = 0; X < 8; X++)
            {
                for (int Y = 0; Y < 8; Y++)
                {
                    int squareX = Board_X + SpaceApart + (X * (Square_Size + SpaceApart));
                    int squareY = Board_Y + SpaceApart + (Y * (Square_Size + SpaceApart));

                    // Save Square Indfo
                    SquarePositions[X, Y].X = squareX;
                    SquarePositions[X, Y].Y = squareY;
                    SquarePositions[X, Y].Width = Square_Size;
                    SquarePositions[X, Y].Height = Square_Size;

                    if(SelectedSquare.HasValue && SelectedSquare.Value.X == X && SelectedSquare.Value.Y == Y)
                    {
                        e.Graphics.FillRectangle(Brushes.Cyan, squareX, squareY, Square_Size, Square_Size);
                    }
                    else if (!IsBlack)
                    {
                        e.Graphics.FillRectangle(Brushes.SeaShell, squareX, squareY, Square_Size, Square_Size);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(Brushes.SaddleBrown, squareX, squareY, Square_Size, Square_Size);
                    }


                    foreach(var piece in Black)
                    {
                        if (piece.IsCaptured)
                        {
                            continue;
                        }
                        else if(piece.Position.X == X && piece.Position.Y == Y)
                        {
                            e.Graphics.FillEllipse(Brushes.Black, squareX, squareY, Square_Size, Square_Size);

                            if (piece.IsKing) 
                            {
                                Pen gold = (Pen)Pens.Gold.Clone();
                                gold.Width += 2;
                                e.Graphics.DrawEllipse(gold, squareX, squareY, Square_Size, Square_Size);
                            }
                        }
                    }
                    foreach (var piece in White)
                    {
                        if (piece.IsCaptured)
                        {
                            continue;
                        }
                        else if (piece.Position.X == X && piece.Position.Y == Y)
                        {
                            e.Graphics.FillEllipse(Brushes.Tan, squareX, squareY, Square_Size, Square_Size);

                            if (piece.IsKing)
                            {
                                Pen gold = (Pen)Pens.Gold.Clone();
                                gold.Width += 2;
                                e.Graphics.DrawEllipse(gold, squareX, squareY, Square_Size, Square_Size);
                            }
                        }
                    }
                    IsBlack = !IsBlack;
                }
                IsBlack = !IsBlack;
            }
            control.Invalidate();
        }

        public Checkers()
        {

        }
    }


    public class CheckersPiece
    {
        public bool IsKing { get; set; }
        public bool IsCaptured { get; set; }
        public bool IsBlack { get; set; }

        public Vector2 Position;


        public CheckersPiece(int X, int Y, bool black)
        {
            IsKing = false;
            IsCaptured = false;
            Position = new Vector2() { X = X, Y = Y };
            IsBlack = black;
        }
    }

    public struct Vector2
    {
        public int X, Y;
    }
}