StreamReader streamReader = new StreamReader("input.txt");

List<string> lines = new List<string>();

while (!streamReader.EndOfStream)
{
    lines.Add(streamReader.ReadLine());
}

int rows = lines.Count;
int cols = lines[0].Length;

int[,] treeMap = new int[rows, cols];
bool[,] visible = new bool[rows, cols];

for (int row = 0; row < rows; row++)
{
    for (int col = 0; col < cols; col++)
    {
        treeMap[row, col] = lines[row][col] - '0';
    }
}

for (int row = 0; row < rows; row++)
{
    int tallest = -1;

    for (int col = 0; col < cols; col++)
    {
        if (treeMap[row, col] > tallest)
        {
            tallest = treeMap[row, col];
            visible[row, col] = true;
        }
    }
}

for (int row = 0; row < rows; row++)
{
    int tallest = -1;

    for (int col = cols - 1; col >= 0; col--)
    {
        if (treeMap[row, col] > tallest)
        {
            tallest = treeMap[row, col];
            visible[row, col] = true;
        }
    }
}

for (int col = 0; col < cols; col++)
{
    int tallest = -1;

    for (int row = 0; row < rows; row++)
    {
        if (treeMap[row, col] > tallest)
        {
            tallest = treeMap[row, col];
            visible[row, col] = true;
        }
    }
}

for (int col = 0; col < cols; col++)
{
    int tallest = -1;

    for (int row = rows - 1; row >= 0; row--)
    {
        if (treeMap[row, col] > tallest)
        {
            tallest = treeMap[row, col];
            visible[row, col] = true;
        }
    }
}

int count = 0;

for (int row = 0; row < rows; row++)
{
    for (int col = 0; col < cols; col++)
    {
        //Console.Write(visible[row, col] ? 'O' : 'X');
        //Console.Write(' ');
        if (visible[row, col])
        {
            count++;
        }
    }
    //Console.WriteLine();
}

Console.WriteLine(count);

int[][] directions = new[]
{
    new int[] {1, 0},
    new int[] {0, 1},
    new int[] {-1, 0},
    new int[] {0, -1},
};

int[,] valueMap = new int[rows, cols];

for (int startRow = 1; startRow < rows - 1; startRow++)
{
    for (int startCol = 1; startCol < cols - 1; startCol++)
    {
        int value = 1;
        foreach (var direction in directions)
        {
            int viewingDistance = 0;
            bool goOn = true;
            int row = startRow;
            int col = startCol;
            int houseHeight = treeMap[startRow, startCol];
            do
            {
                row += direction[0];
                col += direction[1];
                if (InBoundary(treeMap, row, col))
                {
                    viewingDistance++;
                    goOn &= treeMap[row, col] < houseHeight;
                }
                else
                {
                    goOn = false;
                }
            } while (goOn);

            value *= viewingDistance;
        }
        valueMap[startRow, startCol] = value;
    }
}

int bestView = 0;

for (int row = 0; row < rows; row++)
{
    for (int col = 0; col < cols; col++)
    {
        //Console.Write($"{valueMap[row, col]:D2} ");
        if (valueMap[row, col] > bestView)
        {
            bestView = valueMap[row, col];
        }
    }
    //Console.WriteLine();
}

Console.WriteLine(bestView);

bool InBoundary(Array array, int row, int col) => row >= 0 && col >= 0 && row <array.GetLength(0) &&col < array.GetLength(1);