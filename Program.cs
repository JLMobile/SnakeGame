using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Media;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Data;

namespace Snake
{
    //Define a struct for the position of the snake and other objects
    struct Position
    {
        public int row;
        public int col;
        public Position(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    class Snake
    {
        private static int MainMenu()
        {
            //main menu options
            Console.Clear();
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Choose an option:");
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "1) Start Game");
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "2) High Scores");
            Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "3) Exit");
            Console.Write("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Select an option: ");

            switch (Console.ReadLine())
            {
                //level selector
                case "1":
                    Console.Clear();
                    Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "1)Normal Mode");
                    Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "2)Hard Mode");
                    Console.Write("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Select an option: ");
                    switch (Console.ReadLine())
                    {
                        case "1":
                            return 1;
                        case "2":
                            return 2;
                        default:
                            return 1;
                    }
                    //show high scores
                case "2":
                    string path = @"C:\Users\Public\Documents\userPoints.txt";
                    string readText = File.ReadAllText(path);
                    Console.WriteLine(readText);
                    Console.ReadKey();
                    return 1;
                    //exit the game
                case "3":
                    Environment.Exit(0);
                    return 1;
                default:
                    return 1;
            }
        }
        public void BgMusic()
        {
            //Create SoundPlayer objbect to control background music playback in the game
            SoundPlayer bgMusic = new SoundPlayer();
            //Locating the soundtrack in the directory
            bgMusic.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + @"\matter.wav";
            //Will loop the background music if it finishes
            bgMusic.PlayLooping();
        }
        //Method to draw the food
        public void DrawFood()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("@");
        }
        //Method to draw the obstacle
        public void DrawObstacle()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("=");
        }
        //Method to draw the snake body
        public void DrawSnakeBody()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("*");
        }
        //Method for the directional positions
        public void Direction(Position[] directions)
        {
            directions[0] = new Position(0, 1);
            directions[1] = new Position(0, -1);
            directions[2] = new Position(1, 0);
            directions[3] = new Position(-1, 0);
        }
        //Method to create obstacl and initialise certain random position of obstacles at every game play
        public void Obstacles(List<Position> obstacles)
        {
            Random rand = new Random();
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
        }
        public void CheckUserInput(ref int direction, byte right, byte left, byte down, byte up)
        {
            //User key pressed statement: depends on which direction the user want to go to get food or avoid obstacle
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo userInput = Console.ReadKey();
                if (userInput.Key == ConsoleKey.LeftArrow)
                {
                    if (direction != right) direction = left;
                }
                if (userInput.Key == ConsoleKey.RightArrow)
                {
                    if (direction != left) direction = right;
                }
                if (userInput.Key == ConsoleKey.UpArrow)
                {
                    if (direction != down) direction = up;
                }
                if (userInput.Key == ConsoleKey.DownArrow)
                {
                    if (direction != up) direction = down;
                }
            }
        }
        //Overloaded method for the GameOver part of the game.
        public int GameOver(Queue<Position> snakeElements, Position snakeNewHead, int negativePoints, List<Position> obstacles)
        {
            if (snakeElements.Contains(snakeNewHead) || obstacles.Contains(snakeNewHead))
            {
                Console.SetCursorPosition(0, 0);
                Console.ForegroundColor = ConsoleColor.Red;//Text colour for the game over screen                                     
                Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Game Over!");
                int userPoints = (snakeElements.Count - 4) * 100 - negativePoints;    //points calculated for player
                userPoints = Math.Max(userPoints, 0);
                Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Your points are: {0}", userPoints);
                Console.Write("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Please enter your name: ");
                string user_name = Console.ReadLine();
                SaveFile(userPoints, user_name);//keeps the points and username is SaveFile to be written in to file
                //Exits the game when "Enter Key" is pressed
                Console.WriteLine("\t" + "\t" + "\t" + "\t" + "\t" + "\t" + "Press Enter to exit the game!");
                while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                return 1;
            }
            return 0;
        }
        //Method to create file and write to file the username and points.
        public void SaveFile(int userPoints, string user_name)
        {
            String filePath = Path.Combine(@"C:\Users\Public\Documents\userPoints.txt");
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                    File.WriteAllText(filePath, user_name + "  " + userPoints.ToString() + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText(filePath, user_name + "  " + userPoints.ToString() + Environment.NewLine);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("{0} Exception caught.", exception);
            }
        }
   //Main Program
        static void Main(string[] args)
        {
            byte right = 0;
            byte left = 1;
            byte down = 2;
            byte up = 3;
            int lastFoodTime = 0;
            int negativePoints = 0;
            int foodDissapearTime = 0;  
            double sleepTime = 0;
            Position[] directions = new Position[4];
            Random rand = new Random();
            int showMenu = 0;
            while (showMenu == 0)
            {
                showMenu = MainMenu();//main menu
            }
            if (showMenu == 1)
            { // Normal Mode
                foodDissapearTime = 15000;
                sleepTime = 100;
            }
            if (showMenu == 2)
            { // Hard Mode
                foodDissapearTime = 7500;
                sleepTime = 50;
            }
            Snake s = new Snake();
            // Define direction with characteristic of index of array
            s.BgMusic();
            s.Direction(directions);
            List<Position> obstacles = new List<Position>();
            if (showMenu == 1)
            {
                s.Obstacles(obstacles);
            }
            if (showMenu == 2)
            {
                s.Obstacles(obstacles);
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
                obstacles.Add(new Position(rand.Next(1, Console.WindowHeight), rand.Next(0, Console.WindowWidth)));
            }
            //Initializes the direction of the snakes head and the food timer and the speed of the snake.
            int direction = right;
            Console.BufferHeight = Console.WindowHeight;
            lastFoodTime = Environment.TickCount;
            Console.Clear();
            Thread.Sleep(2000);
            //Loop to show obstacles in the console window
            foreach (Position obstacle in obstacles)
            {
                Console.SetCursorPosition(obstacle.col, obstacle.row);
                s.DrawObstacle();
            }
            //Initialise the snake position in top left corner of the windows
            //The snakes length is reduced to 3* instead of 5.
            Queue<Position> snakeElements = new Queue<Position>();
            for (int i = 0; i <= 3; i++)
            {
                snakeElements.Enqueue(new Position(0, i));
            }
            //To position food randomly in the console
            Position food = new Position();
            do
            {
                food = new Position(rand.Next(0, Console.WindowHeight), //Food generated within limits of the console height
                    rand.Next(0, Console.WindowWidth)); //Food generated within the limits of the console width
            }
            //loop to continue putting food in the game
            //put food in random places with "@" symbol
            while (snakeElements.Contains(food) || obstacles.Contains(food));
            Console.SetCursorPosition(food.col, food.row);
            s.DrawFood();
            //during the game, the snake is shown with "*" symbol
            foreach (Position position in snakeElements)
            {
                Console.SetCursorPosition(position.col, position.row);
                s.DrawSnakeBody();
            }
            while (true)
            {
                //negative points increased if the food is not eaten in time
                negativePoints++;
                s.CheckUserInput(ref direction, right, left, down, up);
                //Manages the position of the snakes head.
                Position snakeHead = snakeElements.Last();
                Position nextDirection = directions[direction];
                //Snake position when it goes through the terminal sides
                Position snakeNewHead = new Position(snakeHead.row + nextDirection.row,
                    snakeHead.col + nextDirection.col);
                if (snakeNewHead.col < 0) snakeNewHead.col = Console.WindowWidth - 2;
                if (snakeNewHead.row < 0) snakeNewHead.row = Console.WindowHeight - 2;
                if (snakeNewHead.row >= Console.WindowHeight) snakeNewHead.row = 0;
                if (snakeNewHead.col >= Console.WindowWidth) snakeNewHead.col = 0;
                int gameOver = s.GameOver(snakeElements, snakeNewHead, negativePoints, obstacles);
                if (gameOver == 1) //if snake hits an obstacle then its gameover
                    return;
                //The position of the snake head according the body
                Console.SetCursorPosition(snakeHead.col, snakeHead.row);
                s.DrawSnakeBody();
                //Snake head shape when the user presses the key to change his direction
                snakeElements.Enqueue(snakeNewHead);
                Console.SetCursorPosition(snakeNewHead.col, snakeNewHead.row);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (direction == right) Console.Write(">"); //Snakes head when moving right
                if (direction == left) Console.Write("<");//Snakes head when moving left
                if (direction == up) Console.Write("^");//Snakes head when moving up
                if (direction == down) Console.Write("v");//Snakes head when moving down
                // food will be positioned in different column and row from snakes head
                if (snakeNewHead.col == food.col && snakeNewHead.row == food.row)
                {
                    Console.Beep();//Beep when food is eaten
                    do
                    {
                        food = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    //if the snake consumes the food, lastfoodtime will be reset
                    //new food will be drawn and the snakes speed will increase
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                    Console.SetCursorPosition(food.col, food.row);
                    s.DrawFood();
                    sleepTime--;
                    //setting the obstacles in the game randomly
                    Position obstacle = new Position();
                    do
                    {
                        obstacle = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    //new obstacle will not be placed in the current position of the snake and previous obstacles.
                    //new obstacle will not be placed at the same row & column of food
                    while (snakeElements.Contains(obstacle) ||
                        obstacles.Contains(obstacle) ||
                        (food.row != obstacle.row && food.col != obstacle.row));
                    obstacles.Add(obstacle);
                    Console.SetCursorPosition(obstacle.col, obstacle.row);
                    s.DrawObstacle();
                }
                else
                {
                    // snakes movement shown by blank spaces
                    Position last = snakeElements.Dequeue();
                    Console.SetCursorPosition(last.col, last.row);
                    Console.Write(" ");
                }
                //if snake didnt eat in time, 50 will be added to negative points
                //draw new food randomly after the previous one is eaten
                if (Environment.TickCount - lastFoodTime >= foodDissapearTime)
                {
                    negativePoints = negativePoints + 50;
                    Console.SetCursorPosition(food.col, food.row);
                    Console.Write(" ");
                    do
                    {
                        food = new Position(rand.Next(0, Console.WindowHeight),
                            rand.Next(0, Console.WindowWidth));
                    }
                    while (snakeElements.Contains(food) || obstacles.Contains(food));
                    lastFoodTime = Environment.TickCount;
                }
                //draw food with @ symbol
                Console.SetCursorPosition(food.col, food.row);
                s.DrawFood();
                //snake moving speed increased by 0.01.
                sleepTime -= 0.01;
                //pause the execution of snake moving speed
                Thread.Sleep((int)sleepTime);
            }
        }
    }
}
