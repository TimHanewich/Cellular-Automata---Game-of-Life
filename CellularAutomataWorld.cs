using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.IO;

namespace CellularAutomata.GameOfLife
{
    public class CellularAutomataWorld
    {
        public bool[] Cells { get; set; }
        public int WorldHeight { get; set; }
        public int WorldWidth { get; set; }

        public static CellularAutomataWorld Create(int width, int height)
        {
            CellularAutomataWorld ToReturn = new CellularAutomataWorld();

            if (height <= 0 || width <= 0)
            {
                throw new Exception("Height and/or Width was 0 or less than 0.");
            }

            ToReturn.WorldHeight = height;
            ToReturn.WorldWidth = width;

            int TotalCellCount = height * width;
            List<bool> NCells = new List<bool>();
            int t = 1;
            for (t = 1; t <= TotalCellCount; t++)
            {
                bool NC = false;
                NCells.Add(NC);
            }
            ToReturn.Cells = NCells.ToArray();

            ToReturn.RandomizeCells();
            return ToReturn;
        }

        public static CellularAutomataWorld Load(bool[] cells, int width, int height)
        {
            if ((width * height) != cells.Length)
            {
                throw new Exception("Unable to load a Ceullular Automata world from those specifications. The number of cells in the world based on the width and height does not equal the cells provided.");
            }

            CellularAutomataWorld caw = new CellularAutomataWorld();
            caw.Cells = cells;
            caw.WorldWidth = width;
            caw.WorldHeight = height;

            return caw;
        }

        public void RandomizeCells()
        {
            Random r = new Random();
            int t = 0;
            for (t = 0; t < Cells.Length; t++)
            {
                int rn = r.Next(0, 2);
                if (rn == 0)
                {
                    Cells[t] = false;
                }
                else
                {
                    Cells[t] = true;
                }
            }
        }

        public string Print(string AliveSymbol = "1", string DeadSymbol = "0", string colDelimiter = ",", string rowDelimiter = "\n")
        {
            int r = 0;
            int c = 0;
            int onBool = 0;

            string ToReturn = "";
            for (r = 0; r < WorldHeight; r++)
            {
                string ThisColumn = "";
                for (c = 0; c < WorldWidth; c++)
                {
                    string tofillin = "";
                    if (Cells[onBool])
                    {
                        tofillin = AliveSymbol;
                    }
                    else
                    {
                        tofillin = DeadSymbol;
                    }
                    ThisColumn = ThisColumn + tofillin + colDelimiter;
                    onBool = onBool + 1;
                }
                ThisColumn = ThisColumn.Substring(0, ThisColumn.Length - 1);
                ToReturn = ToReturn + ThisColumn + rowDelimiter;
            }
            ToReturn = ToReturn.Substring(0, ToReturn.Length - 1);

            return ToReturn;
        }

        public void NextGeneration(int KillBelow = 2, int KillAbove = 3, int BirthAt = 3)
        {
            List<bool> NewCells = new List<bool>();
            int t = 0;
            for (t = 0; t < Cells.Length; t++)
            {
                bool ToAdd = false;
                int live_neighbors = CountLiveNeighbors(t);
                if (Cells[t] == true)
                {
                    if (live_neighbors < KillBelow)
                    {
                        ToAdd = false;
                    }

                    if (live_neighbors >= KillBelow && live_neighbors <= KillAbove)
                    {
                        ToAdd = true;
                    }

                    if (live_neighbors > KillAbove)
                    {
                        ToAdd = false;
                    }
                }
                else
                {
                    if (live_neighbors == BirthAt)
                    {
                        ToAdd = true;
                    }
                }
                NewCells.Add(ToAdd);
            }
            Cells = NewCells.ToArray();
        }

        

        #region "Utility functions"
        public int CountLiveNeighbors(int index)
        {
            int ToReturn = 0;

            //  neighbor format
            //  1 2 3
            //  8 X 4
            //  7 6 5

            int Index1 = GetIndexLeft(GetIndexAbove(index));
            int Index2 = GetIndexAbove(index);
            int Index3 = GetIndexRight(GetIndexAbove(index));
            int Index4 = GetIndexRight(index);
            int Index5 = GetIndexRight(GetIndexBelow(index));
            int Index6 = GetIndexBelow(index);
            int Index7 = GetIndexLeft(GetIndexBelow(index));
            int Index8 = GetIndexLeft(index);

            if (Cells[Index1])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index2])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index3])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index4])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index5])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index6])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index7])
            {
                ToReturn = ToReturn + 1;
            }
            if (Cells[Index8])
            {
                ToReturn = ToReturn + 1;
            }

            return ToReturn;

        }





        //Naigation below
        private void ValidateIndex(int index)
        {
            if (index < 0 || index > (Cells.Length - 1))
            {
                throw new Exception("Cell index is outside the realms of this world.");
            }
        }

        private bool IsInTopBorder(int index)
        {
            ValidateIndex(index);
            if (index < WorldWidth)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsInBottomBorder(int index)
        {
            ValidateIndex(index);
            int maxind = WorldWidth * (WorldHeight - 1);
            if (index >= maxind)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsInLeftBorder(int index)
        {
            ValidateIndex(index);
            float div = (float)index / (float)WorldWidth;
            if (div.ToString().Contains(".") == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsInRightBorder(int index)
        {
            ValidateIndex(index);
            float div = (float)(index + 1) / (float)WorldWidth;
            if (div.ToString().Contains("."))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool GetCellAbove(int index)
        {
            ValidateIndex(index);
            if (IsInTopBorder(index) == false)
            {
                return Cells[index - WorldWidth];
            }
            else
            {
                throw new Exception("Unable to return cell above.  Indexed cell is already in the top row.");
            }
        }

        private bool GetCellBelow(int index)
        {
            ValidateIndex(index);
            if (IsInBottomBorder(index) == false)
            {
                return Cells[index + WorldWidth];
            }
            else
            {
                throw new Exception("Unable to returnn cell below.  Indexed cell is already in the bottom row.");
            }
        }

        private bool GetCellLeft(int index)
        {
            ValidateIndex(index);
            if (IsInLeftBorder(index) == false)
            {
                return Cells[index - 1];
            }
            else
            {
                throw new Exception("Unable to get cell to the left.  Indexed cell is part of the left-most column.");
            }
        }

        private bool GetCellRight(int index)
        {
            ValidateIndex(index);
            if (IsInRightBorder(index) == false)
            {
                return Cells[index + 1];
            }
            else
            {
                throw new Exception("Unable to get cell to the right.  Indexed cell is part of the right-most column.");
            }
        }

        //Getting indexs

        private int GetIndexAbove(int index)
        {
            ValidateIndex(index);
            if (IsInTopBorder(index) == false)
            {
                return index - WorldWidth;
            }
            else
            {
                int ind = index + (WorldWidth * (WorldHeight - 1));
                return ind;
            }
        }

        private int GetIndexRight(int index)
        {
            if (IsInRightBorder(index) == false)
            {
                return index + 1;
            }
            else
            {
                int ind = index - WorldWidth + 1;
                return ind;
            }
        }

        private int GetIndexLeft(int index)
        {
            if (IsInLeftBorder(index) == false)
            {
                return index - 1;
            }
            else
            {
                int ind = index + WorldWidth - 1;
                return ind;
            }
        }

        private int GetIndexBelow(int index)
        {
            if (IsInBottomBorder(index) == false)
            {
                return index + WorldWidth;
            }
            else
            {
                int ind = index - (WorldWidth * (WorldHeight - 1));
                return ind;
            }
        }

        #endregion





    }
}