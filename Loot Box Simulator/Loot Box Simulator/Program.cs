using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using System.IO;

namespace Loot_Box_Simulator
{
    class Program
    {
        //// GLOBAL VARIABLES ////
        // PLAYER
        public static int playerX = 0, playerY = 0, playerDirection = -41; // PLAYER MOVEMENT
        public static double playerCashMultiplier = 1, playerCash = 0; // PLAYER CASH
        public static string playerEquiped = ""; // ITEM EQUIPED FOR PLAYER
        public static bool playerMovedToNewArea = false, shopKeeperClose = false, playerCaught = false, gettingKnockedOut = false, knockedOut = false;
        public static List<string> playerExtraItems = new List<string>(7); // INITIALIZE EXTRA ITEMS INVENTORY
        public static List<string> playerInventory = new List<string>(5); // INITIALIZE PLAYER INVENTORY
        public static int playerTimer = 35; // TIMER LENGTH
        // ENEMY
        public static int enemyX = 0, enemyY = 0; // ENEMY MOVEMENT
        public static int right = playerX + 1, left = playerX - 1, above = playerY + 1, below = playerY - 1; // FIND PLAYER 
        public static bool enemyActive = false; // START ENEMY
        // LOOTBOXES
        public static int lOneX, lOneY, lTwoX, lTwoY, lThreeX, lThreeY, lFourX, lFourY, lFiveX, lFiveY, lSixX, lSixY; // LOOT CORDS
        public static bool oneOpen = false, twoOpen = false, threeOpen = false, fourOpen = false, fiveOpen = false, sixOpen = false; // LOOT OPENED BEFORE
        public static bool lootLoaded = false, canMove = true, onTreasure = false, lookingAtTreasure = false, gettingTreasure = false; // LOOTBOX FUNCTIONALITY
        // SHOP
        public static bool oneBought = false, twoBought = false, threeBought = false, fourBought = false, fiveBought = false, sixBought = false, playerBoughtItem = false; // TEST IF ITEM WAS BOUGHT BEFORE
        public static string[] shopItems = { "House Key", "Basic Dungeon One Map", "Good Dungeon One Map", "Compass", "Dungeon Two Key", "Basic Dungeon Two Map" }; // ITEM ARRAY
        public static List<string> shop = new List<string>(shopItems); // INITIALIZE SHOP
        // OTHER
        public static Random random = new Random(); // RANDOM
        public static int north = -41, east = -32, south = -24, west = -16; // DIRECTION VALUES
        public static bool loadDungeonOne = true, loadDungeonTwo = false, loadVillage = false, loadHouse = false, canBarelyMove = false; // LOADING
        public static int loadTextBox; // TO LOAD TEXT
        public static string[] images = File.ReadAllLines(@"images.txt"); // LOAD IMAGES
        public static bool tryingToGetIn = false, beenInHouse = false;
        public static bool scammed = false;
        // MAIN FILES
        public static string playerFilePath = Path.GetFullPath("player.txt");
        public static string enemyFilePath = Path.GetFullPath("enemy.txt");
        public static string lootboxesFilePath = Path.GetFullPath("loot.txt");
        public static string shopFilePath = Path.GetFullPath("shop.txt");
        public static string otherFilePath = Path.GetFullPath("other.txt");
        // OTHER FILES
        public static string playerExtraFilePath = Path.GetFullPath("playerExtra.txt");
        public static string playerInFilePath = Path.GetFullPath("playerIn.txt");
        static void Main(string[] args)
        {
            Console.Title = "Treasure Hunter   (By Jeremy)";
            Console.CursorVisible = false;

            // VARIABLES
            Program game = new Program();

            // MAIN LOOP
            bool startupRun = true;
            while(startupRun)
            {
                Console.Clear();
                // DRAW MAIN MENU
                string[] startupMenu = { $"{Create.drawImages(images, 0, 4)}", "1| Play ", "2| About","3| Clear Data", "4| Exit" };
                Menus.drawMenu(startupMenu);
                // MAIN MENU FUNCTIONALITY
                string startupSelect = Convert.ToString(Console.ReadLine());
                switch (startupSelect)
                {
                    case "1": // PLAY
                        Console.Clear();
                        game.runGame();
                        break;
                    case "2": // ABOUT
                        caseAbout();
                        break;
                    case "3": // Clear Data
                        ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Are you sure you want to delete all local data? (y/n)");
                        ResetColor();
                        string deleteSelect = Convert.ToString(Console.ReadLine());
                        switch (deleteSelect)
                        {
                            case "y":
                                if (File.Exists(playerFilePath))
                                {
                                    // DELETE FILES
                                    File.Delete(playerFilePath);
                                    File.Delete(enemyFilePath);
                                    File.Delete(lootboxesFilePath);
                                    File.Delete(shopFilePath);
                                    File.Delete(otherFilePath);
                                    File.Delete(playerExtraFilePath);
                                    File.Delete(playerInFilePath);
                                    // DISPLAY
                                    ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("All data deleted.");
                                    ResetColor();
                                    Continue();
                                }
                                break;
                            case "n":
                                Console.WriteLine("Alright, don't scare me like that.");
                                Continue();
                                break;
                            default:
                                Console.WriteLine("Please input a correct value.");
                                Continue();
                                break;
                        }
                        break;
                    case "4": // EXIT
                        startupRun = false;
                        break;
                    default:
                        Console.WriteLine("Please input a correct value.");
                        Continue();
                        break;
                }
            }
        }
        public static void caseAbout()
        {
            Console.Clear();
            Console.WriteLine("ABOUT");
            ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("- In this game you will wander around dungeon in search for loot,\n  and buy upgrades to help you along the way.");
            Console.WriteLine("- Be careful though your time is limited in there, something is coming.");
            ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nDATA");
            ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("- To save your data press [I] at any point in the game and select the save option,\n  it will take it from there.");
            Console.WriteLine("- WARNING, there is no auto save feature so please do be careful with your data.");
            Console.WriteLine("- To delete all your data, just select the delete data option in the main menu and say yes.");
            ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nOTHER");
            ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("- If you are confused by the map, the bottom left corner is always (0,0).");
            Console.WriteLine("- If you are caught by the monster you will lose some money, goodluck!");
            Console.WriteLine("- Press [esc] at any point to return to the main menu, this will not reset any data,\n  just press play, again.");
            Console.WriteLine("- Leaving any dungeon will reset the timer and randomize the loot again.");
            Console.WriteLine("- Buying maps will lead to better loot\n");
            ResetColor();
            Continue();

        }
        public void runGame()
        {
            // VARIABLES
            ConsoleKey keyPress;
            do
            {
                // INPORTANT FUNCTIONS
                if (lootLoaded == false) // SPAWN INITIAL LOOT
                {
                    LootBoxes.lootBoxSpawns();
                    playerExtraItems.Add("Dungeon One Key");
                    // LOAD FILE DATA
                    if (File.Exists(playerFilePath))
                        Files.loadFiles();
                    lootLoaded = true;
                }
                Enemy.enemyFunctionality();
                Enemy.displayEnemyDirection();
                LootBoxes.lootBoxFunctionality();

                // DRAW MAIN MENU
                Create.drawTimer(images, 50, 53, 3);
                if (loadDungeonOne)
                    Create.drawMapAndView(images, 94, 102, 8, playerDirection, 160);
                else if (loadDungeonTwo)
                    Create.drawMapAndView(images, 482, 490, 8, playerDirection, 216);
                else if (loadVillage)
                    Create.drawMapAndView(images, 264, 272, 8, playerDirection, 208);
                else if (loadHouse)
                    Create.drawMapAndView(images, 708, 716, 8, playerDirection, 34);
                Create.drawDifferentTextBoxes();

                ConsoleKeyInfo keyInfo = ReadKey();
                keyPress = keyInfo.Key;
                if (canMove) // PLAYER MOVEMENT FORWARD
                {
                    if (keyPress == ConsoleKey.UpArrow)
                    {
                        if (canBarelyMove)
                        {
                            if (playerDirection == north) // MOVEMENT NORTH COLLISION
                            {
                                // GO INTO DUNGEON ONE
                                if (loadVillage && playerX == 6 && playerY == 1)
                                {
                                    foreach (string item in playerExtraItems)
                                    {
                                        if (item == "Dungeon One Key")
                                        {
                                            oneOpen = false;
                                            twoOpen = false;
                                            threeOpen = false;
                                            fourOpen = false;
                                            fiveOpen = false;
                                            sixOpen = false;
                                            loadDungeonOne = true;
                                            loadVillage = false;
                                            tryingToGetIn = false;
                                            playerX = 0;
                                            playerY = 0;
                                            enemyX = 0;
                                            enemyY = 0;
                                            playerMovedToNewArea = true;
                                            LootBoxes.lootBoxSpawns();
                                        }
                                    }
                                }
                                // EXIT DUNGEON TWO
                                if (loadDungeonTwo && playerX == 6 && playerY == 0)
                                {
                                    loadDungeonTwo = false;
                                    loadVillage = true;
                                    enemyActive = false;
                                    enemyX = 6;
                                    enemyY = 0;
                                    playerX = 0;
                                    playerY = 3;
                                    playerMovedToNewArea = true;
                                }

                                if (playerMovedToNewArea == false)
                                    playerX++;
                                playerMovedToNewArea = false;
                            }
                            else if (playerDirection == east) // MOVEMENT EAST COLLISION
                            {
                                // EXIT SHOP
                                if (shopKeeperClose == false && playerX == 3 && playerY == 0)
                                {
                                    shopKeeperClose = true;
                                    playerMovedToNewArea = true;
                                }
                                // EXIT HOUSE
                                if (loadHouse && playerX == 3 && playerY == 0)
                                {
                                    loadHouse = false;
                                    loadVillage = true;
                                    playerX = 2;
                                    playerY = 6;
                                    playerMovedToNewArea = true;
                                }

                                if (playerMovedToNewArea == false)
                                    playerY--;
                                playerMovedToNewArea = false;
                            }
                            else if (playerDirection == south) // MOVEMENT SOUTH COLLISION 
                            {
                                // EXIT DUNGEON ONE
                                if (loadDungeonOne && playerX == 0 && playerY == 0)
                                {
                                    loadDungeonOne = false;
                                    loadVillage = true;
                                    enemyActive = false;
                                    enemyX = 0;
                                    enemyY = 0;
                                    playerX = 6;
                                    playerY = 1;
                                    playerMovedToNewArea = true;
                                }
                                // GO INTO DUNGEON TWO
                                if (loadVillage && playerX == 0 && playerY == 3)
                                {
                                    foreach (string item in playerExtraItems)
                                    {
                                        if (item == "Dungeon Two Key")
                                        {
                                            oneOpen = false;
                                            twoOpen = false;
                                            threeOpen = false;
                                            fourOpen = false;
                                            fiveOpen = false;
                                            sixOpen = false;
                                            loadDungeonTwo = true;
                                            loadVillage = false;
                                            tryingToGetIn = false;
                                            playerX = 6;
                                            playerY = 0;
                                            enemyX = 6;
                                            enemyY = 0;
                                            playerMovedToNewArea = true;
                                            LootBoxes.lootBoxSpawns();
                                        }
                                        else
                                        {
                                            tryingToGetIn = true;
                                            playerMovedToNewArea = true;
                                        }
                                    }
                                }

                                if (playerMovedToNewArea == false)
                                    playerX--;
                                playerMovedToNewArea = false;
                            }
                            else if (playerDirection == west) // MOVEMENT WEST COLLISION
                            {
                                // GO INTO THE HOUSE
                                if (loadVillage && playerX == 2 && playerY == 6)
                                {
                                    foreach (string item in playerExtraItems)
                                    {
                                        if (item == "House Key")
                                        {
                                            scammed = true;
                                            tryingToGetIn = false;
                                            loadVillage = false;
                                            loadHouse = true;
                                            playerX = 3;
                                            playerY = 0;
                                            playerMovedToNewArea = true;
                                        }
                                        else
                                        {
                                            tryingToGetIn = true;
                                            playerMovedToNewArea = true;
                                        }
                                    }
                                }

                                if (playerMovedToNewArea == false)
                                    playerY++;
                                playerMovedToNewArea = false;
                            }
                            // PLAYER TIMER
                            if (loadDungeonOne || loadDungeonTwo)
                            {
                                if (playerTimer > 0)
                                {
                                    playerTimer--;
                                    if (playerTimer <= 0 && enemyActive == false)
                                    {
                                        loadTextBox = 3;
                                        enemyActive = true;
                                    }
                                }
                                // ENEMY ACTIVATION
                                if (enemyActive)
                                    Enemy.moveEnemy();
                            } else if (loadVillage)
                            {
                                playerTimer = 36;
                            }
                        }
                    }
                    else if (keyPress == ConsoleKey.LeftArrow) // TURN PLAYER LEFT
                    {
                        if (playerDirection == north)
                            playerDirection = west;
                        else if (playerDirection == east)
                            playerDirection = north;
                        else if (playerDirection == south)
                            playerDirection = east;
                        else if (playerDirection == west)
                            playerDirection = south;
                    }
                    else if (keyPress == ConsoleKey.RightArrow) // TURN PLAYER RIGHT
                    {
                        if (playerDirection == north)
                            playerDirection = east;
                        else if (playerDirection == east)
                            playerDirection = south;
                        else if (playerDirection == south)
                            playerDirection = west;
                        else if (playerDirection == west)
                            playerDirection = north;
                    }
                    else if (keyPress == ConsoleKey.E) // PRESSING E
                    {
                        casePressE();
                    }
                    else if (keyPress == ConsoleKey.I) // PRESSING I
                    {
                        casePressI();
                    }
                }
                Console.Clear();

            } while (keyPress != ConsoleKey.Escape);              
        }
        public static void casePressE()
        {
            if (scammed == false)
            {
                if (shopKeeperClose)
                {
                    bool shopRun = true;
                    while (shopRun)
                    {
                        Console.Clear();
                        // DRAW SHOP MENU
                        Create.drawShopMenu(images, 9, 14, 0, false);
                        string[] shopMenu = { "1| Buy", "2| Sell", "3| Exit" };
                        Menus.drawMenu(shopMenu);
                        // SHOP MENU FUNCTIONALITY
                        string shopMenuSelect = Convert.ToString(Console.ReadLine());
                        switch (shopMenuSelect)
                        {
                            case "1":
                                bool buyRun = true;
                                while (buyRun)
                                {
                                    Console.Clear();
                                    // DRAW SHOP MENU AGAIN
                                    Create.drawShopMenu(images, 9, 14, 0, true);
                                    string[] buyMenu = { "$150K", "$100", "$250", "$10", "$2K", "$10K" };
                                    Menus.drawShopMenu(buyMenu);
                                    // BUY MENU FUNCTIONALITY
                                    string buyMenuSelect = Convert.ToString(Console.ReadLine());
                                    switch (buyMenuSelect)
                                    {
                                        case "1":
                                            if (oneBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(150000, "House Key");
                                                if (playerBoughtItem)
                                                    oneBought = true;
                                            }
                                            break;
                                        case "2":
                                            if (twoBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(100, "Basic Dungeon One Map");
                                                if (playerBoughtItem)
                                                    twoBought = true;
                                            }
                                            break;
                                        case "3":
                                            if (threeBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(250, "Good Dungeon One Map");
                                                if (playerBoughtItem)
                                                    threeBought = true;
                                            }
                                            break;
                                        case "4":
                                            if (fourBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(10, "Compass");
                                                if (playerBoughtItem)
                                                    fourBought = true;
                                            }
                                            break;
                                        case "5":
                                            if (fiveBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(2000, "Dungeon Two Key");
                                                if (playerBoughtItem)
                                                    fiveBought = true;
                                            }
                                            break;
                                        case "6":
                                            if (sixBought == true)
                                                Console.WriteLine("");
                                            else
                                            {
                                                buy(10000, "Basic Dungeon Two Map");
                                                if (playerBoughtItem)
                                                    sixBought = true;
                                            }
                                            break;
                                        case "7":
                                            buyRun = false;
                                            break;
                                        default:
                                            Console.WriteLine("Please input a correct value.");
                                            Continue();
                                            break;
                                    }
                                    playerBoughtItem = false;
                                }
                                break;
                            case "2":
                                bool sellRun = true;
                                while (sellRun)
                                {
                                    Console.Clear();
                                    // DRAW SHOP MENU AGAIN
                                    Create.drawShopMenu(images, 9, 14, 0, false);
                                    int selectvalue = 1;
                                    foreach (string item in playerInventory)
                                    {
                                        Console.Write($"{selectvalue}| {item}");
                                        if (item == "Magnet")
                                            Console.Write(" +$100\n");
                                        else if (item == "Amulet")
                                            Console.Write(" +$500\n");
                                        if (item == "Magic Ring")
                                            Console.Write(" +$2500\n");
                                        if (item == "Enchanted Necklace")
                                            Console.Write(" +$15000\n");
                                        selectvalue++;
                                    }
                                    Console.WriteLine($"{selectvalue}| Back");
                                    Console.Write("Which to sell? Input| ");
                                    // SELL MENU FUNCTIONALITY
                                    string sellMenuSelect = Convert.ToString(Console.ReadLine());
                                    switch (sellMenuSelect)
                                    {
                                        case "1":
                                            if (selectvalue == 1)
                                                sellRun = false;
                                            else
                                                removeFromInv(0);
                                            break;
                                        case "2":
                                            if (selectvalue == 2)
                                                sellRun = false;
                                            else
                                                removeFromInv(1);
                                            break;
                                        case "3":
                                            if (selectvalue == 3)
                                                sellRun = false;
                                            else
                                                removeFromInv(2);
                                            break;
                                        case "4":
                                            if (selectvalue == 4)
                                                sellRun = false;
                                            else
                                                removeFromInv(3);
                                            break;
                                        case "5":
                                            if (selectvalue == 5)
                                                sellRun = false;
                                            else
                                                removeFromInv(4);
                                            break;
                                        case "6":
                                            if (selectvalue == 6)
                                                sellRun = false;
                                            else
                                                removeFromInv(5);
                                            break;
                                        default:
                                            Console.WriteLine("Please input a correct value.");
                                            Continue();
                                            break;
                                    }
                                }

                                break;
                            case "3":
                                shopRun = false;
                                break;
                            default:
                                Console.WriteLine("Please input a correct value.");
                                Continue();
                                break;
                        }
                    }
                }
            }
        }
        public static void removeFromInv(int removePos)
        {
            // SELL IF removePos HAS NAME
            if (playerInventory[removePos] == "Magnet")
                playerCash += 100;
            else if (playerInventory[removePos] == "Amulet")
                playerCash += 500;
            else if (playerInventory[removePos] == "Magic Ring")
                playerCash += 2500;
            else if (playerInventory[removePos] == "Enchanted Necklace")
                playerCash += 15000;
            playerInventory.RemoveAt(removePos);
        }
        public static void casePressI()
        {
            bool inventoryRun = true;
            while (inventoryRun)
            {
                Console.Clear();
                // DRAW INVENTORY MENU
                Create.drawInventory(images, 4, 9);
                string[] inventoryMenu = { "1| Equip", "2| Trash", "3| Extras", "4| Save", "5| Exit" };
                Menus.drawMenu(inventoryMenu);
                // INVENTORY MENU FUNCTIONALITY
                string inventoryMenuSelect = Convert.ToString(Console.ReadLine());
                switch (inventoryMenuSelect)
                {
                    case "1":
                        bool equipRun = true;
                        while (equipRun)
                        {
                            Console.Clear();
                            // DRAW INVENTORY MENU AGAIN
                            Create.drawInventory(images, 4, 9);
                            Console.WriteLine("");
                            int selectvalue = 1;
                            foreach (string item in playerInventory)
                            {
                                Console.WriteLine($"{selectvalue}| {item}");
                                selectvalue++;
                            }
                            Console.WriteLine($"{selectvalue}| Back");
                            Console.Write("Which to equip? Input| ");
                            // EQUIP MENU FUNCTIONALITY
                            string equipMenuSelect = Convert.ToString(Console.ReadLine());
                            switch (equipMenuSelect)
                            {
                                case "1":
                                    if (selectvalue == 1)
                                        equipRun = false;
                                    else
                                        equipFunctionality(0);
                                    break;
                                case "2":
                                    if (selectvalue == 2)
                                        equipRun = false;
                                    else
                                        equipFunctionality(1);
                                    break;
                                case "3":
                                    if (selectvalue == 3)
                                        equipRun = false;
                                    else
                                        equipFunctionality(2);
                                    break;
                                case "4":
                                    if (selectvalue == 4)
                                        equipRun = false;
                                    else
                                        equipFunctionality(3);
                                    break;
                                case "5":
                                    if (selectvalue == 5)
                                        equipRun = false;
                                    else
                                        equipFunctionality(4);
                                    break;
                                case "6":
                                    if (selectvalue == 6)
                                        equipRun = false;
                                    else
                                        equipFunctionality(5);
                                    break;
                                default:
                                    Console.WriteLine("Please input a correct value.");
                                    Continue();
                                    break;
                            }
                        }
                        break;
                    case "2":
                        bool trashRun = true;
                        while (trashRun)
                        {
                            Console.Clear();
                            // DRAW INVENTORY MENU AGAIN
                            Create.drawInventory(images, 4, 9);
                            Console.WriteLine("");
                            int selectedvalue = 1;
                            foreach (string item in playerInventory)
                            {
                                Console.WriteLine($"{selectedvalue}| {item}");
                                selectedvalue++;
                            }
                            Console.WriteLine($"{selectedvalue}| Back");
                            Console.Write("Which to remove? Input| ");
                            // REMOVE MENU FUNCTIONALITY
                            string trashMenuSelect = Convert.ToString(Console.ReadLine());
                            switch (trashMenuSelect)
                            {
                                case "1":
                                    if (selectedvalue == 1)
                                        trashRun = false;
                                    else
                                        playerInventory.RemoveAt(0);
                                    break;
                                case "2":
                                    if (selectedvalue == 2)
                                        trashRun = false;
                                    else
                                        if (playerInventory.Count > 2)
                                        playerInventory.RemoveAt(1);
                                    break;
                                case "3":
                                    if (selectedvalue == 3)
                                        trashRun = false;
                                    else
                                        if (playerInventory.Count > 3)
                                        playerInventory.RemoveAt(2);
                                    break;
                                case "4":
                                    if (selectedvalue == 4)
                                        trashRun = false;
                                    else
                                        if (playerInventory.Count > 4)
                                        playerInventory.RemoveAt(3);
                                    break;
                                case "5":
                                    if (selectedvalue == 5)
                                        trashRun = false;
                                    else
                                        if (playerInventory.Count > 5)
                                        playerInventory.RemoveAt(4);
                                    break;
                                case "6":
                                    if (selectedvalue == 6)
                                        trashRun = false;
                                    break;
                                default:
                                    Console.WriteLine("Please input a correct value.");
                                    Continue();
                                    break;
                            }
                        }
                        break;
                    case "3":
                        bool extraRun = true;
                        while (extraRun)
                        {
                            Console.Clear();
                            // DRAW INVENTORY MENU AGAIN
                            Create.drawInventory(images, 4, 9);
                            foreach (string item in playerExtraItems)
                            {
                                Console.Write(item);
                                ForegroundColor = ConsoleColor.DarkGray;
                                if (item == "Dungeon One Key")
                                    Console.Write(" Allows access to dungeon one.");
                                else if (item == "Dungeon Two Key")
                                    Console.Write(" Allows access to dungeon two.");
                                else if (item == "House Key")
                                    Console.Write(" Allows access to your new home.");
                                else if (item == "Compass")
                                    Console.Write(" Shows your direction in the dungeon.");
                                else if (item == "Basic Dungeon One Map")
                                    Console.Write(" Allows for basic loot (1, 5, 10).");
                                else if (item == "Good Dungeon One Map")
                                    Console.Write(" Allows for better loot (25, 50, 100).");
                                else if (item == "Basic Dungeon Two Map")
                                    Console.Write(" Allows for basic loot (2500, 5000, 10000).");
                                ResetColor();
                                Console.WriteLine("");
                            }
                            Console.WriteLine("");
                            Console.WriteLine("1| Back");
                            Console.Write("Input| ");
                            // EXTRA MENU FUNCTIONALITY
                            string extraMenuSelect = Convert.ToString(Console.ReadLine());
                            switch (extraMenuSelect)
                            {
                                case "1":
                                    extraRun = false;
                                    break;
                                default:
                                    Console.WriteLine("Please input a correct value.");
                                    Continue();
                                    break;
                            }
                        }
                        break;
                    case "4":
                        // SAVE DATA TO FILES
                        Files.loadToFiles();
                        ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nData saved.");
                        ResetColor();
                        ReadKey(true);
                        break;
                    case "5":
                        inventoryRun = false;
                        break;
                    default:
                        Console.WriteLine("Please input a correct value.");
                        Continue();
                        break;
                }
            }
        }
        public static void buy(int cost, string item)
        {
            // TEST IF YOU CAN BUY ITEM
            if (playerCash >= cost)
            {
                playerCash -= cost;
                playerExtraItems.Add(item);
                playerBoughtItem = true;
            }
        }
        public static void equipFunctionality(int position)
        {
            // TEST IF PLAYER HAS ITEMS
            if (playerInventory.Count >= 1)
            {
                if (position <= playerInventory.Count)
                {
                    // MONEY MULTIPLIER
                    if (playerInventory[position] == "Magnet")
                        playerCashMultiplier = 1.5;
                    else if (playerInventory[position] == "Amulet")
                        playerCashMultiplier = 2.0;
                    else if (playerInventory[position] == "Magic Ring")
                        playerCashMultiplier = 3.0;
                    else if (playerInventory[position] == "Enchanted Necklace")
                        playerCashMultiplier = 4.0;
                    else if (playerInventory[position] == "Shop Keepers Hat")
                        playerCashMultiplier = 8.0;
                    // EQUIP AND SWITCH
                    string insert = playerEquiped;
                    playerEquiped = playerInventory[position];
                    playerInventory.RemoveAt(position);
                    if (insert != "")
                        playerInventory.Insert(position, insert);
                }
            }
        }
        static void Continue()
        {
            Console.WriteLine("Press any key to continue.");
            ReadKey(true);
            Console.Clear();
        }

    }
    class LootBoxes
    {
        private readonly int smallDrop, mediumDrop, largeDrop; // ITEM DROPS
        private readonly string itemDrop; // SPECIAL ITEM DROP
        private readonly int smallChance, mediumChance, largeChance; // ITEM DROP CHANCES
        public LootBoxes(int newSmallDrop, int newSmallChance, int newMediumDrop, int newMediumChance,
                         int newLargeDrop, int newLargeChance, string newItemDrop)
        {
            this.smallDrop = newSmallDrop;
            this.smallChance = newSmallChance;
            this.mediumDrop = newMediumDrop;
            this.mediumChance = newMediumChance;
            this.largeDrop = newLargeDrop;
            this.largeChance = newLargeChance;
            this.itemDrop = newItemDrop;
        }
        public void openLootBox()
        {
            // DETERMINE LOOT
            int randNum = Program.random.Next(0, 101);
            if (randNum <= this.smallChance)
            {
                Program.playerCash += this.smallDrop * Program.playerCashMultiplier;
            } else if (randNum <= this.mediumChance)
            {
                Program.playerCash += this.mediumDrop * Program.playerCashMultiplier;
            } else if (randNum <= this.largeChance)
            {
                Program.playerCash += this.largeDrop * Program.playerCashMultiplier;
            } else
            {
                Program.playerInventory.Add($"{this.itemDrop}");
            }
        }
        static int lootBoxSpawnRuns = 1;
        public static void lootBoxSpawns()
        {
            // INITALIZE VARIABLES
            int randX = Program.random.Next(0, 6);
            int randY = Program.random.Next(0, 6);

            // DETECT CORDINATES TO NOT PLACE LOOT ON
            if (Program.loadDungeonOne)
                if (randX >= 4 && randY >= 4 || randX >= 3 && randX <= 5 && randY == 2 || randX == 3 && randY == 1 ||
                    randX == 5 && randY == 1 || randX == 1 && randY == 3 || randX >= 1 && randX <= 2 && randY == 5
                    || randX == 0 && randY == 2 || randX <= 1 && randY == 1 || randX == Program.lOneX && randY ==
                    Program.lOneY || randX == Program.lTwoX && randY == Program.lTwoY || randX == Program.lThreeX &&
                    randY == Program.lThreeY || randX == Program.lFourX && randY == Program.lFourY || randX ==
                    Program.lFiveX && randY == Program.lFiveY || randX == 0 && randY == 0 || randX == 1 && randY == 0
                    || randX == 6 && randY == 0 || randX == 6 && randY == 3 || randX == 2 && randY == 4 ||
                    randX == 0 && randY == 6 || randX == 3 && randY == 6)
                    // REDO FUNCTION IF ON CORDINATES
                    lootBoxSpawns();
                else
                    assignLoot(randX, randY);
            else if (Program.loadDungeonTwo)
                if (randX >= 4 && randY >= 4 || randX == 0 && randY == 6 || randX == 3 && randY == 6 || randX >= 1 &&
                    randX <= 2 && randY == 5 || randX >= 0 && randX <= 1 && randY == 3 || randX == 2 && randY == 3 ||
                    randX == 6 && randY >= 2 && randY <= 3 || randX == 0 && randY == 2 || randX == 4 && randY == 2 ||
                    randX == 5 && randY == 2 || randX == 6 && randY >= 0 && randY <= 1 || randX == 5 && randY == 0 ||
                    randX == 1 && randY >= 0 && randY <= 1 || randX == Program.lOneX && randY == Program.lOneY ||
                    randX == Program.lTwoX && randY == Program.lTwoY || randX == Program.lThreeX && randY == Program.lThreeY
                    || randX == Program.lFourX && randY == Program.lFourY)
                    // REDO FUNCTION IF ON CORDINATES
                    lootBoxSpawns();
                else
                    assignLoot(randX, randY);
        }
        public static void assignLoot(int randX, int randY)
        {
            // ASIGN VALUES TO GLOBAL VARIABLES
            if (lootBoxSpawnRuns == 1)
            {
                Program.lOneX = randX;
                Program.lOneY = randY;
            }
            else if (lootBoxSpawnRuns == 2)
            {
                Program.lTwoX = randX;
                Program.lTwoY = randY;
            }
            else if (lootBoxSpawnRuns == 3)
            {
                Program.lThreeX = randX;
                Program.lThreeY = randY;
            }
            else if (lootBoxSpawnRuns == 4)
            {
                Program.lFourX = randX;
                Program.lFourY = randY;
            }
            else if (lootBoxSpawnRuns == 5)
            {
                Program.lFiveX = randX;
                Program.lFiveY = randY;
            }
            else if (lootBoxSpawnRuns == 6)
            {
                Program.lSixX = randX;
                Program.lSixY = randY;
            }
            // LOOP THE FUNCTION IF LESS THAN SIX RUNS
            if (lootBoxSpawnRuns < 6)
            {
                lootBoxSpawnRuns++;
                lootBoxSpawns();
            }
            lootBoxSpawnRuns = 1;
        }
        public static void lootBoxFunctionality()
        {
            if (Program.loadDungeonOne || Program.loadDungeonTwo)
            {
                // INITIALZE VARIABLES
                LootBoxes woodenBox = new LootBoxes(1, 50, 5, 80, 10, 95, "Magnet");
                LootBoxes silverBox = new LootBoxes(25, 50, 50, 75, 100, 95, "Amulet");
                LootBoxes goldenBox = new LootBoxes(250, 45, 500, 70, 1000, 90, "Magic Ring");
                LootBoxes platinumBox = new LootBoxes(2500, 40, 5000, 65, 10000, 90, "Enchanted Necklace");
                // TEST IF PLAYER IS ON TREASURE
                if (Program.playerX == Program.lOneX && Program.playerY == Program.lOneY && Program.oneOpen == false ||
                    Program.playerX == Program.lTwoX && Program.playerY == Program.lTwoY && Program.twoOpen == false ||
                    Program.playerX == Program.lThreeX && Program.playerY == Program.lThreeY && Program.threeOpen == false ||
                    Program.playerX == Program.lFourX && Program.playerY == Program.lFourY && Program.fourOpen == false ||
                    Program.playerX == Program.lFiveX && Program.playerY == Program.lFiveY && Program.fiveOpen == false ||
                    Program.playerX == Program.lSixX && Program.playerY == Program.lSixY && Program.sixOpen == false)
                    Program.onTreasure = true;
                else
                    Program.onTreasure = false;
                if (Program.lookingAtTreasure)
                {
                    // TEST IF TREASURE HAS BEEN OPENED
                    if (Program.playerX == Program.lOneX && Program.playerY == Program.lOneY && Program.oneOpen == false)
                        Program.oneOpen = true;
                    else if (Program.playerX == Program.lTwoX && Program.playerY == Program.lTwoY && Program.twoOpen == false)
                        Program.twoOpen = true;
                    else if (Program.playerX == Program.lThreeX && Program.playerY == Program.lThreeY && Program.threeOpen == false)
                        Program.threeOpen = true;
                    else if (Program.playerX == Program.lFourX && Program.playerY == Program.lFourY && Program.fourOpen == false)
                        Program.fourOpen = true;
                    else if (Program.playerX == Program.lFiveX && Program.playerY == Program.lFiveY && Program.fiveOpen == false)
                        Program.fiveOpen = true;
                    else if (Program.playerX == Program.lSixX && Program.playerY == Program.lSixY && Program.sixOpen == false)
                        Program.sixOpen = true;
                    // TEST IT FIRST TIME LOOKING AT TREASURE
                    if (Program.gettingTreasure == false)
                    {
                        Program.gettingTreasure = true;
                    }
                    else
                    {
                        if (Program.loadDungeonOne && Program.threeBought == false)
                            woodenBox.openLootBox();
                        else if (Program.loadDungeonOne && Program.threeBought)
                            silverBox.openLootBox();
                        else if (Program.loadDungeonTwo && Program.sixBought == false)
                            goldenBox.openLootBox();
                        else if (Program.loadDungeonTwo && Program.sixBought)
                            platinumBox.openLootBox();

                        Program.lookingAtTreasure = false;
                        Program.gettingTreasure = false;
                        Program.canMove = true;
                    }
                }
            }
        }
    }
    class Menus
    {
        public static void drawMenu(string[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine(array[i]);
            }
            Console.Write("Input| ");
        }
        public static void drawShopMenu(string[] array)
        {
            int selected = 1;
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{selected}| ");
                if (selected == 1 && Program.oneBought == false)
                    if (Program.playerCash >= 150000)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else if (selected == 2 && Program.twoBought == false)
                    if (Program.playerCash >= 100)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else if (selected == 3 && Program.threeBought == false)
                    if (Program.playerCash >= 250)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else if (selected == 4 && Program.fourBought == false)
                    if (Program.playerCash >= 10)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else if (selected == 5 && Program.fiveBought == false)
                    if (Program.playerCash >= 2000)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else if (selected == 6 && Program.sixBought == false)
                    if (Program.playerCash >= 10000)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                else
                    ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(array[i]);
                ResetColor();
                selected++;
                Console.WriteLine("");
            }
            Console.WriteLine($"{selected}| Back");
            Console.Write("Input| ");
        }
    }
    class Enemy
    {
        public static void moveEnemy()
        {
            // RANDOM MOVEMENT VARIABLES
            int xMovement = Program.random.Next(1, 3);
            int yMovement = Program.random.Next(1, 3);
            // ENEMY MOVEMENT
            if (yMovement == 1 && Program.enemyY > 0) // EAST COLLISION
                    Program.enemyY--;
            else if (yMovement == 2 && Program.enemyY < 6) // WEST COLLISION
                    Program.enemyY++;
            if (xMovement == 1 && Program.enemyX > 0) // SOUTH COLLISION
                    Program.enemyX--;
            else if (xMovement == 2 && Program.enemyX < 6) // NORTH COLLISION
                    Program.enemyX++;
        }
        public static void displayEnemyDirection()
        {
            // DETERMINE DIRECTION AND ASSIGN loadTextBox
            if (Program.enemyActive)
            {
                if (Program.playerX + 1 == Program.enemyX && Program.playerY == Program.enemyY) // RIGHT OF THE PLAYER
                {
                    determineDirection(4, 6, 5, 7);
                }
                else if (Program.playerX - 1 == Program.enemyX && Program.playerY == Program.enemyY) // LEFT OF THE PLAYER
                {
                    determineDirection(5, 7, 4, 6);
                }
                else if (Program.playerY + 1 == Program.enemyY && Program.playerX == Program.enemyX) // ABOVE THE PLAYER
                {
                    determineDirection(6, 5, 7, 4);
                }
                else if (Program.playerY - 1 == Program.enemyY && Program.playerX == Program.enemyX) // BELOW THE PLAYER
                {
                    determineDirection(7, 4, 6, 5);
                }
            }
        }
        private static void determineDirection(int loadNorth, int loadEast, int loadSouth, int loadWest)
        {
            // ASSIGN loadTextBox BASED OFF VALUES
            if (Program.playerDirection == Program.north)
                Program.loadTextBox = loadNorth;
            else if (Program.playerDirection == Program.east)
                Program.loadTextBox = loadEast;
            else if (Program.playerDirection == Program.south)
                Program.loadTextBox = loadSouth;
            else
                Program.loadTextBox = loadWest;
        }
        public static int playerIsCaught()
        {
            if (Program.playerX == Program.enemyX && Program.playerY == Program.enemyY && Program.loadVillage != true && Program.enemyActive)
            {
                Program.loadTextBox = 14;
                Program.playerCaught = true;
                Program.canMove = false;
                if (Program.gettingKnockedOut )
                {
                    Program.loadTextBox = 12;
                    if (Program.knockedOut)
                        Program.loadTextBox = 13;
                    return 8;
                }
                if (Program.loadDungeonOne)
                    return 144;
                else if (Program.loadDungeonTwo)
                    return 208;
            }
            return 8;
        }
        public static void enemyFunctionality()
        {
            if (Program.enemyActive && Program.playerCaught)
            {
                if (Program.gettingKnockedOut == false)
                    Program.gettingKnockedOut = true;
                else if (Program.knockedOut == false && Program.gettingKnockedOut)
                    Program.knockedOut = true;
                else if (Program.gettingKnockedOut && Program.knockedOut)
                {
                    // IF KNOCKED OUT BY GHOST RESET VARIABLES
                    Program.canMove = true;
                    Program.knockedOut = false;
                    Program.gettingKnockedOut = false;
                    Program.playerCaught = false;
                    Program.loadVillage = true;
                    Program.loadDungeonOne = false;
                    Program.loadDungeonTwo = false;
                    Program.enemyActive = false;
                    Program.enemyX = 0;
                    Program.enemyY = 0;
                    Program.playerX = 3;
                    Program.playerY = 0;
                    Program.playerDirection = Program.east;
                    Program.playerTimer = 35;
                    if (Program.playerCash > 0)
                        Program.playerCash -= Program.playerCash * .25;
                }
            }
        }
    }
    class Create
    {
        public static void drawDifferentTextBoxes()
        {
            if (Program.loadTextBox == 0) // EMPTY 
            {
                Create.drawTextBox(Program.images, 14, 16, 248);
            }
            else if (Program.loadTextBox == 1) // SOMETHING MIGHT BE VALUABLE THERE 
            {
                Create.drawTextBox(Program.images, 28, 30, 234);
            }
            else if (Program.loadTextBox == 2) // AH TREASURE 
            {
                Create.drawTextBox(Program.images, 30, 32, 232);
            }
            else if (Program.loadTextBox == 3) // SOMETHING BAD IS HERE NOW 
            {
                Create.drawTextBox(Program.images, 32, 34, 230);
            }
            else if (Program.loadTextBox == 4) // INFRONT OF YOU AHH 
            {
                Create.drawTextBox(Program.images, 38, 40, 224);
            }
            else if (Program.loadTextBox == 5) // BEHINED YOU AHH 
            {
                Create.drawTextBox(Program.images, 40, 42, 222);
            }
            else if (Program.loadTextBox == 6) // TO YOUR LEFT AHH 
            {
                Create.drawTextBox(Program.images, 34, 36, 228);
            }
            else if (Program.loadTextBox == 7) // TO YOUR RIGHT AHH 
            {
                Create.drawTextBox(Program.images, 36, 38, 226);
            }
            else if (Program.loadTextBox == 8) // TIME TO LEAVE? 
            {
                Create.drawTextBox(Program.images, 16, 18, 246);
            }
            else if (Program.loadTextBox == 9) // TIME TO HEAD INTO D1? 
            {
                Create.drawTextBox(Program.images, 22, 24, 240);
            }
            else if (Program.loadTextBox == 10) // GO TO SHOP? 
            {
                Create.drawTextBox(Program.images, 18, 20, 244);
            }
            else if (Program.loadTextBox == 11) // WATCHA WANNA BUY? 
            {
                Create.drawTextBox(Program.images, 26, 28, 236);
            }
            else if (Program.loadTextBox == 12) // YOU WERE BEATEN 
            {
                Create.drawTextBox(Program.images, 44, 46, 218);
            }
            else if (Program.loadTextBox == 13) // SOMEONE FOUND YOU 
            {
                Create.drawTextBox(Program.images, 48, 50, 214);
            }
            else if (Program.loadTextBox == 14) // AHH YOU WERE CAUGHT 
            {
                Create.drawTextBox(Program.images, 42, 44, 220);
            }
            else if (Program.loadTextBox == 15) // ENTER HOUSE?
            {
                Create.drawTextBox(Program.images, 20, 22, 242);
            }
            else if (Program.loadTextBox == 16) // TIME TO HEAD INTO D2?
            {
                Create.drawTextBox(Program.images, 24, 26, 238);
            }
            else if (Program.loadTextBox == 17) // NO ACCESS
            {
                Create.drawTextBox(Program.images, 46, 48, 216);
            }
            else if (Program.loadTextBox == 18) // LOOKS LIKE YOU GOT SCAMMED
            {
                Create.drawTextBox(Program.images, 756, 758, 9);
            }
            else if (Program.loadTextBox == 19) // AND THEY'RE GONE
            {
                Create.drawTextBox(Program.images, 758, 760, -278);
            }
            else if (Program.loadTextBox == 20) // EXIT THIS DISAPPOINTMENT
            {
                Create.drawTextBox(Program.images, 760, 762, 5);
            }
            Program.loadTextBox = 0;
        }
        public static int drawImageBasedOffDirection(int northI, int eastI, int southI, int westI, bool northMove, bool eastMove,
                                             bool southMove, bool westMove)
        {
            // DETERMINE WHICH DRAWING CORDINATES TO USE 
            Program.canBarelyMove = false;
            if (Program.playerDirection == Program.north)
                if (Program.onTreasure && northI == 8)
                    return drawImagesbasedOffTreasure();
                else if (Program.enemyActive && Program.playerX == Program.enemyX && Program.playerY == Program.enemyY && Program.loadVillage == false)
                {
                    return Enemy.playerIsCaught();
                }
                else
                {
                    if (northMove)
                        Program.canBarelyMove = true;
                    return northI;
                }
            else if (Program.playerDirection == Program.south)
                if (Program.onTreasure && southI == 8)
                    return drawImagesbasedOffTreasure();
                else if (Program.enemyActive && Program.playerX == Program.enemyX && Program.playerY == Program.enemyY && Program.loadVillage == false)
                {
                    return Enemy.playerIsCaught();
                }
                else
                {
                    if (southMove)
                        Program.canBarelyMove = true;
                    return southI;
                }
            else if (Program.playerDirection == Program.east)
                if (Program.onTreasure && eastI == 8)
                    return drawImagesbasedOffTreasure();
                else if (Program.enemyActive && Program.playerX == Program.enemyX && Program.playerY == Program.enemyY && Program.loadVillage == false)
                {
                    return Enemy.playerIsCaught();
                }
                else
                {
                    if (eastMove)
                        Program.canBarelyMove = true;
                    return eastI;
                }
            else if (Program.playerDirection == Program.west)
                if (Program.onTreasure && westI == 8)
                    return drawImagesbasedOffTreasure();
                else if (Program.enemyActive && Program.playerX == Program.enemyX && Program.playerY == Program.enemyY && Program.loadVillage == false)
                {
                    return Enemy.playerIsCaught();
                }
                else
                {
                    if (westMove)
                        Program.canBarelyMove = true;
                    return westI;
                }
            return 0;
        }
        public static int drawImagesbasedOffTreasure()
        {
            Program.canMove = false;
            Program.lookingAtTreasure = true;
            Program.loadTextBox = 1;
            if (Program.gettingTreasure)
            {
                Program.loadTextBox = 2;
                if (Program.loadDungeonOne)
                    return 136;
                else if (Program.loadDungeonTwo)
                    return 200;
            }
            if (Program.loadDungeonOne)
                return 128;
            else if (Program.loadDungeonTwo)
                return 192;
            return 8;
        }
        public static void drawInventory(string[] array, int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                Console.Write(array[i]);
                if (i == 5) // DRAW PLAYER CASH
                {
                    ForegroundColor = ConsoleColor.DarkYellow;
                    double cash = Program.playerCash;
                    if (Program.playerCash < 1000)
                        Console.Write($" ${cash}");
                    else if (Program.playerCash >= 1000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}K (x{Program.playerCashMultiplier})");
                    else if (Program.playerCash >= 1000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}M (x{Program.playerCashMultiplier})");
                    else if (Program.playerCash >= 1000000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}B (x{Program.playerCashMultiplier})");
                    else if (Program.playerCash >= 1000000000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)} Why are you doing this. . . (x{Program.playerCashMultiplier})");
                    ResetColor();
                }
                else if (i == 6) // DRAW PLAYER ITEMS AND LIMIT
                {
                    if (Program.playerInventory.Count < Program.playerInventory.Capacity)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {Program.playerInventory.Count}/{Program.playerInventory.Capacity}");
                    ResetColor();
                }
                Console.WriteLine("");
            }
            // DRAW PLAYER EQUPIED AND INVENTORY
            Console.Write("Equiped: ");
            Console.Write($"{Program.playerEquiped}\n");
            Console.WriteLine("");
            foreach (string item in Program.playerInventory)
            {
                Console.Write(item);
                ForegroundColor = ConsoleColor.DarkGray;
                if (item == "Magnet")
                    Console.Write(" x1.5 Cash");
                else if (item == "Amulet")
                    Console.Write(" x2.0 Cash");
                else if (item == "Magic Ring")
                    Console.Write(" x3.0 Cash");
                else if (item == "Enchanted Necklace")
                    Console.Write(" x4.0 Cash");
                else if (item == "Shop Keepers Hat")
                    Console.Write(" x8.0 Cash");
                ResetColor();
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        public static string drawImages(string[] array, int start, int finish)
        {
            for (int i = start; i < finish; i++)
            {
                Console.WriteLine(array[i]);
            }
            return " ";
        }
        public static string drawMapAndView(string[] array, int start, int finish, int between, int betweenTwo, int betweenThree)
        {
            // DETERMINE DRAWING FOR PLAYER POV
            for (int i = start; i < finish; i++)
            {
                if (Program.loadDungeonOne)
                {
                    if (Program.playerX == 0 && Program.playerY == 0) // 0,0
                    {
                        between = drawImageBasedOffDirection(32, 8, 120, 8, true, false, true, false);
                        if (between == 120)
                            Program.loadTextBox = 8;
                    }
                    else if (Program.playerX == 1 && Program.playerY == 0) // 1,0
                        between = drawImageBasedOffDirection(80, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 0) // 2,0
                        between = drawImageBasedOffDirection(64, 8, 56, 96, true, false, true, true);
                    else if (Program.playerX == 3 && Program.playerY == 0) // 3,0
                        between = drawImageBasedOffDirection(80, 8, 72, 8, true, false, true, false);
                    else if (Program.playerX == 4 && Program.playerY == 0) // 4,0
                        between = drawImageBasedOffDirection(64, 8, 56, 96, true, false, true, true);
                    else if (Program.playerX == 5 && Program.playerY == 0) // 5,0
                        between = drawImageBasedOffDirection(48, 8, 72, 8, true, false, true, false);
                    else if (Program.playerX == 6 && Program.playerY == 0) // 6,0
                        between = drawImageBasedOffDirection(24, 16, 56, 64, false, false, true, true);
                    else if (Program.playerX == 2 && Program.playerY == 1) // 2,1
                        between = drawImageBasedOffDirection(8, 112, 8, 80, false, true, false, true);
                    else if (Program.playerX == 4 && Program.playerY == 1) // 4,1
                        between = drawImageBasedOffDirection(8, 112, 8, 88, false, true, false, false);
                    else if (Program.playerX == 6 && Program.playerY == 1) // 6,1
                        between = drawImageBasedOffDirection(8, 40, 8, 32, false, true, false, true);
                    else if (Program.playerX == 1 && Program.playerY == 2) // 1,2
                        between = drawImageBasedOffDirection(112, 8, 88, 8, true, false, false, false);
                    else if (Program.playerX == 2 && Program.playerY == 2) // 2,2
                        between = drawImageBasedOffDirection(8, 56, 96, 104, false, true, true, true);
                    else if (Program.playerX == 6 && Program.playerY == 2) // 6,2
                        between = drawImageBasedOffDirection(8, 32, 8, 48, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 3) // 2,3
                        between = drawImageBasedOffDirection(96, 104, 8, 152, true, true, false, true);
                    else if (Program.playerX == 0 && Program.playerY == 3) //0,3
                        between = drawImageBasedOffDirection(8, 88, 8, 72, false, false, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 3) //3,3
                        between = drawImageBasedOffDirection(32, 8, 112, 8, true, false, true, false);
                    else if (Program.playerX == 4 && Program.playerY == 3) // 4,3
                        between = drawImageBasedOffDirection(32, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 5 && Program.playerY == 3) // 5,3   
                        between = drawImageBasedOffDirection(40, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 6 && Program.playerY == 3) // 6,3
                        between = drawImageBasedOffDirection(16, 56, 64, 24, false, true, true, false);
                    else if (Program.playerX == 0 && Program.playerY == 4) // 0,4
                        between = drawImageBasedOffDirection(96, 64, 8, 56, true, true, false, true);
                    else if (Program.playerX == 1 && Program.playerY == 4) // 1,4
                        between = drawImageBasedOffDirection(40, 8, 112, 8, true, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 4) // 2,4
                        between = drawImageBasedOffDirection(16, 80, 64, 24, false, true, true, false);
                    else if (Program.playerX == 3 && Program.playerY == 4) // 3,4
                        between = drawImageBasedOffDirection(8, 88, 8, 32, false, false, false, true);
                    else if (Program.playerX == 0 && Program.playerY == 5) // 0,5
                        between = drawImageBasedOffDirection(8, 80, 8, 40, false, true, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 5) // 3,5
                        between = drawImageBasedOffDirection(8, 32, 8, 48, false, true, false, true);
                    else if (Program.playerX == 0 && Program.playerY == 6) // 0,6
                        between = drawImageBasedOffDirection(56, 64, 24, 16, true, true, false, false);
                    else if (Program.playerX == 1 && Program.playerY == 6) // 1,6
                        between = drawImageBasedOffDirection(32, 8, 48, 8, true, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 6) // 2,6
                        between = drawImageBasedOffDirection(40, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 3 && Program.playerY == 6) // 3,6
                        between = drawImageBasedOffDirection(16, 56, 64, 24, false, true, true, false);
                }
                else if (Program.loadDungeonTwo)
                {
                    if (Program.playerX == 6 && Program.playerY == 0) // 6,0
                    {
                        between = drawImageBasedOffDirection(136, 24, 16, 48, true, false, false, true);
                        if (between == 136)
                            Program.loadTextBox = 8;
                    }
                    else if (Program.playerX == 5 && Program.playerY == 0) // 5,0
                        between = drawImageBasedOffDirection(24, 16, 144, 128, false, false, true, true);
                    else if (Program.playerX == 4 && Program.playerY == 0) // 4,0
                        between = drawImageBasedOffDirection(160, 8, 56, 104, true, false, true, true);
                    else if (Program.playerX == 3 && Program.playerY == 0) // 3,0
                        between = drawImageBasedOffDirection(80, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 0) // 2,0
                        between = drawImageBasedOffDirection(32, 8, 40, 8, true, false, true, false);
                    else if (Program.playerX == 1 && Program.playerY == 0) // 1,0
                        between = drawImageBasedOffDirection(64, 24, 16, 152, true, false, true, true);
                    else if (Program.playerX == 0 && Program.playerY == 0) // 0,0
                        between = drawImageBasedOffDirection(8, 96, 8, 32, false, false, false, true);
                    else if (Program.playerX == 6 && Program.playerY == 1) // 6,1
                        between = drawImageBasedOffDirection(16, 176, 120, 24, false, true, true, false);
                    else if (Program.playerX == 5 && Program.playerY == 1) // 5,1
                        between = drawImageBasedOffDirection(112, 168, 8, 152, true, true, false, true);
                    else if (Program.playerX == 4 && Program.playerY == 1) // 4,1
                        between = drawImageBasedOffDirection(8, 88, 8, 48, false, true, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 1) // 3,1
                        between = drawImageBasedOffDirection(96, 8, 32, 8, false, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 1) // 2,1
                        between = drawImageBasedOffDirection(32, 8, 48, 8, true, false, true, false);
                    else if (Program.playerX == 1 && Program.playerY == 1) // 1,1
                        between = drawImageBasedOffDirection(56, 160, 24, 16, true, true, false, false);
                    else if (Program.playerX == 0 && Program.playerY == 1) // 0,1
                        between = drawImageBasedOffDirection(8, 32, 8, 40, false, true, false, true);
                    else if (Program.playerX == 6 && Program.playerY == 2) // 6,2
                        between = drawImageBasedOffDirection(24, 16, 176, 160, false, false, true, true);
                    else if (Program.playerX == 5 && Program.playerY == 2) // 5,2
                        between = drawImageBasedOffDirection(176, 184, 24, 16, true, true, false, false);
                    else if (Program.playerX == 4 && Program.playerY == 2) // 4,2
                        between = drawImageBasedOffDirection(16, 56, 64, 24, false, true, true, false);
                    else if (Program.playerX == 3 && Program.playerY == 2) // 3,2
                        between = drawImageBasedOffDirection(40, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 2) // 2,2
                        between = drawImageBasedOffDirection(32, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 1 && Program.playerY == 2) // 1,2
                        between = drawImageBasedOffDirection(32, 8, 48, 8, true, false, true, false);
                    else if (Program.playerX == 0 && Program.playerY == 2) // 0,2
                        between = drawImageBasedOffDirection(56, 64, 24, 16, true, true, false, false);
                    else if (Program.playerX == 6 && Program.playerY == 3) // 6,3
                        between = drawImageBasedOffDirection(16, 152, 64, 24, false, true, true, false);
                    else if (Program.playerX == 5 && Program.playerY == 3) // 5,3
                        between = drawImageBasedOffDirection(40, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 4 && Program.playerY == 3) // 4,3
                        between = drawImageBasedOffDirection(32, 8, 72, 8, true, false, true, false);
                    else if (Program.playerX == 3 && Program.playerY == 3) // 3,3
                        between = drawImageBasedOffDirection(64, 8, 152, 104, true, false, true, true);
                    else if (Program.playerX == 2 && Program.playerY == 3) // 2,3
                        between = drawImageBasedOffDirection(184, 24, 16, 56, true, false, false, true);
                    else if (Program.playerX == 1 && Program.playerY == 3) // 1,3
                        between = drawImageBasedOffDirection(24, 16, 152, 64, false, false, true, true);
                    else if (Program.playerX == 0 && Program.playerY == 3) // 0,3
                        between = drawImageBasedOffDirection(160, 24, 16, 56, true, false, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 4) // 3,4
                        between = drawImageBasedOffDirection(8, 88, 8, 32, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 4) // 2,4
                        between = drawImageBasedOffDirection(8, 48, 8, 48, false, true, false, true);
                    else if (Program.playerX == 1 && Program.playerY == 4) // 1,4
                        between = drawImageBasedOffDirection(8, 40, 8, 40, false, true, false, true);
                    else if (Program.playerX == 0 && Program.playerY == 4) // 0,4
                        between = drawImageBasedOffDirection(8, 48, 8, 32, false, true, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 5) // 3,5
                        between = drawImageBasedOffDirection(8, 32, 8, 48, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 5) // 2,5
                        between = drawImageBasedOffDirection(16, 56, 160, 24, false, true, true, false);
                    else if (Program.playerX == 1 && Program.playerY == 5) // 1,5
                        between = drawImageBasedOffDirection(152, 64, 24, 16, true, true, false, false);
                    else if (Program.playerX == 0 && Program.playerY == 5) // 0,5
                        between = drawImageBasedOffDirection(8, 32, 8, 40, false, true, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 6) // 3,6
                        between = drawImageBasedOffDirection(16, 56, 64, 24, false, true, true, false);
                    else if (Program.playerX == 2 && Program.playerY == 6) // 2,6
                        between = drawImageBasedOffDirection(40, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 1 && Program.playerY == 6) // 1,6
                        between = drawImageBasedOffDirection(32, 8, 48, 8, true, false, true, false);
                    else if (Program.playerX == 0 && Program.playerY == 6) // 0,6
                        between = drawImageBasedOffDirection(56, 64, 24, 16, true, true, false, false);
                }
                else if (Program.loadVillage)
                {
                    if (Program.playerX == 0 && Program.playerY == 3) // 0,3
                    {
                        between = drawImageBasedOffDirection(32, 8, 200, 8, true, false, true, false);
                        if (between == 200 && Program.tryingToGetIn == false)
                            Program.loadTextBox = 16;
                        else if (between == 200 && Program.tryingToGetIn)
                            Program.loadTextBox = 17;
                        else
                            Program.tryingToGetIn = false;
                    }
                    else if (Program.playerX == 1 && Program.playerY == 3) // 1,3
                        between = drawImageBasedOffDirection(96, 8, 32, 8, true, false, true, false);

                    else if (Program.playerX == 2 && Program.playerY == 3) // 2,3
                        between = drawImageBasedOffDirection(8, 56, 88, 64, false, true, true, true);
                    else if (Program.playerX == 2 && Program.playerY == 2) // 2,2
                        between = drawImageBasedOffDirection(8, 48, 8, 80, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 1) // 2,1
                        between = drawImageBasedOffDirection(192, 24, 16, 56, true, false, false, true);
                    else if (Program.playerX == 3 && Program.playerY == 1) // 3,1
                    {
                        Program.shopKeeperClose = false;
                        between = drawImageBasedOffDirection(56, 88, 184, 8, true, true, true, false);
                    }
                    else if (Program.playerX == 3 && Program.playerY == 0) // 3,0
                    {
                        if (Program.shopKeeperClose)
                        {
                            between = drawImageBasedOffDirection(128, 112, 120, 96, false, false, false, true);
                            if (between == 112 && Program.scammed == false)
                                Program.loadTextBox = 11;
                            else if (Program.scammed && Program.playerDirection == Program.east)
                            {
                                between = 484;
                                Program.loadTextBox = 19;
                            }
                        }
                        else
                        {
                            between = drawImageBasedOffDirection(8, 104, 8, 96, false, true, false, true);
                            if (between == 104 && Program.scammed == false)
                                Program.loadTextBox = 10;
                            else if (Program.scammed && Program.playerDirection == Program.east)
                                between = 476;
                        }
                    }
                    else if (Program.playerX == 4 && Program.playerY == 1) // 4,1
                        between = drawImageBasedOffDirection(32, 8, 80, 8, true, false, true, false);
                    else if (Program.playerX == 5 && Program.playerY == 1) // 5,1
                        between = drawImageBasedOffDirection(32, 8, 32, 8, true, false, true, false);
                    else if (Program.playerX == 6 && Program.playerY == 1) // 6,1
                    {
                        between = drawImageBasedOffDirection(176, 8, 32, 8, true, false, true, false);
                        if (between == 176)
                            Program.loadTextBox = 9;
                    }
                    else if (Program.playerX == 2 && Program.playerY == 4) // 2,4
                        between = drawImageBasedOffDirection(8, 72, 8, 32, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 5) // 2,5
                        between = drawImageBasedOffDirection(8, 32, 8, 136, false, true, false, true);
                    else if (Program.playerX == 2 && Program.playerY == 6) // 2,6
                    {
                        between = drawImageBasedOffDirection(152, 168, 160, 144, false, true, false, true);
                        if (between == 144 && Program.tryingToGetIn == false)
                            Program.loadTextBox = 15;
                        else if (between == 144 && Program.tryingToGetIn)
                            Program.loadTextBox = 17;                            
                        else
                            Program.tryingToGetIn = false;
                    }

                } 
                else if (Program.loadHouse)
                {
                    if (Program.playerX == 3 && Program.playerY == 0) // 3,0
                    {
                        between = drawImageBasedOffDirection(8, 24, 8, 16, false, true, false, false);
                        if (between == 16)
                            Program.loadTextBox = 18;
                        else if (between == 24)
                            Program.loadTextBox = 20;

                        if (Program.beenInHouse == false)
                            if (Program.playerInventory.Count < Program.playerInventory.Capacity)
                            {
                                Program.playerInventory.Add("Shop Keepers Hat");
                                Program.beenInHouse = true;
                            }
                    }
                }
                // DRAW MAP
                Console.Write(array[i]);
                // DRAW PLAYER POV
                Console.Write(array[i + between]);
                // DRAW COMPASS
                if (Program.loadVillage == false && Program.playerCaught == false && Program.fourBought && Program.loadHouse == false)
                {
                    if (Program.loadDungeonOne)
                        Console.Write(array[i + betweenTwo]);
                    else if (Program.loadDungeonTwo)
                        Console.Write(array[i + betweenTwo + -388]);
                }
                else if (Program.loadVillage == false && Program.playerCaught == false && Program.fourBought == false ||
                         Program.loadVillage == false && Program.playerCaught && Program.fourBought == false && Program.gettingKnockedOut ||
                         Program.loadVillage == false && Program.playerCaught && Program.fourBought && Program.gettingKnockedOut)
                    if (Program.loadDungeonOne)
                        Console.Write(array[i - 8]);
                    else if (Program.loadDungeonTwo)
                        Console.Write(array[i - 396]);
                // DRAW TREASURE MAP
                if (Program.twoBought && Program.loadDungeonOne|| Program.threeBought && Program.loadDungeonOne || Program.sixBought && Program.loadDungeonTwo|| Program.loadVillage)
                {
                    Console.Write(array[i + betweenThree]);
                    if (Program.loadDungeonOne)
                        drawLootCord(i, 97, 98, 99, 100, 101);
                    else if (Program.loadDungeonTwo)
                        drawLootCord(i, 485, 486, 487, 488, 489);
                }
                Console.WriteLine("");
            }
            return " ";
        }
        public static void drawLootCord(int input, int one, int two, int three, int four, int five)
        {
            // DRAWING LOOT CORDINATES FOR LIST
            if (Program.twoBought || Program.threeBought || Program.sixBought)
            {
                if (input == one)
                    Console.Write($"1 = ({Program.lOneX},{Program.lOneY})|");
                if (input == two)
                    Console.Write($"2 = ({Program.lTwoX},{Program.lTwoY})|");
                if (input == three)
                    Console.Write($"3 = ({Program.lThreeX},{Program.lThreeY})|");
                if (input == four)
                    Console.Write($"4 = ({Program.lFourX},{Program.lFourY})|");
                if (input == five)
                    Console.Write($"5 = ({Program.lFiveX},{Program.lFiveY})|");
            }
        }
        public static string drawTimer(string[] array, int start, int finish, int between)
        {
            bool draw = true;
            for (int i = start; i < finish; i++)
            {
                Console.Write(array[i]);
                if (draw && i == 51)
                {
                    // DRAW TIMER BAR
                    for (int s = 0; s < 35; s++)
                    {
                        if (Program.playerTimer > s)
                            BackgroundColor = ConsoleColor.Gray;
                        else
                            BackgroundColor = ConsoleColor.Red;
                        Console.Write("_");
                        ResetColor();
                    }
                    Console.Write("  ");
                    // DRAW PLAYER CASH 
                    ForegroundColor = ConsoleColor.DarkYellow;
                    double cash = Program.playerCash;
                    if (Program.playerCash < 1000)
                        Console.Write($"${cash}");
                    else if (Program.playerCash >= 1000)
                        Console.Write($"${String.Format("{0:#.##}", cash / 1000)}K");
                    else if (Program.playerCash >= 1000000)
                        Console.Write($"${String.Format("{0:#.##}", cash / 1000)}M");
                    else if (Program.playerCash >= 1000000000)
                        Console.Write($"${String.Format("{0:#.##}", cash / 1000)}B");
                    else if (Program.playerCash >= 1000000000000)
                        Console.Write($"${String.Format("{0:#.##}", cash / 1000)} Why are you doing this. . .");

                    ResetColor();
                    draw = false;
                }
                else if (i == 52)
                {
                    // DRAW PLAYER INVENTORY AND LIMIT
                    Program.canMove = true;
                    Program.canBarelyMove = true;
                    if (Program.playerInventory.Count < Program.playerInventory.Capacity)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                    Console.Write($"{Program.playerInventory.Count}/{Program.playerInventory.Capacity}");
                    ResetColor();
                }
                Console.WriteLine("");
            }
            return "";
        }
        public static void drawTextBox(string[] array, int start, int finish, int between)
        {
            // DRAWS TEXT AND BOTTOM OF LIST
            for (int i = start; i < finish; i++)
            {
                Console.Write(array[i]);
                if (Program.twoBought && Program.loadDungeonOne || Program.threeBought && Program.loadDungeonOne || Program.sixBought && Program.loadDungeonTwo || Program.loadVillage)
                {
                    Console.Write(array[i + between]);
                    if (i == start && Program.loadVillage)
                        Console.Write("         |");
                }

                if (i == start && Program.loadDungeonOne || i == start && Program.loadDungeonTwo)
                    if (Program.twoBought && Program.loadDungeonOne || Program.threeBought && Program.loadDungeonOne || Program.sixBought && Program.loadDungeonTwo)
                        Console.Write($"6 = ({Program.lSixX},{Program.lSixY})|");
                Console.WriteLine("");
            }
        }
        public static void drawShopMenu(string[] array, int start, int finish, int arrayStart, bool drawList)
        {
            for (int i = start; i < finish; i++)
            {
                Console.Write(array[i]);
                if (i == 10)
                {
                    // DRAW PLAYER CASH
                    ForegroundColor = ConsoleColor.DarkYellow;
                    double cash = Program.playerCash;
                    if (Program.playerCash < 1000)
                        Console.Write($" ${cash}");
                    else if (Program.playerCash >= 1000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}K");
                    else if (Program.playerCash >= 1000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}M");
                    else if (Program.playerCash >= 1000000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)}B");
                    else if (Program.playerCash >= 1000000000000)
                        Console.Write($" ${String.Format("{0:#.##}", cash / 1000)} Why are you doing this. . .");
                    ResetColor();
                }
                else if (i == 11)
                {
                    // DRAW PLAYER INVENTORY AND LIMIT
                    if (Program.playerInventory.Count < Program.playerInventory.Capacity)
                        ForegroundColor = ConsoleColor.Green;
                    else
                        ForegroundColor = ConsoleColor.Red;
                    Console.Write($" {Program.playerInventory.Count}/{Program.playerInventory.Capacity}");
                    ResetColor();
                }
                if (drawList)
                {
                    if (i >= 13)
                    {
                        Console.WriteLine("");
                        // DETERMINE COLOURS TO USE AND DRAW SHOP
                        for (int it = 0; it < 6; it++)
                        {
                            if (arrayStart == 0 && Program.oneBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            if (arrayStart == 1 && Program.twoBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            if (arrayStart == 2 && Program.threeBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            if (arrayStart == 3 && Program.fourBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            if (arrayStart == 4 && Program.fiveBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            if (arrayStart == 5 && Program.sixBought)
                                ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine(Program.shop[arrayStart]);
                            ResetColor();
                            arrayStart++;
                        }
                    }
                }
                Console.WriteLine("");
            }
        }
    }
    class Files
    {
        public static void writeToFile(string filePath, string[] dataToPutIn)
        {
            TextWriter updatedFile = new StreamWriter(filePath);
            foreach (string var in dataToPutIn)
                updatedFile.WriteLine(var);
            updatedFile.Close();
        }
        public static void loadFiles()
        {

            if (File.Exists(Program.playerFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.playerFilePath);
                Program.playerX = Convert.ToInt32(oldData[0]);
                Program.playerY = Convert.ToInt32(oldData[1]);
                Program.playerDirection = Convert.ToInt32(oldData[2]);
                Program.playerCashMultiplier = Convert.ToDouble(oldData[3]);
                Program.playerCash = Convert.ToDouble(oldData[4]);
                Program.playerEquiped = oldData[5];
                Program.playerMovedToNewArea = Convert.ToBoolean(oldData[6]);
                Program.shopKeeperClose = Convert.ToBoolean(oldData[7]);
                Program.playerCaught = Convert.ToBoolean(oldData[8]);
                Program.gettingKnockedOut = Convert.ToBoolean(oldData[9]);
                Program.knockedOut = Convert.ToBoolean(oldData[10]);
            }
            if (File.Exists(Program.enemyFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.enemyFilePath);
                Program.enemyX = Convert.ToInt32(oldData[0]);
                Program.enemyY = Convert.ToInt32(oldData[1]);
                Program.enemyActive = Convert.ToBoolean(oldData[2]);
            }
            if (File.Exists(Program.lootboxesFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.lootboxesFilePath);
                Program.lOneX = Convert.ToInt32(oldData[0]);
                Program.lOneY = Convert.ToInt32(oldData[1]);
                Program.lTwoX = Convert.ToInt32(oldData[2]);
                Program.lTwoY = Convert.ToInt32(oldData[3]);
                Program.lThreeX = Convert.ToInt32(oldData[4]);
                Program.lThreeY = Convert.ToInt32(oldData[5]);
                Program.lFourX = Convert.ToInt32(oldData[6]);
                Program.lFourY = Convert.ToInt32(oldData[7]);
                Program.lFiveX = Convert.ToInt32(oldData[8]);
                Program.lFiveY = Convert.ToInt32(oldData[9]);
                Program.lSixX = Convert.ToInt32(oldData[10]);
                Program.lSixY = Convert.ToInt32(oldData[11]);
                Program.oneOpen = Convert.ToBoolean(oldData[12]);
                Program.twoOpen = Convert.ToBoolean(oldData[13]);
                Program.threeOpen = Convert.ToBoolean(oldData[14]);
                Program.fourOpen = Convert.ToBoolean(oldData[15]);
                Program.fiveOpen = Convert.ToBoolean(oldData[16]);
                Program.sixOpen = Convert.ToBoolean(oldData[17]);
                Program.lootLoaded = Convert.ToBoolean(oldData[18]);
                Program.canMove = Convert.ToBoolean(oldData[19]);
                Program.onTreasure = Convert.ToBoolean(oldData[20]);
                Program.lookingAtTreasure = Convert.ToBoolean(oldData[21]);
                Program.gettingTreasure = Convert.ToBoolean(oldData[22]);
            }
            if (File.Exists(Program.shopFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.shopFilePath);
                Program.oneBought = Convert.ToBoolean(oldData[0]);
                Program.twoBought = Convert.ToBoolean(oldData[1]);
                Program.threeBought = Convert.ToBoolean(oldData[2]);
                Program.fourBought = Convert.ToBoolean(oldData[3]);
                Program.fiveBought = Convert.ToBoolean(oldData[4]);
                Program.sixBought = Convert.ToBoolean(oldData[5]);
                Program.playerBoughtItem = Convert.ToBoolean(oldData[6]);
            }
            if (File.Exists(Program.otherFilePath))
            {         
                string[] oldData = File.ReadAllLines(Program.otherFilePath);
                Program.loadDungeonOne = Convert.ToBoolean(oldData[0]);
                Program.loadDungeonTwo = Convert.ToBoolean(oldData[1]);
                Program.loadVillage = Convert.ToBoolean(oldData[2]);
                Program.canBarelyMove = Convert.ToBoolean(oldData[3]);
                Program.loadTextBox = Convert.ToInt32(oldData[4]);
                Program.tryingToGetIn = Convert.ToBoolean(oldData[5]);
                Program.loadHouse = Convert.ToBoolean(oldData[6]);
                Program.beenInHouse = Convert.ToBoolean(oldData[7]);
                Program.scammed = Convert.ToBoolean(oldData[8]);
            }
            if (File.Exists(Program.playerInFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.playerInFilePath);
                Program.playerInventory.Clear();
                Program.playerInventory.AddRange(oldData);

            }
            if (File.Exists(Program.playerExtraFilePath))
            {
                string[] oldData = File.ReadAllLines(Program.playerExtraFilePath);
                Program.playerExtraItems.Clear();
                Program.playerExtraItems.AddRange(oldData);
            }
        }
        public static void loadToFiles()
        {
            // CREATE PLAYER.TXT
            string[] dataPlayer = {$"{Program.playerX}", $"{Program.playerY}", $"{Program.playerDirection}", $"{Program.playerCashMultiplier}", $"{Program.playerCash}",
                                 $"{Program.playerEquiped}", $"{Program.playerMovedToNewArea}", $"{Program.shopKeeperClose}", $"{Program.playerCaught}",
                                 $"{Program.gettingKnockedOut}", $"{Program.knockedOut}"};
            Files.writeToFile(Program.playerFilePath, dataPlayer);
            // CREATE ENEMY.TXT
            string[] dataEnemy = { $"{Program.enemyX}", $"{Program.enemyY}", $"{Program.enemyActive}" };
            Files.writeToFile(Program.enemyFilePath, dataEnemy);
            // CREATE LOOT.TXT
            string[] dataLoot = {$"{Program.lOneX}", $"{Program.lOneY}", $"{Program.lTwoX}", $"{Program.lTwoY}", $"{Program.lThreeX}", $"{Program.lThreeY}", $"{Program.lFourX}", $"{Program.lFourX}",
                                 $"{Program.lFiveX}", $"{Program.lFiveY}", $"{Program.lSixX}", $"{Program.lSixY}", $"{Program.oneOpen}", $"{Program.twoOpen}", $"{Program.threeOpen}",
                                 $"{Program.fourOpen}", $"{Program.fiveOpen}", $"{Program.sixOpen}", $"{Program.lootLoaded}", $"{Program.canMove}", $"{Program.onTreasure}",
                                 $"{Program.lookingAtTreasure}", $"{Program.gettingTreasure}"};
            Files.writeToFile(Program.lootboxesFilePath, dataLoot);
            // CREATE SHOP.TXT
            string[] dataShop = {$"{Program.oneBought}", $"{Program.twoBought}", $"{Program.threeBought}", $"{Program.fourBought}", $"{Program.fiveBought}", $"{Program.sixBought}",
                                 $"{Program.playerBoughtItem}"};
            Files.writeToFile(Program.shopFilePath, dataShop);
            // CREATE OTHER.TXT
            string[] dataOther = { $"{Program.loadDungeonOne}", $"{Program.loadDungeonTwo}", $"{Program.loadVillage}", $"{Program.canBarelyMove}", $"{Program.loadTextBox}", $"{Program.tryingToGetIn}", $"{Program.loadHouse}", $"{Program.beenInHouse}", $"{Program.scammed}" };
            Files.writeToFile(Program.otherFilePath, dataOther);
            // CREATE PLAYEREXTRA.TXT
            string[] dataPlayerExtra = Program.playerExtraItems.ToArray();
            Files.writeToFile(Program.playerExtraFilePath, dataPlayerExtra);
            // CREATE PLAYERIN.TXT
            string[] dataPlayerIn = Program.playerInventory.ToArray();
            Files.writeToFile(Program.playerInFilePath, dataPlayerIn);
        }
    }
}