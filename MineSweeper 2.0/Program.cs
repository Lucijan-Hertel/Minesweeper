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
        const int CellWidth = 32;
        const int Coloums = 25;
        const int Rows = 25;
        int bombCount = 100;
        int LoopedTrough = 0;
        double Time = 0;
        int HintsUsed = 3;
        int fieldsNotVisable = 0;
        bool declared = false;
        bool gameEnded = false;
        bool gameWon = false;
        bool gotOutputed = false;
        bool CheatsUnlocked = false;
        bool playing = true;
        bool KIactive = false;
        string surroundedBombs = "";
        Color GridColor = Color.BLACK;
        Color TextColour = Color.GREEN;
        Vector2 mousePosition = new Vector2(-100.0f, -100.0f);

        Field[,] fields = new Field[Rows, Coloums];

        // Objects
        Random r = new Random();
        Field field = new Field(false, false, Color.DARKGRAY, false, false, false, false, false, 0);
        Gamemode gamemode = new Gamemode();
        Stopwatch stopwatch = new Stopwatch();

        int x = r.Next(24);
        int y = r.Next(24);

        // Lists
        List<Field> allFields = new List<Field>();

        Raylib.InitWindow(800, 800, "MineSweeper");

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
                    DeclareFields(fields, CellWidth);
                    RandomizeBombs(r, fields, bombCount);
                    declared = true;
                    CheckIfZero(fields);
                    Beginning(fields, r, x, y, true);

                }

                DrawFields(fields, CellWidth, mousePosition, TextColour, ref surroundedBombs);

                field.CheckIfBomb(fields, mousePosition, ref field.visable, CellWidth, ref gameEnded, ref bombCount, ref playing);
                AreAllBombsDetectedRight(fields, bombCount, ref gameWon);

                ShouldKIStart(fields, ref fieldsNotVisable, ref KIactive);

                isClickedFieldZero(fields, mousePosition, r);

                Help(ref HintsUsed, r, fields, x, y, ref LoopedTrough);

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

                // CheckIfZero(fields);

                DrawGrid(CellWidth, Coloums, Rows, GridColor);

                if (bombCount == 0)
                {
                    CheckIfBombsAreRightDetected(fields, ref gameEnded, bombCount);
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
                Raylib.ClearBackground(Color.BLUE);
                KIactive = false;
                HintsUsed = 3;
            }

            if (gotOutputed == false)
            {
                OutPut(mousePosition, fields, ref CheatsUnlocked);
                showAllBombs(fields);
                // gotOutputed = true;
            }

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
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
        for (int x = 0;  x < fields.GetLength(0); x++)
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

    public static void Help (ref int HintsUsed, Random r, Field[,] fields, int x, int y, ref int LoopedTrough)
    {
        if (HintsUsed != 0 && Raylib.IsKeyPressed(KeyboardKey.KEY_H))
        {
            if (LoopedTrough < 20)
            { 
            x = r.Next(24);
            y = r.Next(24);

                if (!fields[x, y].visable && fields[x, y].isZeroBombs)
                {
                fields[x, y].visable = true;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {

                        if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
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
                                Beginning(fields, r, x + dx, y + dy, false);
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
                        Help(ref HintsUsed, r, fields, x, y, ref LoopedTrough);
                    }
                }
            }

            else
            {
                HelpWithoutZeros(ref HintsUsed, r, fields, x, y);
            }
        }
    }

    public static void HelpWithoutZeros(ref int HintsUsed, Random r, Field[,] fields, int x, int y)
    {
        bool OneFieldHintUsed = false;
        bool IsUsed = false;

        if (HintsUsed != 0)
        {
            x = r.Next(25);
            y = r.Next(25);

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
                HelpWithoutZeros(ref HintsUsed, r, fields, x, y);
            }
        }
    }

    public static void DrawFields(Field[,] fields, int CellWidth, Vector2 mousePosition, Color TextColour, ref string surroundedBombs)
    {

        for (int i = 0; i < fields.GetLength(0); i++)
        {
            for (int j = 0; j < fields.GetLength(1); j++)
            {

                CheckBombs(fields, i, j);

                Raylib.DrawRectangle(i * CellWidth, j * CellWidth, CellWidth, CellWidth, fields[i, j].colour);
                
                if (!fields[i, j].isBomb && fields[i, j].visable && !fields[i, j].bombDetected)
                {
                    surroundedBombs = fields[i,j].bombDetecting.ToString();
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
                        CheckBombs(fields, i, j);
                    }
                }
            }
        }
    }

    public static void CheckBombs(Field[,] fields, int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (x + dx < 0 || y + dy < 0 || x + dx > 24 || y + dy > 24)
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

    public static void CheckIfZero(Field[,] fields)
    {
        for (int x = 0; x <= 24; x++)
        {
            for (int y = 0; y <= 24; y++)
            {
                CheckBombs(fields, x, y);
                
                if (fields[x,y].bombDetecting == 0)
                {
                    // fields[x, y].visable = true;
                    fields[x, y].isZeroBombs = true;
                    // fields[x, y].colour = Color.DARKGREEN;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
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

    public static void Beginning(Field[,] fields, Random r, int x, int y, bool firsttime)
    {

        for (int i = 0; i < 24; i++)
        {
            if (fields[x, y].isZeroBombs)
            {
                i = 25;
                fields[x, y].visable = true;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {

                        if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
                        {
                        }
                        else
                        {
                            fields[x + dx, y + dy].visable = true;
                            fields[x + dx, y + dy].colour = Color.DARKGREEN;
                            
                            if (fields[x + dx, y + dy].isZeroBombs && !fields[x + dx, y + dy].alreadyChecked)
                            {
                                fields[x + dx, y + dy].alreadyChecked = true;
                                Beginning(fields, r, x + dx, y + dy, false);
                            }
                        }
                    }
                }
            }
            else if (firsttime == true)
            {
                i--;
                x = r.Next(24);
                y = r.Next(24);
            }
        }
    }

    public static void isClickedFieldZero(Field[,] fields, Vector2 mousePosition, Random r)
    {
        if (mousePosition.X >= 0 && mousePosition.X < 25 && mousePosition.Y >= 0 && mousePosition.Y < 25)
        {
            if (fields[(int)mousePosition.X, (int)mousePosition.Y].isZeroBombs && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                Beginning(fields, r, (int)mousePosition.X, (int)mousePosition.Y, false);
            }
        }
    }


    public static void RandomizeBombs(Random r, Field[,] fields, int bombcount)
    {
        for (int x = 1; x <= bombcount; x++)
        {
            int y = r.Next(25);
            int z = r.Next(25);

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
    
    public static void AreAllBombsDetectedRight(Field[,] fields, int bombCount, ref bool gameWin)
    {
        for(int x = 0;x <= 24; x++)
        {
            for (int y = 0; y <= 24; y++)
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
        Time = (int) Time;
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
                Console.Write(fields[(int)mousePosition.X, (int)mousePosition.Y].visable + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].isBomb + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].bombDetected + " | " + fields[(int)mousePosition.X, (int)mousePosition.Y].isZeroBombs );
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

    public static void ShouldKIStart(Field[,] fields, ref int fieldsNotVisable, ref bool KIactive)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_K))
        {
            KIactive = true;
        }
        
        if (KIactive == true)
        {
            for(int x = 0; x < 25; x++)
            {
                for (int y = 0; y < 25; y++)
                {
                    if (fields[x, y].visable && !fields[x, y].bombDetected)
                    {
                        NotVisableFieldsEqualsNumber(fields, ref fieldsNotVisable, x, y);
                        OtherFieldsVisable(fields, x, y);
                    }
                }
            }
        }
    }

    public static void NotVisableFieldsEqualsNumber(Field[,] fields,ref int fieldsNotVisable, int x, int y)
    {
        fieldsNotVisable = 0;
        
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {

                if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
                {
                }
                else if (!fields[x + dx, y + dy].visable || fields[x + dx, y + dy].bombDetected)
                {
                    fieldsNotVisable++;
                }
            }
        }

        if (fieldsNotVisable == fields[x,y].bombDetecting && fields[x, y].bombDetecting != 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {

                    if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
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

    public static void OtherFieldsVisable(Field[,] fields, int x, int y)
    {
        int BombCount = 0;
        
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {

                if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
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

                    if ((x + dx < 0 || y + dy < 0) || (x + dx > 24 || y + dy > 24))
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