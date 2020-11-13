using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//Skeleton Program code for the AQA A Level Paper 1 Summer 2021 examination
//this code should be used in conjunction with the Preliminary Material
//written by the AQA Programmer Team
//developed in the Visual Studio Community Edition programming environment

namespace HexBaronCS
{
    class Program
    {
        static void Main(string[] args)
        {
            bool fileLoaded = true;                                             //Var to store if an external file has correctly loaded in
            Player player1, player2;                                            //Global player instantiations of the player class
            HexGrid grid;                                                       //Global instantiation of the Hex grid to be the main playing grid
            string choice = "";                                                 //Menu choices
            while (choice != "Q")
            {
                DisplayMainMenu();                                              //Displaying the main menu for the application
                choice = Console.ReadLine();
                if (choice == "1")
                {
                    SetUpDefaultGame(out player1, out player2, out grid);       //The out command is like "byRef"
                    PlayGame(player1, player2, grid);
                }
                else if (choice == "2")
                {
                    fileLoaded = LoadGame(out player1, out player2, out grid);  //This uses a try catch in the LoadGame function which returns true if the game has loaded correctly. If error, it returns false and blank grid
                    if (fileLoaded)                                             //If fileLoaded comes back as false, then the game won't play
                        PlayGame(player1, player2, grid);
                }
            }
        }

        public static bool LoadGame(out Player player1, out Player player2, out HexGrid grid)
        {
            string tempFileName = "C:\\Users\\pscullion\\OneDrive - Bedford School\\1_Lessons\\KS5-Comp-Sci\\Pre Release Material\\2021\\game1.txt";
            Console.Write("Enter the name of the file to load: ");
            string fileName = Console.ReadLine();                               //There is no error handling on this input
            List<string> items;                                                 //temp variable to store all of the items from one line in the CSV
            string lineFromFile;                                                //temp variable to store a single line from the CSV before it is split to the List<items>
            try
            {
                using (StreamReader myStream = new StreamReader(tempFileName))  //Read in the whole file using a stream reader
                {
                    lineFromFile = myStream.ReadLine();                         //Read in line one (which contains all the details about player 1)
                    items = lineFromFile.Split(',').ToList();                   //Split line one into a list using the comma as a delimiter
                        //Put items from line 1 into constructor for player 1, (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply)
                    player1 = new Player(items[0], Convert.ToInt32(items[1]), Convert.ToInt32(items[2]), Convert.ToInt32(items[3]), Convert.ToInt32(items[4]));
                    lineFromFile = myStream.ReadLine();                         //Read in line two (which contains all of the details about player 2)
                    items = lineFromFile.Split(',').ToList();
                        //Put items from line 1 into constructor for player 1, (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply)
                    player2 = new Player(items[0], Convert.ToInt32(items[1]), Convert.ToInt32(items[2]), Convert.ToInt32(items[3]), Convert.ToInt32(items[4]));
                    int gridSize = Convert.ToInt32(myStream.ReadLine());        //Read in line three which contains the grid size for that game
                    grid = new HexGrid(gridSize);                               //Pass the grid size into the constructor for the hexGrid for creating a new grid
                    List<string> t = new List<string>(myStream.ReadLine().Split(','));      //Different technique here of reading line four in and splitting it at the same time into a temporary list t
                    grid.SetUpGridTerrain(t);                                   //Pass the temporary list t into the SetupGridTerrain method of the HexGrid to set up what the conditions of each grid tile is (#: forrest, ~: peat bog, (space): field)
                    lineFromFile = myStream.ReadLine();                         //Read in line 5
                    while (lineFromFile != null)                                //Loop to work through the remainder of the file. This will run for the number of rows left in the file, which can change depending on whether or not the player has UPGRADED a piece or SPAWNED a new one
                    {
                        items = lineFromFile.Split(',').ToList();
                        if (items[0] == "1")
                        {
                            grid.AddPiece(true, items[1], Convert.ToInt32(items[2]));
                        }
                        else
                        {
                            grid.AddPiece(false, items[1], Convert.ToInt32(items[2]));
                        }
                        lineFromFile = myStream.ReadLine();                     //Read in the next line - will continue while the new line being read in is not blank
                    }
                }
            }
            catch
            {
                Console.WriteLine("File not loaded");                           //If the file is not loaded correctly
                player1 = new Player("", 0, 0, 0, 0);                           //Instantiates a player with no values. Player constructor is expecting (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply)
                player2 = new Player("", 0, 0, 0, 0);                           //Instantiates a player with no values. Player constructor is expecting (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply)
                grid = new HexGrid(0);                                          //Instantiates a Grid with a size and count of zero. Technically exists, but can't be used for anything. This is just here to ensure there is a grid to stop it crashing
                return false;                                                   //Returns FALSE to the main VOID so it won't run the playGame function
            }
            return true;
        }

        public static void SetUpDefaultGame(out Player player1, out Player player2, out HexGrid grid)
        {
                //The default grid is 8 tiles across. This list works sequentially across the grid with each element representing the associated tile position in the grid (see the tile map in the PM PDF)
            List<string> t = new List<string>() {" ", "#", "#", " ", "~", "~", " ", " ", " ", "~", " ", "#", "#", " ", " ", " "
                                                 , " ", " ", "#", "#", "#", "#", "~", "~", "~", "~", "~", " ", "#", " ", "#", " "};
            int gridSize = 8;
            grid = new HexGrid(gridSize);                                       //Passes 8 into the constructor of the HexGrid which therefore instantiates a new grid over the top of the current one
            player1 = new Player("Player One", 0, 10, 10, 5);                   //Instantiates a player with default values. Player constructor is expecting (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply)
            player2 = new Player("Player Two", 1, 10, 10, 5);                   //Instantiates a player with default values. Player constructor is expecting (str: name, int: VPs, int: fuel, int: Lumber, int: piecesSupply) - Player 2 is already ahead in this game
            grid.SetUpGridTerrain(t);                                           //Sets up the terrain for this grid from the list initialised above
            grid.AddPiece(true, "Baron", 0);                                    //Adds pieces to the tiles in the grid. First param shows if this piece is for player 1 or not, second param is the type of piece, third param is the location (element number) in the hexgrid list
            grid.AddPiece(true, "Serf", 8);                                     //Adds pieces to the tiles in the grid. First param shows if this piece is for player 1 or not, second param is the type of piece, third param is the location (element number) in the hexgrid list
            grid.AddPiece(false, "Baron", 31);                                  //Adds pieces to the tiles in the grid. First param shows if this piece is for player 1 or not, second param is the type of piece, third param is the location (element number) in the hexgrid list
            grid.AddPiece(false, "Serf", 23);                                   //Adds pieces to the tiles in the grid. First param shows if this piece is for player 1 or not, second param is the type of piece, third param is the location (element number) in the hexgrid list
        }

        /// <summary>
        /// Check commands code to ensure that they are valid. Returns true if valid and false if not
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Bool</returns>

        public static bool CheckMoveCommandFormat(List<string> items)           //A MOVE command from the user is in the form of: MOVE INT:startPosition INT:endPosition. 
        {
            int result;
            if (items.Count == 3)                                               //Assuming the command actually has 3 items. If it doesn't it isn't a valid move command
            {
                for (var count = 1; count <= 2; count++)                        //For next loop to iterate through elements 1 and 2 in the list and try converting them from string to int. If they don't convert, the catch picks it up and returns false as they aren't an int
                {
                    try
                    {
                        result = Convert.ToInt32(items[count]);
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;                                                    //If both convert correctly, the command is valid, return true
            }
            return false;                                                       //If there aren't three items, return false
        }

        public static bool CheckStandardCommandFormat(List<string> items)       //All other commands from the user are in the form of: COMMAND INT:Position
        {
            int result;
            if (items.Count == 2)                                               //Assume that the command actually has 2 items. If it doesn't it isn't a valid command anyway
            {
                try
                {   
                    result = Convert.ToInt32(items[1]);                         //Try to convert element 1 in the list from string to int. If it doesn't convert, the catch picks it up and returns false
                }
                catch
                {
                    return false;
                }
                return true;                                                    //If the parameter converts correctly, the command is valid, return true
            }
            return false;                                                       //If there aren't two items, return false
        }

        public static bool CheckUpgradeCommandFormat(List<string> items)        //Upgrade Commands take the form of: UPGRADE STR:("LESS" or "PBDS") INT:Position
        {
            int result;
            if (items.Count == 3)                                               //Assuming the command actually has 3 items. If it doesn't it isn't a valid upgrade command                               
            {
                if (items[1].ToUpper() != "LESS" && items[1].ToUpper() != "PBDS" && items[1].ToUpper() != "FARM")   //Check to see if the user has only written one of the two pieces which can be upgraded. If not, return false
                    return false;
                try
                {
                    result = Convert.ToInt32(items[2]);                         //Try to convert element 2 in the list from string to int. If it doesn't convert, the catch picks it up and returns false                
                }
                catch
                {
                    return false;
                }
                return true;                                                    //If everything converts correctly, the command is valid - return true
            }
            return false;                                                       //If there aren't three items, return false
        }

        public static bool CheckCommandIsValid(List<string> items)              //Receives either a 2 element or 3 element list depending on the command being sent. Each list is a single command. Thes function only checks element 0, the instruction itself in the command
        {
            if (items.Count > 0)                                                //Basic initial error handling which confirms the user didn't just press enter when putting in a command, therefore giving a blank command
            {
                switch (items[0])                                               //The first item in the command list will be the instruction itself. Switch case to check and see what that instruction is                            
                {                                                                                   
                    case "move":                                                //used to move a piece from one tile to anoher
                        {
                            return CheckMoveCommandFormat(items);
                        }

                    case "dig":                                                 //used to obtain fuel       The dig and saw commands don't currently do anything specifically different, they both just call the CheckStandardCommandFormat method                                  
                    case "saw":                                                 //used to obtain lumber
                    case "spawn":
                    case "farm":                                               //used to create a new piece
                        {
                            return CheckStandardCommandFormat(items);
                        }

                    case "upgrade":                                             //used to change a piece into another type of piece
                        {
                            return CheckUpgradeCommandFormat(items);
                        }
                }                                                               //This would be where you put in new commands if a question requests it
            }
            return false;                                                       //If it isn't one of these, then the command must be invalid, therefore return false
        }

        public static void PlayGame(Player player1, Player player2, HexGrid grid)   //These will be populated either with default game or loaded game
        {
            bool gameOver = false;                                              //Hold the main game loop until the game is won               
            bool player1Turn = true;                                            //Boolean to flip flop if it is player 1 or player 2
            bool validCommand;                                                  //Boolean which denotes if all three commands from a player are valid    
            List<string> commands = new List<string>();                         //list to contain all three commands from the user
            Console.WriteLine("Player One current state - " + player1.GetStateString());        //Gets the current amount of VPs, Lumber, Fuel and piece supply from player 1 object and displays to screen
            Console.WriteLine("Player Two current state - " + player2.GetStateString());        //Gets the current amount of VPs, Lumber, Fuel and piece supply from player 2 object and displays to screen
            do
            {                                                                   //Do while !gameOver || !player1Turn
                Console.WriteLine(grid.GetGridAsString(player1Turn));           //returns a string (with carriage returns) showing the grid for this game ##########what is Player1Turn doing here?
                if (player1Turn)
                    Console.WriteLine(player1.GetName() + " state your three commands, pressing enter after each one.");
                else
                    Console.WriteLine(player2.GetName() + " state your three commands, pressing enter after each one.");
                for (int count = 1; count <= 3; count++)                        //Definite iteration to input the 3 commands from the user. They are added to the "commands" list
                {
                    Console.Write("Enter command: ");
                    commands.Add(Console.ReadLine().ToLower());                 //No error handling is needed here because the list will be sent through the "checkCommandIsValid()" Method. Commands are either two or three elements in length, separated by space
                }
                foreach (var c in commands)
                {
                    List<string> items = new List<string>(c.Split(' '));        //Split each command (single command from the user) in the list "commands" from the user using the space as a delimiter
                    validCommand = CheckCommandIsValid(items);                  //Send individual command (Which will be a list of either 2 or 3 elements) to the checkCommandIsValid() method
                    if (!validCommand)
                        Console.WriteLine("Invalid command");                   //If checkCommandIsValid() came back false for some reason, then this shows "Invalid Command". CheckCommandIsValid() is done per command, however, therefore some could fail, but not all
                    else
                    {
                        int fuelChange = 0;                                     //default values for the how much each can change as a result of player actions.
                        int lumberChange = 0;
                        int supplyChange = 0;
                        string summaryOfResult;
                        if (player1Turn)
                        {
                            //This is the business end of the application which works out the result of a command from a player and displays the appropriate message on the screen. 
                            summaryOfResult = grid.ExecuteCommand(items, ref fuelChange, ref lumberChange, ref supplyChange,
                                player1.GetFuel(), player1.GetLumber(), player1.GetPiecesInSupply());
                            player1.UpdateLumber(lumberChange);                 //Updates the changes to the amount of lumber for the command selected by the user
                            player1.UpdateFuel(fuelChange);                     //Updates the changes to the amount of fuel for the command selected by the user
                            if (supplyChange == 1)
                                player1.RemoveTileFromSupply();
                        }
                        else
                        {
                            summaryOfResult = grid.ExecuteCommand(items, ref fuelChange, ref lumberChange, ref supplyChange,
                                player2.GetFuel(), player2.GetLumber(), player2.GetPiecesInSupply());
                            player2.UpdateLumber(lumberChange);                 //Updates the changes to the amount of lumber for the command selected by the user
                            player2.UpdateFuel(fuelChange);                     //Updates the changes to the amount of fuel for the command selected by the user
                            if (supplyChange == 1)
                            {
                                player2.RemoveTileFromSupply();
                            }
                        }
                        Console.WriteLine(summaryOfResult);
                    }
                }

                commands.Clear();                                               //Clears out the List "commands" ready for the next turn
                player1Turn = !player1Turn;                                     //Switches to player 2 turn
                int player1VPsGained = 0;
                int player2VPsGained = 0;
                if (gameOver)
                {
                    grid.DestroyPiecesAndCountVPs(ref player1VPsGained, ref player2VPsGained);
                }
                else
                {
                    gameOver = grid.DestroyPiecesAndCountVPs(ref player1VPsGained, ref player2VPsGained);   //Calculates which pieces need to be destroyed based on their neighbours and calculates the victory points to denote if the game has finished
                    player1.AddToVPs(player1VPsGained);
                    player2.AddToVPs(player2VPsGained);
                    Console.WriteLine("Player One current state - " + player1.GetStateString());
                    Console.WriteLine("Player Two current state - " + player2.GetStateString());
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();

                }
                   
            }
            while (!gameOver || !player1Turn);
            Console.WriteLine(grid.GetGridAsString(player1Turn));
            DisplayEndMessages(player1, player2);
        }


        /// <summary>
        /// All the UI commands
        /// </summary>


        public static void DisplayEndMessages(Player player1, Player player2)
        {
            Console.WriteLine();
            Console.WriteLine(player1.GetName() + " final state: " + player1.GetStateString());     //Displays the final player state: VPs, Pieces in Supply, Lumber, Fuel
            Console.WriteLine();
            Console.WriteLine(player2.GetName() + " final state: " + player2.GetStateString());     //Displays the final player state: VPs, Pieces in Supply, Lumber, Fuel
            Console.WriteLine();
            if (player1.GetVPs() > player2.GetVPs())                                                //Detirmine who wins based on the number of VPs
            {
                Console.WriteLine(player1.GetName() + " is the winner!");
            }
            else
            {
                Console.WriteLine(player2.GetName() + " is the winner!");
            }
        }

        public static void DisplayMainMenu()                                    //Main menu system for the game - default game, load game and quit.
        {
            Console.WriteLine("1. Default game");
            Console.WriteLine("2. Load game");
            Console.WriteLine("Q. Quit");
            Console.WriteLine();
            Console.Write("Enter your choice: ");
        }
    }
    /// <summary>
    /// OOP classes
    /// </summary>
    /// 
    class Piece
    {
        protected bool destroyed, belongsToPlayer1;                             //If a piece of destroyed it won't be displayed on the hex grid
        protected int fuelCostOfMove, VPValue, connectionsToDestroy;
        protected string pieceType;                                             //Type of piece, SERF, LESS, PBDS or Baron                          

        public Piece(bool player1)                                              //If the bool is set to true, this is a player 1 piece, otherwise it is a player 2 piece
        {
            fuelCostOfMove = 1;                                                 //The default amount of fuel used to move a piece. Some tiles cost more
            belongsToPlayer1 = player1;                                         //Initial values for a standard SERF piece        
            destroyed = false;                                                  
            pieceType = "S";
            VPValue = 1;
            connectionsToDestroy = 2;                                           //By default, if a piece of next to two other piece, then they are all destroyed
        }

        //Virtual methods exist in both the base class (this class) and any derived classes (Baron, LESS etc).
        //It is used when a method's basic functionality is the same but sometimes more functionality is needed in the derived class. 
        //A virtual method is created in the base class that can be overriden in the derived class

        public virtual int GetVPs()                                             //Gets the number of victory points for this piece. This method is currently only in this class
        {
            return VPValue;
        }

        public virtual bool GetBelongsToPlayer1()                               //Gets if the piece belongs to player 1 or player 2. This method is currently only in this class
        {
            return belongsToPlayer1;
        }

        public virtual int CheckMoveIsValid(int distanceBetweenTiles, string startTerrain, string endTerrain) //This method is overridden in the following derived classes: BaronPiece, LESSPiece, PBDSPiece
        {
            if (distanceBetweenTiles == 1)                                      //Another check to confirm that a move is valid. This looks at the distance between the start and end tile and checks its 1
            {
                if (startTerrain == "~" || endTerrain == "~")                   //Assuming the distance isn't too far, this check the terrain and if the piece if moving from or into peat, then the fuel cost doubles
                {
                    return fuelCostOfMove * 2;
                }
                else
                {
                    return fuelCostOfMove;
                }
            }
            return -1;
        }

        public virtual bool HasMethod(string methodName)                        //The check to confirm if this piece has the method named in the param. This enabled system reflection to look for the move, spawn, upgrade, saw, dig commands. This method is currently only in this class
        {
            return this.GetType().GetMethod(methodName) != null;                //This will allow students to add new methods and call them more easily because it confirms the piece has the method typed in by the user.
        }

        public virtual int GetConnectionsNeededToDestroy()                      //Returns how many pieces a piece needs to be next to for it to be destroyed. This method is currently only in this class
        {
            return connectionsToDestroy;
        }

        public virtual string GetPieceType()                                    //Returns the type of piece, Baron, LESS, PBDS, SERF etc. This method is currently only in this class
        {
            if (belongsToPlayer1)
            {
                return pieceType;
            }
            else
            {
                return pieceType.ToLower();
            }
        }

        public virtual void DestroyPiece()                                      //Destroys a piece so it doesn't show. This method is currently only in this class              
        {
            destroyed = true;
        }
    }

    class BaronPiece : Piece                                                    //Inherits Piece. This allows us to inherit some of the base class properties and overide some of its methods
    {
        public BaronPiece(bool player1)
            : base(player1)
        {
            pieceType = "B";                                                    //These are set in the base class to override a standard piece setting
            VPValue = 10;
        }

        public override int CheckMoveIsValid(int distanceBetweenTiles, string startTerrain, string endTerrain)  //This overides the base class method because a Baron can move without being affected by the terrain.
        {
            if (distanceBetweenTiles == 1)                                      //Assuming the distance between the tiles is 1, then return the fuel cost for the move (1)
            {
                return fuelCostOfMove;
            }
            return -1;
        }
    }

    class LESSPiece : Piece                                                     //Inherits Piece. This allows us to inherit some of the base class properties and overide some of its methods
    {
        public LESSPiece(bool player1)
            : base(player1)
        {
            pieceType = "L";                                                    //These are set in the base class to override a standard piece setting
            VPValue = 3;
        }

        public override int CheckMoveIsValid(int distanceBetweenTiles, string startTerrain, string endTerrain)
        {                                                                       //This overides the base class method because a LESS can't move in a forest.
            if (distanceBetweenTiles == 1 && startTerrain != "#")
            {
                if (startTerrain == "~" || endTerrain == "~")                   //If the LESS is moving in or out of a peat bog then the fuel cost is double.
                {
                    return fuelCostOfMove * 2;
                }
                else
                {
                    return fuelCostOfMove;
                }

            }
            return -1;
        }

        public int Saw(string terrain)                                          //The LESS has the functionality to saw lumber. This checks to confirm that the terrain the LESS is currently in is forest, otherwise it returns 0
        {
            if (terrain != "#")
            {
                return 0;
            }
            return 1;
        }

    }

    class PBDSPiece : Piece                                                     //Inherits piece. This allows us to inherit some of the base class properties and overide some of its methods
    {
        static Random rNoGen = new Random();

        public PBDSPiece(bool player1)
            : base(player1)
        {
            pieceType = "P";                                                    //These are set in the base class to override a standard piece setting. PBDS can only move in field or forest and that costs 2 fuel    
            VPValue = 2;
            fuelCostOfMove = 2;
        }

        public override int CheckMoveIsValid(int distanceBetweenTiles, string startTerrain, string endTerrain)
        {                                                                       //This overides the base class method because a PBDS can't move in a peat bog.
            if (distanceBetweenTiles != 1 || startTerrain == "~")               //This means that once a PBDS moves into a peat bog, it can only move out once it has dug.
            {
                return -1;
            }
            return fuelCostOfMove;
        }

        public int Dig(string terrain)                                          //The PBDS has additional functionality to be able to dig for peat as fuel.
        {
            if (terrain != "~")                                                 //It can only dig if it is in a peat bog
            {
                return 0;
            }
            if (rNoGen.NextDouble() < 0.9)                                      //Generates a random number to determine if the amount of fuel returned from the peat bog is 1 or 5
            {
                return 1;
            }
            else
            {
                return 5;
            }
        }
    }
    class FarmPiece : Piece                                                     //Inherits piece. This allows us to inherit some of the base class properties and overide some of its methods
    {

        public FarmPiece(bool player1)
            : base(player1)
        {
            pieceType = "F";                                                    //These are set in the base class to override a standard piece setting. PBDS can only move in field or forest and that costs 2 fuel    
            VPValue = 2;
            fuelCostOfMove = 2;
        }

        public override int CheckMoveIsValid(int distanceBetweenTiles, string startTerrain, string endTerrain)
        {                                                                       //This overides the base class method because a PBDS can't move in a peat bog.
            if (distanceBetweenTiles != 1 || startTerrain == "~")               //This means that once a PBDS moves into a peat bog, it can only move out once it has dug.
            {
                return -1;
            }
            return fuelCostOfMove;
        }

        public int Dig(string terrain)                                          //The PBDS has additional functionality to be able to dig for peat as fuel.
        {
            if (terrain != "~")                                                 //It can only dig if it is in a peat bog
            {
                return 0;
            }
            else
            {
                return 5;
            }
        }
    
        public int Farm(string terrain)
        {
            if (terrain == "#" || terrain == "~")
            {
                return 4;
            }
            return 1;
        }
    }
    class Tile
    {
        protected string terrain;                                               //Stores the type of terrain for this piece. # -> Forest, ~ -> Peat Bog, " " (empty space) -> Basic Field                         
        protected int x, y, z;                                                  //Location of the tile (used to calculate the distance between two tiles which is required for calculating movement distance
        protected Piece pieceInTile;                                            //Instatiates a basic piece to put on the tile (this can then be updated depending on what type of piece it is.
        protected List<Tile> neighbours = new List<Tile>();                     //A small list of the tiles around each piece. This is used to calculate when a piece of destroyed and also used for checking that spawning is valid

        public Tile(int xcoord, int ycoord, int zcoord)                         //Default settings for a tile. Initially all tiles are fields with no pieces in them.
        {
            x = xcoord;
            y = ycoord;
            z = zcoord;
            terrain = " ";
            pieceInTile = null;
        }

        public int GetDistanceToTileT(Tile t)    
            //Step 1: Break this down into stages. Work out the absolute difference between x of target tile and current tile to check movement across the board left and right
            //Step 2: Work out the absolute difference between y of the target tile and current tile to check movement across the board up and down
            //Step 3: Look at the Max of these two numbers (which if either is greater than 1, the move is invalid)
            //Step 4: Work out the absolute difference between z of the target tile and current tile to check movement across the board diagonally
            //Step 5: Look at the Max of the calculations from Step 3 and Step 4 (which if either is greater than 1, the move is invalid)
        {
            return Math.Max(Math.Max(Math.Abs(Getx() - t.Getx()), Math.Abs(Gety() - t.Gety())), Math.Abs(Getz() - t.Getz()));
        }

        public void AddToNeighbours(Tile n)                                     //Adds a new neighbour to this tiles list of neighbours. The tile index (from the tile list in the HexGrid class) is passed in as a parameter. 
        {
            neighbours.Add(n);
        }

        public List<Tile> GetNeighbours()                                       //Returns this tiles list of neighbours so they can be checked when looking to destroy pieces or when looking to see what is next to a Baron before spawning
        {
            return neighbours;
        }

        public void SetPiece(Piece thePiece)                                    //Updates a piece with a type (Baron, LESS, SERF or PBDS)
        {
            pieceInTile = thePiece;
        }

        public void SetTerrain(string t)                                        //Sets the terrain for this piece # -> Forest, ~ -> Peat Bog, " " (empty space) -> Basic Field  
        {
            terrain = t;
        }

        public int Getx()                                                       //Gets the X coordinate for this tile
        {
            return x;
        }

        public int Gety()                                                       //Gets the Y coordinate for this tile
        {
            return y;
        }

        public int Getz()                                                       //Gets the Z coordinate for this tile
        {
            return z;
        }

        public string GetTerrain()                                              //Gets the terrain for this tile
        {
            return terrain;
        }

        public Piece GetPieceInTile()                                           //Gets the piece currently on this tile (if there is one)
        {
            return pieceInTile;
        }
    }

    class HexGrid
    {
        protected List<Tile> tiles = new List<Tile>();                          //List of all the tiles on the grid. The index ID of the tile in the list is the red figure on the example grid on page 5 of the pre-release material
        protected List<Piece> pieces = new List<Piece>();                       //List of all the pieces in the game - #####NOT SURE WHY THIS IS HERE???
        int size;                                                               //The size of the grid in terms of number of positions in the tiles <list>
        bool player1Turn;

        public HexGrid(int n)
        {
            size = n;
            SetUpTiles();                                                       //Populates the tiles <list> with blank tiles
            SetUpNeighbours();                                                  //Populates each tiles neighbours <list> with their individual neighbour
            player1Turn = true;                                                 //Default for the game is that player 1 starts
        }

        public void SetUpGridTerrain(List<string> listOfTerrain)                //Sets the terrain for all the tiles in the tiles <list> using the data either from the imported CSV file or the default gane
        {
            for (int count = 0; count < listOfTerrain.Count; count++)
            {
                tiles[count].SetTerrain(listOfTerrain[count]);                  //Looping through the tiles <list> applying a terrain to each time as appropriate
            }
        }

        public void AddPiece(bool belongsToPlayer1, string typeOfPiece, int location)   //Adds a piece to a tile. The parameters use a boolean to denote if it is for player 1 or 2 and then the type of piece to be added and its location in the tiles <list> 
        {
            Piece newPiece;
            if (typeOfPiece == "Baron")                     
            {
                newPiece = new BaronPiece(belongsToPlayer1);
            }
            else if (typeOfPiece == "LESS")                                     //This is called if the player is upgrading from a SERF to a LESS
            {
                newPiece = new LESSPiece(belongsToPlayer1);
            }
            else if (typeOfPiece == "PBDS")                                     //This is called if the player is upgrading from a SERF to a PBDS
            {
                newPiece = new PBDSPiece(belongsToPlayer1);
            }
            else if (typeOfPiece == "FARM")                                     //This is called if the player is upgrading from a SERF to a PBDS
            {
                newPiece = new FarmPiece(belongsToPlayer1);
            }
            else
            {
                newPiece = new Piece(belongsToPlayer1);                             
            }
            pieces.Add(newPiece);                                               //Adds the new piece to the piece <list> ######NOT SURE WHAT THIS IS FOR?
            tiles[location].SetPiece(newPiece);                                 //Because the tiles <list> has been populated with blank tiles at initialisation, then this updates the <list> with the "new" piece
        }

        public string ExecuteCommand(List<string> items, ref int fuelChange, ref int lumberChange,      //We can only have got to his point if the command from the user is valid due to previous checks
                                     ref int supplyChange, int fuelAvailable, int lumberAvailable,      //fuelChange, lumberChange and supplyChange are all passed by ref rather than by val
                                     int piecesInSupply)
        {
            switch (items[0])                                                   //The first element in the items list is the command from the user
            {
                case "move":
                    {
                        int fuelCost = ExecuteMoveCommand(items, fuelAvailable);  //The "Move" command only has the "cost" of reducing fuel. If the command is successful, there will be a fuel cost. If the command can't be executed, there will be no cost change
                        if (fuelCost < 0)                                       //If the command isn't successful (Reasons: Start or End Tile is invalid, a piece already occupies the End Tile or the player doesn't have enough fuel to make that move), it returns -1
                        {
                            return "That move can't be done";
                        }
                        fuelChange = -fuelCost;                                 //Assuming the move can be done, there will be a charge for the move. This will either be 1 for a normal move or 2 depending on the piece and terrain (see pg 6 in the PM material for fuel costs)
                        break;
                    }
                case "saw":                                                     //Bcause there is no break command in the "Saw" case, then C# just does the next item in the switch case, therefore no need to repeat the "ExecuteCommandInTile" code
                case "dig":
                case "farm":
                    {

                        if (!ExecuteCommandInTile(items, ref fuelChange, ref lumberChange))             //The Baron, LESS and PBDS pieces all inherit the Piece class . This uses system reflection to ask the relevant sub class if they have the methods "Dig" or "Saw". Depending on which piece is locating that tile, they will return true / false
                        {
                            return "Couldn't do that";                          //Returns this if 1: The player doesn't have enough fuel or lumber perform the dig / saw or 2: the piece in that tile doesn't have that functionality / Method (system.reflection...)
                        }
                        break;
                    }
                case "spawn":
                    {
                        int lumberCost = ExecuteSpawnCommand(items, lumberAvailable, piecesInSupply);   //The "Spawn" command doesn't move any pieces - it only creates a new one, at the cost of 3 Lumber and 1 Piece from Supplies. If the command can't be executed, there will be no cost change
                        if (lumberCost < 0)                                     //If the command isn't successful (Reasons: Not enough pieces in supply, not enough lumber, not a valid tile location, not next to own baron), it returns -1
                            return "Spawning did not occur";
                        lumberChange = -lumberCost;                             //Assuming the spawn can be done, there will be a cost for the spawn. I will be 3 pieces of lumber
 
                         supplyChange = 1;                                      //Assuming the spawn can happen, the amount that supply needs to decrease by is 1 (one new piece made)
                        break;
                    }
                case "upgrade":
                    {
                        int lumberCost = ExecuteUpgradeCommand(items, lumberAvailable);                 //The "Upgrade" command doesn't move any pieces - it changes a Standard SERF piece into a Peat Bog Digger or a Lumber Specialist at the cost of 5 Lumber. If the command can't be executed, there will be no cost change
                        if (lumberCost < 0)                                                             //If the command isn't successful (Reasons: not a valid tile location, not enough lumber, incorrect name given for piece to upgrade to), it returns -1
                            return "Upgrade not possible";
                        lumberChange = -lumberCost;                             //Assuming the spawn can be done, there will be a cost for the upgrade. I will be 5 pieces of lumber
                        break;
                    }
            }
            return "Command executed";                                          //Return for the command summary to display to screen.
        }

        private bool CheckTileIndexIsValid(int tileToCheck)
        {
            return tileToCheck >= 0 && tileToCheck < tiles.Count;               //Pieces are stored in a list, therefore checking to see if their index is valid is just a case of checking to see if it is in the list range.
        }

        private bool CheckPieceAndTileAreValid(int tileToUse)
        {
            if (CheckTileIndexIsValid(tileToUse))                               //Pieces are stored in a list, therefore checking to see if their index is valid is just a case of checking to see if it is in the list range.
            {
                Piece thePiece = tiles[tileToUse].GetPieceInTile();             //Checking to see if there is actually a tile piece at that location
                if (thePiece != null)                                           //Assuming that there is a tile at that location
                {
                    if (thePiece.GetBelongsToPlayer1() == player1Turn)          //Checking to see if the tile belongs to player 1
                    {
                        return true;                                            //Assuming we got through all of this nested selection, the piece must be valid - return true
                    }
                }
            }
            return false;
        }

        private bool ExecuteCommandInTile(List<string> items, ref int fuel, ref int lumber) //This executes a command to be performed by a piece in a tile in the tiles <list>
        {
            int tileToUse = Convert.ToInt32(items[1]);
            if (CheckPieceAndTileAreValid(tileToUse) == false)                              //Check to confirm the tile is valid on the board and there there is actually a piece located there.                     
            {
                return false;
            }
            Piece thePiece = tiles[tileToUse].GetPieceInTile();
            items[0] = items[0][0].ToString().ToUpper() + items[0].Substring(1);            //This just changes the first Char in item[0] to capitalise it
            if (thePiece.HasMethod(items[0]))                                               //Using system reflection to see if the piece has the method (dig, saw etc)
            {
                string methodToCall = items[0];
                Type t = thePiece.GetType();                                                //Temporarily create a variable to store the type of object. (not the name, but the object type)
                System.Reflection.MethodInfo method = t.GetMethod(methodToCall);            //Temporary local method created as an instance of the method from the piece class
                object[] parameters = { tiles[tileToUse].GetTerrain() };                    //Temporary list of the parameters to be passed into the temp method (which for dig and saw is the terrain type)
                if (items[0] == "Saw")
                {
                    lumber += Convert.ToInt32(method.Invoke(thePiece, parameters));         //If the command from the user is "Saw" then call the "Saw" method for this piece and pass in the terrain as the properties
                }
                else if (items[0] == "Dig") 
                {
                    fuel += Convert.ToInt32(method.Invoke(thePiece, parameters));           //If the command from the user is "Dig" then call the "Dig" method for this piece and pass in the terrain as the properties
                    if (Math.Abs(fuel) > 2)
                    {
                        tiles[tileToUse].SetTerrain(" ");                                   //Assuming the "Dig" occured correctly then set the terrain for this particular tile to a standard field
                    }
                }
                else if (items[0] == "farm") 
                {
                    fuel += Convert.ToInt32(method.Invoke(thePiece, parameters));           //If the command from the user is "Dig" then call the "Dig" method for this piece and pass in the terrain as the properties
                    tiles[tileToUse].SetTerrain(" ");                                   //Assuming the "Dig" occured correctly then set the terrain for this particular tile to a standard field
                    
                }
                return true;                                                                //Command all ran correctly - return true
            }
            return false;                                                                   //Something didn't run correctly in the command, return false so it can be displayed back to the user.
        }

        private int ExecuteMoveCommand(List<string> items, int fuelAvailable)               //Param 1: An individual command from the user. Param 2: The amount of fuel that player has available
        {
            int startID = Convert.ToInt32(items[1]);                                        //Move commands have 3 elements. Element 1 is the start ID / position for the piece. 
            int endID = Convert.ToInt32(items[2]);                                          //Element 2 is the end ID / position for the piece
            if (!CheckPieceAndTileAreValid(startID) || !CheckTileIndexIsValid(endID))       //Check to confirm that the start and end points are on the board and are valid pieces that can move
            {
                return -1;
            }
            Piece thePiece = tiles[startID].GetPieceInTile();                               //Temporary piece copy of the piece at the start location
            if (tiles[endID].GetPieceInTile() != null)                                      //Check to confirm that there is not already a piece at the end location. If so, error and return -1
            {
                return -1;
            }
            int distance = tiles[startID].GetDistanceToTileT(tiles[endID]);                 //Calculating the distance between the start point and the end point. 
            int fuelCost = thePiece.CheckMoveIsValid(distance, tiles[startID].GetTerrain(), tiles[endID].GetTerrain()); //Calculating the fuel cost between the start and end point by checking the move it valid, confirming the terrain at the start and end location
            if (fuelCost == -1 || fuelAvailable < fuelCost)                                 //If the move isn't valid or the fuel cost is greater than the amount of fuel the player has, then error and return -1
            {
                return -1;
            }
            MovePiece(endID, startID);                                                      //Assuming all is good, we have got to this point, move the piece on the board
            return fuelCost;                                                                //Return the updated fuel cost for the summary.
        }

        private int ExecuteSpawnCommand(List<string> items, int lumberAvailable, int piecesInSupply)
        {
            int tileToUse = Convert.ToInt32(items[1]);                          //The tile the player wants to spawn to is in element 2 of the Commands list
            if (piecesInSupply < 1 || lumberAvailable < 3 || !CheckTileIndexIsValid(tileToUse))     //If the player doesn't have any pieces left to spawn, or not enough lumber (3 needed), or the tile to spawn at isn't one on the board, return -1
            {
                return -1;
            }
            Piece ThePiece = tiles[tileToUse].GetPieceInTile();                 //Create a temporary piece copy of the piece the user wants to spawn so we can check it
            if (ThePiece != null)                                               //Only actual game pieces are shown in the tiles list, herefore if this check returns not null, it means there MUST be another piece already there
            {
                return -1;                                                      //If there is already a piece in the tile the player wants to spawn to, then the spawn can't happen -> return -1
            }
            bool ownBaronIsNeighbour = false;                                   //Boolean variable to confirm if the tile the user wants to spawn to is actually next to the current player Baron piece
            List<Tile> listOfNeighbours = new List<Tile>(tiles[tileToUse].GetNeighbours());         //A list of tiles which are next to the tile position the user wants to spawn at. Each tile when instantiated creates a list of it's neighbours, so this just gets that list
            foreach (var n in listOfNeighbours)
            {
                ThePiece = n.GetPieceInTile();                                  //Get the details of the piece which is on each tile in the neighbouring tiles list
                if (ThePiece != null)                                           //If there isn't a piece at a location in the neighbour list, then it will return null for that location, therefore by default it can't be a baron                                                        
                {
                    if (player1Turn && ThePiece.GetPieceType() == "B" || !player1Turn && ThePiece.GetPieceType() == "b") //If it is player 1's turn and the piece found at that location in the neighbour list is a "B" then it is a baron. Same for NOT player 1 (i.e. player 2) and "b"
                    {
                        ownBaronIsNeighbour = true;                             //If a baron is found, break out of the for each loop and set ownBaronIsNeighbour to True
                        break;
                    }
                }
            }
            if (!ownBaronIsNeighbour)                                           //If we get this far then none of the neighbours for that player are barons, therefore the user can't spawn there --> return -1
            {
                return -1;
            }
            Piece newPiece = new Piece(player1Turn);                            //If we get this far, then one of the neighbours for that player was a baron, therefore instantiate a new piece. All new pieces have the default value of Serf (s)
            pieces.Add(newPiece);
            tiles[tileToUse].SetPiece(newPiece);                                //Sets the element in the <list> tiles to the new serf piece
            return 3;                                                           //Returns 3 --> the cost in lumber for creating a new piece
        }

        private int ExecuteUpgradeCommand(List<string> items, int lumberAvailable)
        {
            int tileToUse = Convert.ToInt32(items[2]);                          //The tile the player wants to upgrade is at element 2 of the Commands list
            if (!CheckPieceAndTileAreValid(tileToUse) || lumberAvailable < 5 || !(items[1] == "pbds" || items[1] == "less" || items[1] == "farm")) //If the tile isn't on the board, or the player doesn't have enough lumber to upgrade command isn't write, then return -1
            {
                return -1;
            }
            else
            {
                Piece thePiece = tiles[tileToUse].GetPieceInTile();             //Create a temporary copy of the piece at the piece at the location selected by the user in the <list> tiles
                if (thePiece.GetPieceType().ToUpper() != "S")                   //Check to see if that location is a standard SERF piece. If it isn't, it can't be upgraded, therefore return -1
                {
                    return -1;
                }
                thePiece.DestroyPiece();                                        //If there is a piece there and it is a standard SERF piece, then destroy it so it can be changed to a new piece
                if (items[1] == "pbds")                                         //Looks at element 2 in the <list> commands to see if the player wants to create a Peat Bog Digger or a Lumber Engineer Specialist
                {
                    thePiece = new PBDSPiece(player1Turn);
                }
                else if(items[1] =="farm")
                {
                    thePiece = new FarmPiece(player1Turn);
                }
                else
                {
                    thePiece = new LESSPiece(player1Turn);
                }
                pieces.Add(thePiece);                               
                tiles[tileToUse].SetPiece(thePiece);                            //Creates the new playing piece and adds it to the <list> tiles at the location set by the user
                return 5;                                                       //Returns 5 --> the cost in lumber for upgrading a new piece
            }
        }

        private void SetUpTiles()                                               //Creates a new list of blank tiles ready to have pieces added in - either from the external CSV or the default game
        {                                                                       //This should not need adjusting otherwise it will break the board
            int evenStartY = 0;
            int evenStartZ = 0;
            int oddStartZ = 0;
            int oddStartY = -1;
            int x, y, z;
            for (int count = 1; count <= size / 2; count++)
            {
                y = evenStartY;
                z = evenStartZ;
                for (x = 0; x <= size - 2; x += 2)
                {
                    Tile tempTile = new Tile(x, y, z);
                    tiles.Add(tempTile);
                    y -= 1;
                    z -= 1;
                }
                evenStartZ += 1;
                evenStartY -= 1;
                y = oddStartY;
                z = oddStartZ;
                for (x = 1; x <= size - 1; x += 2)
                {
                    Tile tempTile = new Tile(x, y, z);
                    tiles.Add(tempTile);
                    y -= 1;
                    z -= 1;
                }
                oddStartZ += 1;
                oddStartY -= 1;
            }
        }

        private void SetUpNeighbours()                                          //Iterates through all of the tiles calculating the distance between them and the tiles around them. If it is 1, then it is a neighbough                                         
        {                                                                       //This is ineffient O(n2)
            foreach (var fromTile in tiles)
            {
                foreach (var toTile in tiles)
                {
                    if (fromTile.GetDistanceToTileT(toTile) == 1)
                    {
                        fromTile.AddToNeighbours(toTile);
                    }
                }
            }
        }

        public bool DestroyPiecesAndCountVPs(ref int player1VPs, ref int player2VPs)        //Runs after each go to count how many pieces are next to each other another and destroys them if they are next to two other pieces
        {
            bool baronDestroyed = false;                                                    //Assume at the start that the Baron hasn't been destroyed
            List<Tile> listOfTilesContainingDestroyedPieces = new List<Tile>();             //Blank list to contain all of the tiles which will need destroying (this can then be iterated through later on.
            foreach (var t in tiles)
            {
                if (t.GetPieceInTile() != null)                                             //Only run this code if the tile actually contains a piece, otherwise don't look at it
                {
                    List<Tile> listOfNeighbours = new List<Tile>(t.GetNeighbours());        //Temporary list of neighbours of the current piece this loop is looking at
                    int noOfConnections = 0;                                                //Assume it has no connections (i.e. is not next to any other pieces)
                    foreach (var n in listOfNeighbours)                                     //Now start looking through the list of neighbours for the piece in question.
                    {
                        if (n.GetPieceInTile() != null)                                     //If one of the neighbours is not null, this means that there must be a piece next to it, therefore 1 connection
                        {
                            noOfConnections += 1;
                        }
                    }
                    Piece thePiece = t.GetPieceInTile();
                    if (noOfConnections >= thePiece.GetConnectionsNeededToDestroy())        //Look at the piece in question and see how many connections need to be made for it to be destroyed
                    {
                        thePiece.DestroyPiece();                                            //If the number of connections that the piece in question has is greater than the number needed to destroy it, then destroy it
                        if (thePiece.GetPieceType().ToUpper() == "B")                       //If the piece in question is a Baron, then set baronDestroyed to true - end game
                            baronDestroyed = true;
                        listOfTilesContainingDestroyedPieces.Add(t);                        //If we've got this far then the piece in question needs to be destroyed. Add list of pieces which are going to be destoyed
                        if (thePiece.GetBelongsToPlayer1())                                 //Depending on who's turn it is, calculate the number of victory points.
                        {
                            player2VPs += thePiece.GetVPs();
                        }
                        else
                        {
                            player1VPs += thePiece.GetVPs();
                        }
                    }
                }
            }
            foreach (var t in listOfTilesContainingDestroyedPieces)             //Work through the list of piece of be destroyed and call the setPiece() method and set a piece back to null (empty field).
            {
                t.SetPiece(null);                                               //This will only run for the number of pieces that were destroyed in this turn, which could be zero if no pieces were destroyed
            }
            return baronDestroyed;                                              //Return back if the baron was destroyed (end of game)
        }

        public string GetGridAsString(bool p1Turn)                              //Display current board as a string. This method calls a number of other methods to create a single string for the display board
        {
            int listPositionOfTile = 0;
            player1Turn = p1Turn;
            string gridAsString = CreateTopLine() + CreateEvenLine(true, ref listPositionOfTile);   //Uses the CreateTopLine, EvenLine and OddLine methods to draw the grid onto the screen. Any changes on what is to be displayed within the grid needs to happen here
            listPositionOfTile += 1;
            gridAsString += CreateOddLine(ref listPositionOfTile);
            for (var count = 1; count <= size - 2; count += 2)
            {
                listPositionOfTile += 1;
                gridAsString += CreateEvenLine(false, ref listPositionOfTile);
                listPositionOfTile += 1;
                gridAsString += CreateOddLine(ref listPositionOfTile);
            }
            return gridAsString + CreateBottomLine();
        }

        private void MovePiece(int newIndex, int oldIndex)
        {
            tiles[newIndex].SetPiece(tiles[oldIndex].GetPieceInTile());         //This is the method which actually moved a piece from one tile to another by finding out what the type was at the old index, setting the new index to that type and then setting the old index to null
            tiles[oldIndex].SetPiece(null);
        }

        public string GetPieceTypeInTile(int ID)                                //This returns the type of piece at a particular location in the hex grid.
        {                                                                       //It uses the GetPieceType virtual method and returns the type of piece, Baron, LESS, PBDS, SERF etc
            Piece thePiece = tiles[ID].GetPieceInTile();
            if (thePiece == null)
            {
                return " ";
            }
            else
            {
                return thePiece.GetPieceType();
            }
        }

        private string CreateBottomLine()                                       //Draws the bottom line of a hex. This is repeated for the number of hex's in a grid row
        {
            string line = "   ";
            for (var count = 1; count <= size / 2; count++)
            {
                line += @" \__/ ";                                              //The @ symbol is needed to tell C# that the \ is not an escape char
            }
            return line + Environment.NewLine;
        }

        private string CreateTopLine()                                          //Draws the top line of a hex. This is repeated for the number of hex's in a grid row
        {
            string line = Environment.NewLine + "  ";
            for (var count = 1; count <= size / 2; count++)
            {
                line += "__    ";
            }
            return line + Environment.NewLine;
        }

        private string CreateOddLine(ref int listPositionOfTile)                //Draws the odd and even lines on the drawing grid.
        {                                                                       //This also draws in the pieces and the terrains. Any changes to the display of a hext grid should go in here
            string line = "";
            for (var count = 1; count <= size / 2; count++)
            {
                if (count > 1 & count < size / 2)
                {
                    line += GetPieceTypeInTile(listPositionOfTile) + @"\__/";
                    listPositionOfTile += 1;
                    line += tiles[listPositionOfTile].GetTerrain();
                }
                else if (count == 1)
                {
                    line += @" \__/" + tiles[listPositionOfTile].GetTerrain();
                }
            }
            line += GetPieceTypeInTile(listPositionOfTile) + @"\__/";
            listPositionOfTile += 1;
            if (listPositionOfTile < tiles.Count())
            {
                line += tiles[listPositionOfTile].GetTerrain() + GetPieceTypeInTile(listPositionOfTile) + @"\" + Environment.NewLine;
            }
            else
            {
                line += @"\" + Environment.NewLine;                             //environment.newline is needed because we have removed the escape char
            }
            return line;
        }

        private string CreateEvenLine(bool firstEvenLine, ref int listPositionOfTile)
        {
            string line = " /" + tiles[listPositionOfTile].GetTerrain();
            for (var count = 1; count <= size / 2 - 1; count++)
            {
                line += GetPieceTypeInTile(listPositionOfTile);
                listPositionOfTile += 1;
                line += @"\__/" + tiles[listPositionOfTile].GetTerrain();
            }
            if (firstEvenLine)
            {
                line += GetPieceTypeInTile(listPositionOfTile) + @"\__" + Environment.NewLine;
            }
            else
            {
                line += GetPieceTypeInTile(listPositionOfTile) + @"\__/" + Environment.NewLine;
            }
            return line;
        }
    }

    class Player
    {
        protected int piecesInSupply, fuel, VPs, lumber;
        protected string name;

        public Player(string n, int v, int f, int l, int t)                     //Initialising a player with the default name, victory points, fuel, lumber and number of piece available to them
        {
            name = n;
            VPs = v;
            fuel = f;
            lumber = l;
            piecesInSupply = t;
        }

        public virtual string GetStateString()                                  //This displays after each turn to show the user what their current state is
        {
            return "VPs: " + VPs.ToString() + "   Pieces in supply: " + piecesInSupply.ToString() + "   Lumber: " + lumber.ToString() + "   Fuel: " + fuel.ToString();
        }

        public virtual int GetVPs()                                             //Getter for the player victory points
        {
            return VPs;
        }

        public virtual int GetFuel()                                            //Getter for the player fuel levels
        {
            return fuel;
        }

        public virtual int GetLumber()                                          //Getter for the player amount of lumber
        {
            return lumber;
        }

        public virtual string GetName()                                         //Getter for the player name
        {
            return name;
        }

        public virtual void AddToVPs(int n)                                     //Setter to add Victory Points to the player. This is a virtual method, but is not overridden in any other classes 
        {
            VPs += n;
        }

        public virtual void UpdateFuel(int n)                                   //Setter to change the fuel level for the player. This is a virtual method, but is not overridden in any other classes 
        {
            fuel += n;
        }

        public virtual void UpdateLumber(int n)                                 //Setter to change the lumber level for the player. This is a virtual method, but is not overridden in any other classes 
        {
            lumber += n;
        }

        public int GetPiecesInSupply()                                          //Getter used by the GetStateString() method to display the current player state on the screen
        {
            return piecesInSupply;
        }

        public virtual void RemoveTileFromSupply()                              //Setter to change the number of new tiles a player can create. This is a virtual method, but is not overridden in any other classes 
        {
            piecesInSupply -= 1;
        }
    }
}

