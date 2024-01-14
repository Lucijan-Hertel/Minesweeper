using MineSweeper_2._0;
using Raylib_cs;
using System.Diagnostics;
using System.Numerics;

namespace MineSweeper_2._0;

class Program
{
    public static void Main()
    {
        // Game variables & vectors
        const int CellWidth = 35; /*-------------------------------------*/ // How big a cell is (pixels)                                                                      [field.CheckIfBomb(); | ShouldKiStart(); | Converting(); | DrawRectangle(); | DeclareFields(); | DrawFields(); | DrawGrid(); |GameOver(); | Win();]
        const int Coloumns = 25; /*--------------------------------------*/ // How many coloumns one gamefield has                                                              [fields(); | ShouldKiStart(); | RandomizeBombs(); | CheckIfZero(); | Beginning(); | DrawFields(); | AreAllBombsRightDetected(); | IsClickedFieldZero(); | Help(); | DrawGrid();]
        const int Rows = 25; /*------------------------------------------*/ // How many rows one gamefield has                                                                  [fields(); | RandomizeBombs(); | CheckIfZero(); | Beginning(); | DrawFields(); | AreAllBombsRightDetected(); | IsClickedFieldZero(); | Help(); | DrawGrid();]
        int bombCount = Coloumns * Rows / 6; /*------------------------*/ // How many bombs are allowed in one game                                                           [field.CheckIfBomb(); | RandomizeBombs(); | AreAllBombsDetectedRight(); | CheckIfBombsAreRightDetected();]
        int LoopedTrough = 0; /*-----------------------------------------*/ // How often sth. got looped through                                                                [Help();]
        int x = 0; /*------------------------------------------------------*/ // Random generated x position                                                                      [Beginning(); | Help();]
        int y = 0; /*------------------------------------------------------*/ // Random generated y position                                                                      [Beginning(); | Help();]
        double Time = 0; /*-----------------------------------------------*/ // How long the user took until the game ended (because he lost or he won, both)                    [Win(); | GameOver();]
        int HintsUsed = 3; /*---------------------------------------------*/ // How many hints the user can use                                                                  [Help();]
        int fieldsNotVisable = 0; /*-------------------------------------*/ // [ShouldKiStart();]
        bool declared = false; /*----------------------------------------*/ // If fields are already declared at the start [GameOver(); | Win();]
        bool gameEnded = false; /*---------------------------------------*/ // If the game ends (by losing)                                                                     [field.CheckIfBomb() | CheckIfAllBombsAreRightDetected(); | GameOver(); | Win();] 
        bool gameWon = false; /*------------------------------------------*/ // If the game ends (by winning)                                                                    [AreAllBombsRightDetected(); | GameOver(); | Win();]    
        bool gotOutputed = false; /*-------------------------------------*/ // If the user wants to outpoot details about a field he hovers over with his mouse                 [----] 
        bool CheatsUnlocked = false; /*----------------------------------*/ // If the user wants to get cheats outputted                                                        [OutPut();]
        bool playing = true; /*-------------------------------------------*/ // If the gamemode is set to play (so that you can use the main game                                [field.CheckIfBomb(); | GameOver(); | Win();]
        bool KIactive = false; /*-----------------------------------------*/ // If the algorythmn should be turned on                                                            [ShouldKiStart();]
        string surroundedBombs = ""; /*----------------------------------*/ // Tells the program which number it should write on a field if it is visable and not a bomb        [DrawFields();]
        Color GridColor = Color.BLACK; /*--------------------------------*/ // What the colour of the grid should be                                                            [DrawGrid();]
        Color TextColour = Color.GREEN; /*-------------------------------*/ // What the text colour of the numbers on the field should be                                       [DrawFields();]
        Vector2 mousePosition = new Vector2(-100.0f, -100.0f); /*-----*/ // Where the mouse is on the screen                                                                 [field.CheckIfBomb(); | Converting(); | DrawRectangle(); | DrawFields(); | IsClickedFieldZero(); | GameOver(); | Win(); | OutPut();]

        Field[,] fields = new Field[Rows, Coloumns];

        // Objects
        Random r = new Random();
        Field field = new Field(false, false, Color.DARKGRAY, false, false, false, false, false, 0);
        Gamemode gamemode = new Gamemode();
        Stopwatch stopwatch = new Stopwatch();

        // Lists
        List<Field> allFields = new List<Field>();

        Raylib.InitWindow(CellWidth * Rows, CellWidth * Coloumns, "MineSweeper");

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            if (playing)
            {
                gamemode = Gamemode.playing;
            }

            if (gamemode == Gamemode.playing)
            {
                stopwatch.Start();
                Raylib.ClearBackground(Color.DARKGRAY);

                mousePosition = Raylib.GetMousePosition();

                // Code here
                Converting(ref mousePosition, CellWidth);

                DrawRectangle(mousePosition, CellWidth);

                if (declared == false)
                {
                    x = r.Next(Rows - 1);
                    y = r.Next(Coloumns - 1);
                    DeclareFields(fields, CellWidth);
                    RandomizeBombs(r, fields, bombCount, Coloumns, Rows);
                    CheckIfZero(fields, Coloumns, Rows);
                    Beginning(fields, r, Coloumns, Rows, ref x, ref y, true);
                    KIactive = true;
                    for (int i = 0; i < 20; i++)
                    {
                        ShouldKIStart(fields, ref fieldsNotVisable, Rows, Coloumns, ref KIactive);
                    }
                }

                DrawFields(fields, mousePosition, TextColour, ref surroundedBombs, CellWidth, Coloumns, Rows);

                field.CheckIfBomb(fields, mousePosition, ref field.visable, CellWidth, ref gameEnded, ref bombCount, Coloumns, Rows, ref playing);

                AreAllBombsDetectedRight(fields, bombCount, Coloumns, Rows, ref gameWon);

                ShouldKIStart(fields, ref fieldsNotVisable, Rows, Coloumns, ref KIactive);

                IsClickedFieldZero(fields, mousePosition, r, Coloumns, Rows);

                Help(r, fields, ref HintsUsed, ref LoopedTrough, Coloumns, Rows, x, y);

                // CheckIfZero(fields);

                DrawGrid(CellWidth, Coloumns, Rows, GridColor);

                if (bombCount == 0)
                {
                    CheckIfBombsAreRightDetected(fields, ref gameEnded, bombCount);
                }

                if (gameEnded)
                {
                    gamemode = Gamemode.end;
                    Time = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();
                }

                if (gameWon)
                {
                    gamemode = Gamemode.win;
                    Time = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();
                }

                if (declared == false)
                {
                    if (gamemode == Gamemode.win)
                    {
                        gamemode = Gamemode.playing;
                        declared = true;
                        KIactive = false;
                        gameWon = false;
                        for (int i = 0; i < Rows; i++)
                        {
                            for (int j = 0; j < Coloumns; j++)
                            {
                                fields[i, j].gotClicked = false;
                                fields[i, j].visable = false;
                                fields[i, j].bombDetected = false;
                                fields[i, j].alreadyChecked = false;
                                fields[i, j].colour = Color.DARKGRAY;
                            }
                        }
                        Beginning(fields, r, Coloumns, Rows, ref x, ref y, false);
                    }
                }
            }

            if (gamemode == Gamemode.end)
            {
                GameOver(ref Time, CellWidth, mousePosition, ref playing, ref gameEnded, ref declared, ref gameWon);
                Raylib.ClearBackground(Color.BLUE);
                KIactive = false;
                HintsUsed = 3;
            }

            if (gamemode == Gamemode.win)
            {
                playing = false;
                Win(ref Time, CellWidth, mousePosition, ref playing, ref gameEnded, ref declared, ref gameWon);
                Raylib.ClearBackground(Color.GREEN);
                KIactive = false;
                HintsUsed = 3;
            }

            if (gotOutputed == false)
            {
                OutPut(mousePosition, fields, ref CheatsUnlocked);
                showAllBombs(fields);
                // gotOutputed = true;
            }

            for (int i = 0; i < Coloumns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    fields[i, j].bombDetecting = 0;
                }
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static void CheckIfBombsAreRightDetected(Field[,] fields, ref bool gameEnded, int bombCount)
    {
        int RightBombDetected = 0;
        for (int x = 0; x < fields.GetLength(0); x++)
        {
            for (int y = 0; y < fields.GetLength(1); y++)
            {
                if (fields[x, y].isBomb && fields[x, y].bombDetected)
                {
                    RightBombDetected++;
                }
            }
        }

        if (RightBombDetected == bombCount)
        {
            // Game Should End Now
            gameEnded = true;
        }
    }

    public static void Converting(ref Vector2 mousePosition, int CellWidth)
    {
        mousePosition.X = mousePosition.X / CellWidth;
        mousePosition.Y = mousePosition.Y / CellWidth;
    }

    public static void DrawGrid(int CellWidth, int Coloums, int Rows, Color GridColour)
    {
        for (int i = 0; i < Coloums; i++)
        {
            for (int j = 0; j < Rows; j++)
            {
                Raylib.DrawRectangleLines(i * CellWidth, j * CellWidth, Coloums * CellWidth, Rows * CellWidth, GridColour);
            }
        }
    }

    public static void DrawRectangle(Vector2 mousePosition, int CellWidth)
    {
        Raylib.DrawRectangle((int)mousePosition.X * CellWidth, (int)mousePosition.Y * CellWidth, CellWidth, CellWidth, Color.DARKBLUE);
    }

    public static void DeclareFields(Field[,] fields, int CellWidth)
    {

        for (int i = 0; i < fields.GetLength(0); i++)
        {
            for (int j = 0; j < fields.GetLength(1); j++)
            {
                fields[i, j] = new Field(false, false, Color.DARKGRAY, false, false, false, false, false, 0);
            }
        }

    }

    public static void Help(Random r, Field[,] fields, ref int HintsUsed, ref int LoopedTrough, int Coloumns, int Rows, int x, int y)
    {
        if (HintsUsed != 0 && Raylib.IsKeyPressed(KeyboardKey.KEY_H))
        {
            if (LoopedTrough < 20)
            {
                x = r.Next(Coloumns - 1);
                y = r.Next(Rows - 1);

                if (!fields[x, y].visable && fields[x, y].isZeroBombs)
                {
                    fields[x, y].visable = true;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {

                            if ((x + dx < 0 || y + dy < 0) || (x + dx > Rows - 1 || y + dy > Coloumns - 1))
                            {
                            }
                            else
                            {
                                fields[x + dx, y + dy].visable = true;
                                fields[x + dx, y + dy].colour = Color.DARKGREEN;


                                if (fields[x + dx, y + dy].isZeroBombs && !fields[x + dx, y + dy].alreadyChecked)
                                {
                                    fields[x + dx, y + dy].alreadyChecked = true;
                                    LoopedTrough = 0;
                                    int xx = x + dx;
                                    int yy = y + dy;
                                    Beginning(fields, r, Coloumns, Rows, ref xx, ref yy, false);
                                }
                            }
                        }
                    }

                    HintsUsed--;
                }
                else
                {
                    if (LoopedTrough < 20)
                    {
                        LoopedTrough++;
                        Help(r, fields, ref HintsUsed, ref LoopedTrough, Coloumns, Rows, x, y);
                    }
                }
            }

            else
            {
                HelpWithoutZeros(fields, r, ref HintsUsed, Coloumns, Rows, x, y);
            }
        }
    }

    public static void HelpWithoutZeros(Field[,] fields, Random r, ref int HintsUsed, int Coloumns, int Rows, int x, int y)
    {
        bool OneFieldHintUsed = false;
        bool IsUsed = false;

        if (HintsUsed != 0)
        {
            x = r.Next(Coloumns - 1);
            y = r.Next(Rows - 1);

            if (!fields[x, y].visable && !fields[x, y].isBomb)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24) || OneFieldHintUsed)
                        {
                        }
                        else if (fields[x + dx, y + dy].visable)
                        {
                            fields[x, y].visable = true;
                            fields[x, y].colour = Color.DARKGREEN;
                            fields[x, y].gotClicked = true;
                            OneFieldHintUsed = true;
                            Console.Write(x + dx);
                            Console.Write(y + dy);
                            Console.WriteLine();
                            HintsUsed--;
                            IsUsed = true;
                        }
                        else if (!fields[x, y].visable && fields[x, y].isBomb && fields[x + dx, y + dy].visable)
                        {
                            fields[x, y].bombDetected = true;
                        }
                    }
                }

                Console.WriteLine(HintsUsed);
            }
            if (!IsUsed)
            {
                HelpWithoutZeros(fields, r, ref HintsUsed, Coloumns, Rows, x, y);
            }
        }
    }

    public static void DrawFields(Field[,] fields, Vector2 mousePosition, Color TextColour, ref string surroundedBombs, int CellWidth, int Coloumns, int Rows)
    {

        for (int i = 0; i < fields.GetLength(0); i++)
        {
            for (int j = 0; j < fields.GetLength(1); j++)
            {

                CheckBombs(fields, Coloumns, Rows, i, j);

                Raylib.DrawRectangle(i * CellWidth, j * CellWidth, CellWidth, CellWidth, fields[i, j].colour);

                if (!fields[i, j].isBomb && fields[i, j].visable && !fields[i, j].bombDetected)
                {
                    surroundedBombs = fields[i, j].bombDetecting.ToString();

                    if (surroundedBombs == "0")
                    {
                        surroundedBombs = "";
                    }

                    Raylib.DrawText(surroundedBombs.ToString(), i * CellWidth + 8, j * CellWidth + 2, CellWidth, TextColour);
                }
                else
                {
                    if (fields[i, j].gotClicked == true)
                    {
                        CheckBombs(fields, Coloumns, Rows, i, j);
                    }
                }
            }
        }
    }

    public static void CheckBombs(Field[,] fields, int Coloumns, int Rows, int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (x + dx < 0 || y + dy < 0 || x + dx > Coloumns - 1 || y + dy > Rows - 1)
                {
                }
                else
                {
                    if (fields[x + dx, y + dy].isBomb)
                    {
                        fields[x, y].bombDetecting++;
                    }
                }
            }
        }
    }

    public static void CheckIfZero(Field[,] fields, int Coloumns, int Rows)
    {
        for (int x = 0; x <= Rows - 1; x++)
        {
            for (int y = 0; y <= Coloumns - 1; y++)
            {
                CheckBombs(fields, Coloumns, Rows, x, y);

                if (fields[x, y].bombDetecting == 0)
                {
                    // fields[x, y].visable = true;
                    fields[x, y].isZeroBombs = true;
                    // fields[x, y].colour = Color.DARKGREEN;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if ((x + dx < 0 || y + dy < 0) || (x + dx > Coloumns - 1 || y + dy > Rows - 1))
                            {
                            }
                            else
                            {
                                // fields[x + dx, y + dy].visable = true;
                                // fields[x + dx, y + dy].colour = Color.DARKGREEN;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void Beginning(Field[,] fields, Random r, int Coloumns, int Rows, ref int x, ref int y, bool firsttime)
    {

        for (int i = 0; i < Coloumns; i++) // Could be an mistake
        {
            if (fields[x, y].isZeroBombs)
            {
                i = Coloumns;
                fields[x, y].visable = true;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {

                        if ((x + dx < 0 || y + dy < 0) || (x + dx > Coloumns - 1 || y + dy > Rows - 1))
                        {
                        }
                        else
                        {
                            fields[x + dx, y + dy].visable = true;
                            fields[x + dx, y + dy].colour = Color.DARKGREEN;

                            if (fields[x + dx, y + dy].isZeroBombs && !fields[x + dx, y + dy].alreadyChecked)
                            {
                                fields[x + dx, y + dy].alreadyChecked = true;
                                int xx = x + dx;
                                int yy = y + dy;
                                Beginning(fields, r, Coloumns, Rows, ref xx, ref yy, false);
                            }
                        }
                    }
                }
            }
            else if (firsttime)
            {
                i--;
                x = r.Next(Rows - 1);
                y = r.Next(Coloumns - 1);
            }
        }
    }

    public static void IsClickedFieldZero(Field[,] fields, Vector2 mousePosition, Random r, int Coloumns, int Rows)
    {
        if (mousePosition.X >= 0 && mousePosition.X < Rows && mousePosition.Y >= 0 && mousePosition.Y < Coloumns)
        {
            if (fields[(int)mousePosition.X, (int)mousePosition.Y].isZeroBombs && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                int xx = (int)mousePosition.X;
                int yy = (int)mousePosition.Y;
                Beginning(fields, r, Coloumns, Rows, ref xx, ref yy, false);
            }
        }
    }

    public static void RandomizeBombs(Random r, Field[,] fields, int bombcount, int Coloumns, int Rows)
    {
        for (int x = 1; x <= bombcount; x++)
        {
            int z = r.Next(Rows); // Z is cursed
            int y = r.Next(Coloumns);

            if (!fields[z, y].isBomb)
            {
                fields[z, y].isBomb = true;
            }
            else
            {
                x--;
            }
        }
    }

    public static void AreAllBombsDetectedRight(Field[,] fields, int bombCount, int Coloumns, int Rows, ref bool gameWin)
    {
        for (int x = 0; x <= Rows - 1; x++)
        {
            for (int y = 0; y <= Coloumns - 1; y++)
            {
                if (fields[x, y].isBomb && fields[x, y].bombDetected)
                {
                    bombCount--;
                }
            }
        }

        if (bombCount == 0)
        {
            gameWin = true;
        }
    }

    public static void GameOver(ref double Time, int CellWidth, Vector2 mousePosition, ref bool playing, ref bool gameEnded, ref bool declared, ref bool GameWon)
    {
        Raylib.DrawText("Game Over", 5 * 30, 11 * 30, 100, Color.RED);
        TimeRound(ref Time);
        Raylib.DrawText(Time.ToString() + "s", 13 * 30, 15 * 30, 100, Color.RED);
        Button(CellWidth, mousePosition, ref playing, ref gameEnded, ref declared, ref Time, ref GameWon);
    }

    public static void Win(ref double Time, int CellWidth, Vector2 mousePosition, ref bool playing, ref bool gameEnded, ref bool declared, ref bool GameWon)
    {
        Raylib.DrawText("Congrats! You win", 3 * 30, 11 * 30, 75, Color.RED);
        TimeRound(ref Time);
        Raylib.DrawText(Time.ToString() + "s", 13 * 30, 15 * 30, 100, Color.RED);
        Button(CellWidth, mousePosition, ref playing, ref gameEnded, ref declared, ref Time, ref GameWon);
    }

    public static void TimeRound(ref double Time)
    {
        Time = (int)Time;
    }

    public static void Button(int CellWidth, Vector2 mousePosition, ref bool playing, ref bool gameEnded, ref bool declared, ref double Time, ref bool gameWon)
    {
        for (int i = 9; i < 18; i++)
        {
            for (int j = 19; j < 23; j++)
            {
                Raylib.DrawRectangle(i * CellWidth, j * CellWidth, CellWidth, CellWidth, Color.DARKBLUE);
                Raylib.DrawText("Play Again", 15 + 9 * CellWidth, 20 * CellWidth, 50, Color.RED);
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                mousePosition = Raylib.GetMousePosition() / CellWidth;

                if ((int)mousePosition.X > 9 && (int)mousePosition.X < 18 && (int)mousePosition.Y > 19 && (int)mousePosition.Y < 23)
                {
                    playing = true;
                    gameEnded = false;
                    gameWon = false;
                    declared = false;
                    Time = 0;
                }
            }
        }
    }

    // DEFENITLY NO CHEATS NO WHY SHOULD I HAVE THIS IDEA

    public static void OutPut(Vector2 mousePosition, Field[,] fields, ref bool CheatsUnlocked)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_ALT))
        {
            CheatsUnlocked = true;
        }

        if (CheatsUnlocked)
        {
            if (mousePosition.X >= 0 && mousePosition.X < 25 && mousePosition.Y >= 0 && mousePosition.Y < 25)
            {
                Console.Write((int)mousePosition.X + " | " + (int)mousePosition.Y + " || ");
                Console.Write(fields[(int)mousePosition.X, (int)mousePosition.Y].visable + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].isBomb + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].bombDetected + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].isZeroBombs);
                Console.WriteLine(" ||| " + "[X-Coordinate | Y-Coordinate || Visable | isBomb | isBombDetected | NoBombs]");
            }
        }
    }

    public static void showAllBombs(Field[,] fields)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT_ALT))
        {
            for (int i = 0; i < fields.GetLength(0); i++)
            {
                for (int j = 0; j < fields.GetLength(1); j++)
                {
                    if (fields[i, j].isBomb)
                    {
                        fields[i, j].visable = true;
                        fields[i, j].bombDetected = true;
                        fields[i, j].colour = Color.BLUE;
                    }
                }
            }
        }
    }

    // Defenetly not the Start of a KI which can play the full game to the end

    public static void ShouldKIStart(Field[,] fields, ref int fieldsNotVisable, int Rows, int Coloumns, ref bool KIactive)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_K) && KIactive == false)
        {
            KIactive = true;
        }
        else if (Raylib.IsKeyPressed(KeyboardKey.KEY_K) && KIactive == true)
        {
            KIactive = false;
        }


        if (KIactive == true)
        {
            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Coloumns; y++)
                {
                    if (fields[x, y].visable && !fields[x, y].bombDetected)
                    {
                        NotVisableFieldsEqualsNumber(fields, ref fieldsNotVisable, Coloumns, Rows, x, y);
                        OtherFieldsVisable(fields, Coloumns, Rows, x, y);
                    }
                }
            }
        }
    }

    public static void NotVisableFieldsEqualsNumber(Field[,] fields, ref int fieldsNotVisable, int Coloumns, int Rows, int x, int y)
    {
        fieldsNotVisable = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {

                if ((x + dx < 0 || y + dy < 0) || (x + dx > Rows - 1 || y + dy > Coloumns - 1))
                {
                }
                else if (!fields[x + dx, y + dy].visable || fields[x + dx, y + dy].bombDetected)
                {
                    fieldsNotVisable++;
                }
            }
        }

        if (fieldsNotVisable == fields[x, y].bombDetecting && fields[x, y].bombDetecting != 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {

                    if ((x + dx < 0 || y + dy < 0) || (x + dx > Rows - 1 || y + dy > Coloumns - 1))
                    {
                    }
                    else if (!fields[x + dx, y + dy].visable)
                    {
                        fields[x + dx, y + dy].bombDetected = true;
                        fields[x + dx, y + dy].visable = true;
                    }

                }
            }
        }
    }

    public static void OtherFieldsVisable(Field[,] fields, int Coloumns, int Rows, int x, int y)
    {
        int BombCount = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {

                if ((x + dx < 0 || y + dy < 0) || (x + dx > Rows - 1 || y + dy > Coloumns - 1))
                {
                }
                else if (fields[x + dx, y + dy].bombDetected)
                {
                    BombCount++;
                }
            }
        }

        if (BombCount == fields[x, y].bombDetecting)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {

                    if ((x + dx < 0 || y + dy < 0) || (x + dx > Rows - 1 || y + dy > Coloumns - 1))
                    {
                    }
                    else if (!fields[x + dx, y + dy].visable)
                    {
                        fields[x + dx, y + dy].visable = true;
                        fields[x + dx, y + dy].colour = Color.DARKGREEN;
                        fields[x + dx, y + dy].gotClicked = true;
                    }
                }
            }
        }
    }

    enum Gamemode
    {
        playing,
        end,
        win
    }
}