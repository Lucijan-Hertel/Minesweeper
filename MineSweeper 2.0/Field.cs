using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace MineSweeper_2._0
{
    internal class Field
    {
        // Variables & Vectors
        // Bomb variables & vectors
        public bool visable;
        public bool isBomb;
        public Color colour;
        public bool gotClicked;
        public bool bombDetected;
        public bool isZeroBombs;
        public bool alreadyChecked;
        // For AI
        public int bombDetecting;

        public Field(bool visable, bool isBomb, Color colour, bool changedColour, bool gotClicked, bool bombDetected, bool isZeroBombs, bool alreadyChecked, int bombDetecting)
        {
            this.visable = visable;
            this.isBomb = isBomb;
            this.colour = colour;
            this.gotClicked = gotClicked;
            this.bombDetected = bombDetected;
            this.isZeroBombs = isZeroBombs;
            this.alreadyChecked = alreadyChecked;
            // For AI
            this.bombDetecting = bombDetecting;
        }

        public void CheckIfBomb(Field[,] fields, Vector2 mousePosition, ref bool visable, int CellWidth, ref bool gameEnded, ref int bombCount, int Coloumns, int Rows, ref bool playing)
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (fields[(int)mousePosition.X, (int)mousePosition.Y].isBomb)
                {
                    fields[(int)mousePosition.X, (int)mousePosition.Y].colour = Color.RED;
                    gameEnded = true;
                    playing = false;
                }
                else if (!fields[(int)mousePosition.X, (int)mousePosition.Y].visable)
                {
                    fields[(int)mousePosition.X, (int)mousePosition.Y].visable = true;
                    fields[(int)mousePosition.X, (int)mousePosition.Y].colour = Color.DARKGREEN;
                }
                fields[(int)mousePosition.X, (int)mousePosition.Y].gotClicked = true;
            }

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT))
            {
                if (!fields[(int)mousePosition.X, (int)mousePosition.Y].visable)
                {
                    fields[(int)mousePosition.X, (int)mousePosition.Y].visable = true;
                    fields[(int)mousePosition.X, (int)mousePosition.Y].colour = Color.BLUE;
                    fields[(int)mousePosition.X, (int)mousePosition.Y].bombDetected = true;
                }
                else if (fields[(int)mousePosition.X, (int)mousePosition.Y].bombDetected == true)
                {
                    fields[(int)mousePosition.X, (int)mousePosition.Y].bombDetected = false;
                    fields[(int)mousePosition.X, (int)mousePosition.Y].colour = Color.DARKGRAY;
                    fields[(int)mousePosition.X, (int)mousePosition.Y].visable = false;
                }
                fields[(int)mousePosition.X, (int)mousePosition.Y].gotClicked = true;
            }

            for (int x = 0; x < Rows; x++)
            {
                for (int y = 0; y < Coloumns; y++)
                {
                    if (fields[x, y].bombDetected)
                    {
                        fields[x, y].visable = true;
                        fields[x, y].colour = Color.BLUE;
                        fields[x, y].bombDetected = true;
                        fields[x, y].gotClicked = true;
                    }
                }
            }
        }
    }
}