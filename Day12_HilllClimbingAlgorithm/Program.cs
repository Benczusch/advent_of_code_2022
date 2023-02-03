Dictionary<Direction, int[]> Steps = new Dictionary<Direction, int[]>()
{
    {Direction.Left, new int[]{0, -1}},
    {Direction.Right, new int[]{0, 1}},
    {Direction.Up, new int[]{-1, 0}},
    {Direction.Down, new int[]{1, 0}},
}; 

StreamReader streamReader = new StreamReader("input.txt");
List<string> lines = new List<string>();

while (!streamReader.EndOfStream)
{
    lines.Add(streamReader.ReadLine());
}

int Rows = lines.Count;
int Cols = lines[0].Length;

int[,] elevationMap = new int[Rows, Cols];
int[,] shortestRoute = new int[Rows, Cols];

Queue<(int, int)> reevaluate = new Queue<(int, int)>();

int startRow = 0;
int startCol = 0;

int endRow = 0;
int endCol = 0;

for (int row = 0; row < Rows; row++)
{
    for (int col = 0; col < Cols; col++)
    {
        if (lines[row][col] == 'S')
        {
            elevationMap[row, col] = 0;
            startRow = row;
            startCol = col;
        }
        else if (lines[row][col] == 'E')
        {
            elevationMap[row, col] = 'z' - 'a';
            endRow = row;
            endCol = col;
        }
        else
        {
            elevationMap[row, col] = lines[row][col] - 'a';
        }

        shortestRoute[row, col] = 9999;
    }
}

bool again = true;
Set(endRow, endCol, 0);

while (reevaluate.Count > 0)
{
    again = false;
    var location = reevaluate.Dequeue();
    int row = location.Item1;
    int col = location.Item2;
    
            
    foreach (var step in Steps)
    {
        if (InBoundary(row + step.Value[0], col + step.Value[1]) 
            && 
            shortestRoute[row, col] > shortestRoute[row + step.Value[0], col + step.Value[1]] + 1
            &&
            elevationMap[row, col] + 1 >= elevationMap[row + step.Value[0], col + step.Value[1]])
        {
            Set(row, col, shortestRoute[row + step.Value[0], col + step.Value[1]] + 1);
        }
    }
}

Console.WriteLine(shortestRoute[startRow, startCol]);

int shortestToAnyA = 9999;

for (int row = 0; row < Rows; row++)
{
    for (int col = 0; col < Cols; col++)
    {
        if (elevationMap[row, col] == 0 && shortestRoute[row, col] < shortestToAnyA)
        {
            shortestToAnyA = shortestRoute[row, col];
        }
    }
}
Console.WriteLine(shortestToAnyA);

void Set(int row, int col, int distance)
{
    shortestRoute[row, col] = distance;
    foreach (var keyValuePair in Steps)
    {
        if (InBoundary(row + keyValuePair.Value[0], col + keyValuePair.Value[1]))
        {
            reevaluate.Enqueue((row + keyValuePair.Value[0], col + keyValuePair.Value[1]));
        }
    }
}

bool InBoundary(int row, int col)
{
    return row >= 0 && col >= 0 && row < Rows && col < Cols;
}

enum Direction
{
    Left,
    Right,
    Up,
    Down
}
