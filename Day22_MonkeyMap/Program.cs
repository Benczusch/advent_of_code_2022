using System.Runtime.CompilerServices;
using System.Text;

bool testData = false;
bool secondTask = true;

Dictionary<Facing, (int, int)> Directions = new Dictionary<Facing, (int, int)>()
{
    { Facing.Right, (0, 1) },
    { Facing.Down, (1, 0) },
    { Facing.Left, (0, -1) },
    { Facing.Up, (-1, 0) },
};
Dictionary<Facing, string> FacingNames = new Dictionary<Facing, string>()
{
    { Facing.Right, "Right" },
    { Facing.Down, "Down" },
    { Facing.Left, "Left" },
    { Facing.Up, "Up" },
};
Dictionary<Facing, char> FacingChars = new Dictionary<Facing, char>()
{
    { Facing.Right, '>' },
    { Facing.Down, 'v' },
    { Facing.Left, '<' },
    { Facing.Up, '^' },
};

List<string> mapLines = new List<string>();

StreamReader streamReader = new StreamReader(testData ? "testInput.txt" : "input.txt");
bool goOn = true;
while (goOn)
{
    string line = streamReader.ReadLine();
    if (line.Length > 0)
    {
        mapLines.Add(line);
    }
    else
    {
        goOn = false;
    }
}
string pathLine = streamReader.ReadLine();

int sideLength = testData ? mapLines.Count/3 : mapLines.Count / 4;

int Rows = mapLines.Count;
int Cols = mapLines.Max(x =>x.Length);

Tile[,] map = new Tile[Rows, Cols];
char[,] charMap = new char[Rows, Cols];

for (int row = 0; row < Rows; row++)
{
    for (int col = 0; col < Cols; col++)
    {
        if (col < mapLines[row].Length)
        {
            charMap[row, col] = mapLines[row][col];
            switch (mapLines[row][col])
            {
                case ' ':
                    map[row, col] = Tile.ForceField;
                    break;
                case '.':
                    map[row, col] = Tile.Nothing;
                    break;
                case '#':
                    map[row, col] = Tile.Wall;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        else
        {
            map[row, col] = Tile.ForceField;
            charMap[row, col] = ' ';
        }
    }
}

(int, int) position = (0, 0);
while (map[position.Item1, position.Item2] != Tile.Nothing)
{
    position.Item2++;
}

StringBuilder numberBuilder = new StringBuilder();
Facing currentFacing = Facing.Right;


for (int i = 0; i <= pathLine.Length; i++)
{
    if (i >= pathLine.Length || pathLine[i] == 'R' || pathLine[i] == 'L')
    {
        (int, int) direction = Directions[currentFacing];
        if (numberBuilder.Length == 0)
            numberBuilder.Append('0');
        int steps = int.Parse(numberBuilder.ToString());

        Move(steps, direction, ref position);
        Console.Write($"Moved {steps} {FacingNames[currentFacing]} - ");
        Console.WriteLine($"Row: {position.Item1}, Column: {position.Item2}");
        numberBuilder.Clear();

        if (i >= pathLine.Length)
        {
            continue;
        }

        if (pathLine[i] == 'R')
        {
            currentFacing = (Facing)(((int) currentFacing + 1) % 4);
        }
        else
        {
            currentFacing = (Facing)(((int)currentFacing + 3) % 4);
        }
    }
    else
    {
        numberBuilder.Append(pathLine[i]);
    }
}

Console.WriteLine(1000 * (position.Item1 + 1) + 4 * (position.Item2 + 1) + currentFacing);

StreamWriter sW = new StreamWriter("lastCharMap.txt");
for (int row = 0; row < Rows; row++)
{
    for (int col = 0; col < Cols; col++)
    {
        sW.Write(charMap[row, col]);
    }
    sW.WriteLine();
}
sW.Close();

void Move(int steps, (int, int) direction, ref (int, int) position)
{
    for (int j = 0; j < steps; j++)
    {
        charMap[position.Item1, position.Item2] = FacingChars[currentFacing];

        int newRow = position.Item1 + direction.Item1;
        int newCol = position.Item2 + direction.Item2;
        if (!InBoundary(newRow, newCol))
        {
            (int, int, Facing) newPos = FirstFromSide(direction, newRow, newCol);
            if (position.Item1 == newPos.Item1 && position.Item2 == newPos.Item2)
            {
                return;
            }
            position.Item1 = newPos.Item1;
            position.Item2 = newPos.Item2;
            currentFacing = newPos.Item3;
            direction = Directions[currentFacing];
            continue;
        }
        switch (map[newRow, newCol])
        {
            case Tile.Wall:
                return;
            case Tile.ForceField:
                (int, int, Facing) newPos = FirstFromSide(direction, newRow, newCol);
                if (position.Item1 == newPos.Item1 && position.Item2 == newPos.Item2)
                {
                    return;
                }
                position.Item1 = newPos.Item1;
                position.Item2 = newPos.Item2;
                currentFacing = newPos.Item3;
                direction = Directions[currentFacing];
                break;
            case Tile.Nothing:
                position = (newRow, newCol);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

(int, int, Facing) FirstFromSide((int, int) direction, int row, int col)
{
    int nRow = row;
    int nCol = col;
    Facing nFacing = currentFacing;
    if (secondTask)
    {
        if (testData)
        {
            HandleTestDataFaceChange(ref nRow, ref nCol, ref nFacing);
        }
        else
        {
            HandleRealDataFaceChange(ref nRow, ref nCol, ref nFacing);
        }
    }
    else
    {

        while (InBoundary(nRow, nCol))
        {
            nRow += direction.Item1;
            nCol += direction.Item2;
        }

        nRow += Rows;
        nCol += Cols;
        nRow %= Rows;
        nCol %= Cols;

        while (map[nRow, nCol] == Tile.ForceField)
        {
            nRow += direction.Item1;
            nCol += direction.Item2;
        }
    }

    if (map[nRow, nCol] == Tile.Nothing)
    {
        return (nRow, nCol, nFacing);
    }
    else if (map[nRow, nCol] == Tile.Wall)
    {
        return (row - direction.Item1, col - direction.Item2, currentFacing);
    }

    return (0, 0, Facing.Right);
}

void HandleTestDataFaceChange(ref int nRow, ref int nCol, ref Facing nFacing)
{
    switch (currentFacing)
    {
        case Facing.Right:
            switch (nRow / sideLength)
            {
                case 0:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = 4 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
                case 1:
                    nCol = 5 * sideLength - 1 - nRow;
                    nRow = 2 * sideLength;
                    nFacing = Facing.Down;
                    break;
                case 2:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = 3 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
            }
            break;
        case Facing.Down:
            switch (nCol / sideLength)
            {
                case 0:
                    nCol = 3 * sideLength - 1 - nCol;
                    nRow = 3 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
                case 1:
                    nRow = 3 * sideLength - 1 - nCol;
                    nCol = sideLength * 2;
                    nFacing = Facing.Right;
                    break;
                case 2:
                    nCol = 3 * sideLength - 1 - nCol;
                    nRow = 2 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
                case 3:
                    nRow = 5 * sideLength - nCol;
                    nCol = 0;
                    nFacing = Facing.Right;
                    break;
            }
            break;
        case Facing.Left:
            switch (nRow / sideLength)
            {
                case 0:
                    nCol = sideLength + nRow;
                    nRow = sideLength;
                    nFacing = Facing.Down;
                    break;
                case 1:
                    nCol = 5 * sideLength - nRow - 1;
                    nRow = 3 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
                case 2:
                    nCol = 4 * sideLength - 1 - nRow;
                    nRow = 3 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
            }
            break;
        case Facing.Up:
            switch (nCol / sideLength)
            {
                case 0:
                    nCol = 3 * sideLength - 1 - nCol;
                    nRow = 0;
                    nFacing = Facing.Down;
                    break;
                case 1:
                    nRow = nCol - sideLength;
                    nCol = 2 * sideLength;
                    nFacing = Facing.Right;
                    break;
                case 2:
                    nCol = 3*sideLength - 1 - nCol;
                    nRow = 2 * sideLength;
                    nFacing = Facing.Down;
                    break;
                case 3:
                    nRow = 4 * sideLength - 1 - nCol;
                    nCol = 3 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
            }
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}

void HandleRealDataFaceChange(ref int nRow, ref int nCol, ref Facing nFacing)
{
    switch (currentFacing)
    {
        case Facing.Right:
            switch (nRow / sideLength)
            {
                case 0:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = 2 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
                case 1:
                    nCol = nRow + sideLength;
                    nRow = sideLength - 1;
                    nFacing = Facing.Up;
                    break;
                case 2:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = 3 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
                case 3:
                    nCol = nRow - 2 * sideLength;
                    nRow = 3 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
            }
            break;
        case Facing.Down:
            switch (nCol / sideLength)
            {
                case 0:
                    nCol = nCol + 2 * sideLength;
                    nRow = 0;
                    nFacing = Facing.Down;
                    break;
                case 1:
                    nRow = nCol + 2 * sideLength;
                    nCol = sideLength - 1;
                    nFacing = Facing.Left;
                    break;
                case 2:
                    nRow = nCol - sideLength;
                    nCol = 2 * sideLength - 1;
                    nFacing = Facing.Left;
                    break;
            }
            break;
        case Facing.Left:
            switch (nRow / sideLength)
            {
                case 0:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = 0;
                    nFacing = Facing.Right;
                    break;
                case 1:
                    nCol = nRow - sideLength;
                    nRow = 2 * sideLength;
                    nFacing = Facing.Down;
                    break;
                case 2:
                    nRow = 3 * sideLength - 1 - nRow;
                    nCol = sideLength;
                    nFacing = Facing.Right;
                    break;
                case 3:
                    nCol = nRow - 2 * sideLength;
                    nRow = 0;
                    nFacing = Facing.Down;
                    break;
            }
            break;
        case Facing.Up:
            switch (nCol / sideLength)
            {
                case 0:
                    nRow = nCol + sideLength;
                    nCol = sideLength;
                    nFacing = Facing.Right;
                    break;
                case 1:
                    nRow = nCol + 2 * sideLength;
                    nCol = 0;
                    nFacing = Facing.Right;
                    break;
                case 2:
                    nCol = nCol - 2 * sideLength;
                    nRow = 4 * sideLength - 1;
                    nFacing = Facing.Up;
                    break;
            }
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
}

bool InBoundary(int newRow, int newCol)
{
    return newRow >= 0 && newCol >= 0 && newRow < Rows && newCol < Cols;
}

enum Tile
{
    Wall,
    ForceField,
    Nothing
}

enum Facing
{
    Right = 0,
    Down = 1,
    Left = 2,
    Up = 3,
}

