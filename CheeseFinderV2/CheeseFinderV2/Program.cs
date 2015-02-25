using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheeseFinderV2
{
    class Program
    {
        static void Main(string[] args)
        {
            CheeseNibbler test = new CheeseNibbler();
            test.PlayGame();

            Console.ReadKey();
        }
    }

    /// <summary>
    /// Class Point which has x and y coordinates and its status
    /// </summary>
    public class Point
    {
        //enums for different point statuses
        public enum PointStatus
        {
            Empty, Mouse, Cheese, Cat, CatAndCheese
        }
        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }
        //Constructor that takes in x and y coordinates and sets status to Empty by default
        public Point(int x, int y)
        {
            this.X = x; this.Y = y;
            this.Status = PointStatus.Empty;
        }
    }

    /// <summary>
    /// Class for Mouse
    /// </summary>
    public class Mouse
    {
        //Property of type Point that holds x and y coordinates and status
        public Point Position { get; set; }
        public int Energy { get; set; }
        public bool HasBeenPouncedOn { get; set; }
        //Constructor that initializes statring energy amount and sets boolean HasBeenPouncedOn to false 
        public Mouse()
        {
            this.Energy = 50;
            this.HasBeenPouncedOn = false;
        }
    }

    /// <summary>
    /// Class for Cat that hold cat's coordinates and status
    /// </summary>
    public class Cat
    {
        public Point Position { get; set; }
        public Cat() { }
    }

    public class CheeseNibbler
    {
        //Grid property of 2D array type with Points elements in it
        public Point[,] Grid { get; set; }
        //property for holding a mouse
        public Mouse Mouse { get; set; }
        //property for holding a cheese
        public Point Cheese { get; set; }
        //property that keeps count of how many cheeses were eaten
        public int CheeseCount { get; set; }
        //counter for number of rounds
        public int Round { get; set; }
        //list property to store cats
        public List<Cat> Cats { get; set; }
        public Random rng { get; set; }

        /// <summary>
        /// Constructor that initializes the grid, sets mouse and cheese points and initializing a list of cats
        /// </summary>
        public CheeseNibbler()
        {
            this.rng = new Random();
            this.Round = 1;
            this.Grid = new Point[10, 10];
            //setting up a Grid
            for (int y = 0; y < this.Grid.GetLength(1); y++)
            {
                for (int x = 0; x < this.Grid.GetLength(0); x++)
                {
                    this.Grid[x, y] = new Point(x, y);
                }
            }
            //initializing and placing a Mouse
            this.Mouse = new Mouse();
            this.Mouse.Position = this.Grid[this.rng.Next(0, 10), this.rng.Next(0, 10)];
            this.Mouse.Position.Status = Point.PointStatus.Mouse;
            //placing a cheese
            PlaceCheese();
            //initializing a list of cats
            this.Cats = new List<Cat>();
        }
        /// <summary>
        /// Method that draws a grid on the console
        /// </summary>
        public void DrawGrid()
        {
            Console.Clear();
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    switch (this.Grid[x, y].Status)
                    {
                        case Point.PointStatus.Empty:
                            Console.Write("[   ] ");
                            break;
                        case Point.PointStatus.Mouse:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("[ M ] ");
                            Console.ResetColor();
                            break;
                        case Point.PointStatus.Cheese:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("[ C ] ");
                            Console.ResetColor();
                            break;
                        case Point.PointStatus.CatAndCheese:
                        case Point.PointStatus.Cat:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("[ X ] ");
                            Console.ResetColor();
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Your energy: {0}",this.Mouse.Energy);
        }

        /// <summary>
        /// Method that processes a user input
        /// </summary>
        /// <returns>Returns a key pressed by user</returns>
        public ConsoleKey GetUserMove()
        {
            //by putting true in the ReadKey, you doesn't show the character its self
            ConsoleKeyInfo userInput = Console.ReadKey(true);
            //checking if only arrow key was pressed and if it's a valid move
            while (userInput.Key != ConsoleKey.LeftArrow && userInput.Key != ConsoleKey.RightArrow && userInput.Key != ConsoleKey.UpArrow && userInput.Key != ConsoleKey.DownArrow || !ValidMove(userInput.Key))
            {
                Console.WriteLine("Invalid input");
                userInput = Console.ReadKey(true);
            }
            return userInput.Key;
        }

        /// <summary>
        /// Method that checks if user move was valid (within the grid)
        /// </summary>
        /// <param name="input">User input</param>
        /// <returns>Returns true if move was valid and false if not</returns>
        public bool ValidMove(ConsoleKey input)
        {
            if ((input == ConsoleKey.LeftArrow && this.Mouse.Position.X == 0) || (input == ConsoleKey.RightArrow && this.Mouse.Position.X == 9) || (input == ConsoleKey.UpArrow && this.Mouse.Position.Y == 0) || (input == ConsoleKey.DownArrow && this.Mouse.Position.Y == 9))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method that performs a mouse move
        /// </summary>
        /// <param name="input">Key pressed by user</param>
        public void MoveMouse(ConsoleKey input)
        {
            //storing initial mouse cordinates
            int newMouseX = this.Mouse.Position.X;
            int newMouseY = this.Mouse.Position.Y;
            //checking for key pressed and increasing/decreasing respective coordinate
            switch (input)
            {
                case ConsoleKey.LeftArrow:
                    newMouseX--;
                    break;
                case ConsoleKey.RightArrow:
                    newMouseX++;
                    break;
                case ConsoleKey.UpArrow:
                    newMouseY--;
                    break;
                default:
                    newMouseY++;
                    break;
            }
            //declaring a new (targeted) point where mouse should be moved to
            Point targetPosition = this.Grid[newMouseX, newMouseY];
            //checking if targeted point is a cheese
            if (targetPosition.Status == Point.PointStatus.Cheese)
            {
                //increasing cheese counter and placing a new cheese
                this.CheeseCount++;
                PlaceCheese();
                //moving mouse to new (targeted) point and changing the old point to empty
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                targetPosition.Status = Point.PointStatus.Mouse;
                this.Mouse.Position = targetPosition;
                //increasing mouse energy
                this.Mouse.Energy +=10;
                //adding a cat on the grid for every two cheeses consumed
                if (this.CheeseCount % 2 == 0)
                {
                    AddCat();
                }
            }
            //this part executes if targeted point wasn't a cheese
            else
            {
                //moving mouse to new (targeted) point, changing the old point to empty and decreasing mouse energy by 1
                this.Mouse.Position.Status = Point.PointStatus.Empty;
                targetPosition.Status = Point.PointStatus.Mouse;
                this.Mouse.Position = targetPosition;
                this.Mouse.Energy--;
            }
        }
        /// <summary>
        /// This method runs a game
        /// </summary>
        public void PlayGame()
        {
            //boolean for running a game loop
            bool playing = false;
            //main game loop
            while (!playing)
            {
                //drawing a grid
                this.DrawGrid();
                //processing mouse move by respective user input
                this.MoveMouse(this.GetUserMove());
                //increasing round counter
                this.Round++;
                //processing a move for each cat in the cats list
                foreach (Cat cat in Cats)
                {
                    MoveCat(cat);
                }
                //checking if a mouse has any energy left
                if (this.Mouse.Energy < 0)
                {
                    //if it doensn't then stops the game
                   playing = true;
                }
                //checking if mouse was caught by a cat
                playing = this.Mouse.HasBeenPouncedOn;
            }
            //printing out a message depending of a scenario
            if (this.Mouse.Energy < 0)
            {
                Console.WriteLine("You have run out of energy.");
            }
            else
            {
                Console.WriteLine("You got caught after {0} moves!", this.Round);
            }
        }

        /// <summary>
        /// Method that places a cheese on a grid
        /// </summary>
        public void PlaceCheese()
        {
            do
            {
                this.Cheese = this.Grid[this.rng.Next(0, 10), this.rng.Next(0, 10)];
            } while (this.Cheese.Status != Point.PointStatus.Empty);
            this.Cheese.Status = Point.PointStatus.Cheese;
        }

        /// <summary>
        /// Method which creates a new cat, places it in on the grid and adds to the cats list
        /// </summary>
        public void AddCat()
        {
            Cat cat = new Cat();
            PlaceCat(cat);
            this.Cats.Add(cat);
        }

        /// <summary>
        /// Method that places a cat on the grid
        /// </summary>
        /// <param name="cat">Cat to be placed</param>
        public void PlaceCat(Cat cat)
        {
            do
            {
                cat.Position = this.Grid[this.rng.Next(0, 10), this.rng.Next(0, 10)];
            } while (cat.Position.Status != Point.PointStatus.Empty);
            cat.Position.Status = Point.PointStatus.Cat;
        }

        /// <summary>
        /// Method that performs a cat's move
        /// </summary>
        /// <param name="cat">Cat to be moved</param>
        public void MoveCat(Cat cat)
        {
            //Cat has an 80% chance to move
            int moveChance = this.rng.Next(1, 11);
            //checking for move chance
            if (moveChance <= 8)
            {
                //calculating a difference between cat and mouse coordinates
                int diffX = this.Mouse.Position.X - cat.Position.X;
                int diffY = this.Mouse.Position.Y - cat.Position.Y;
                //declaring booleans that tell which direction a mouse is from cat's position
                bool tryLeft = diffX < 0;
                bool tryRight = diffX > 0;
                bool tryUp = diffY < 0;
                bool tryDown = diffY > 0;
                //declaring a target point for next cat move
                Point targetPosition = cat.Position;
                //creating a boolean for checking if possible move is valid
                bool validMove = false;
                //loop that checks if possible moves are valid
                while (!validMove && (tryLeft || tryRight || tryUp || tryDown))
                {
                    //storing original cat coordinates
                    int startingCatX = cat.Position.X;
                    int startingCatY = cat.Position.Y;
                    //checking for possible moves, increasing/decreasing respective coordinate and changing possible move boolean to false since we tried that move
                    if (tryLeft)
                    {
                        tryLeft = false;
                        startingCatX--;
                    }
                    else if (tryRight)
                    {
                        tryRight = false;
                        startingCatX++;
                    }
                    else if (tryUp)
                    {
                        tryUp = false;
                        startingCatY--;
                    }
                    else if (tryDown)
                    {
                        tryDown = false;
                        startingCatY++;
                    }
                    //checking if targeted position is within a grid range
                    if ((startingCatX >= 0 && startingCatX <=9) && (startingCatY >= 0 && startingCatY <=9))
                    {
                        //passing a targeted point from a grid to targeted point variable
                        targetPosition = this.Grid[startingCatX, startingCatY];
                        //checking if targeted point is a valid point for cat move
                        //if the point is valid that loop breaks
                        validMove = IsValidCatMove(targetPosition);
                    }
                }
                //below block executes if we found a valid point to move
                if (validMove)
                {
                    //checking if previous cat position was cat and cheese
                    if (cat.Position.Status == Point.PointStatus.CatAndCheese)
                    {
                        //if it was, then switching in to cheese
                        cat.Position.Status = Point.PointStatus.Cheese;
                    }
                    else
                    {
                        //if it wasn't a cheese, then changing it to empty
                        cat.Position.Status = Point.PointStatus.Empty;
                    }
                    //checking for status of target point for cat move
                    //checking if it is a mouse
                    if (targetPosition.Status == Point.PointStatus.Mouse)
                    {
                        //if it's a mouse then stops the game and moves cat there
                        this.Mouse.HasBeenPouncedOn = true;
                        targetPosition.Status = Point.PointStatus.Cat;
                    }
                    //checking if it's a cheese
                    else if (targetPosition.Status == Point.PointStatus.Cheese)
                    {
                        //changing target point to cat and cheese status
                        targetPosition.Status = Point.PointStatus.CatAndCheese;
                    }
                    //if it wasn't a mouse or cheese, then it is an empty point
                    else
                    {
                        //changing target point to cat status
                        targetPosition.Status = Point.PointStatus.Cat;
                    }
                    //passing a target position to cat position
                    cat.Position = targetPosition;
                }
            }
        }
        /// <summary>
        /// Method that checks if a target point is valid for cat move
        /// </summary>
        /// <param name="targetPosition">Point to be checked</param>
        /// <returns>Returns true if point is valid for move and false if it's not</returns>
        public bool IsValidCatMove(Point targetPosition)
        {
            if (targetPosition.Status == Point.PointStatus.Empty || targetPosition.Status == Point.PointStatus.Mouse || targetPosition.Status == Point.PointStatus.Cheese)
            {
                return true;
            }
            return false;
        }
    }
}
