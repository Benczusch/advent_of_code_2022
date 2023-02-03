// See https://aka.ms/new-console-template for more information

using System.Data;

const int EMPTY = 0;
const int ROCK = 1;
const int SAND = 2;

Console.WriteLine("Hello, World!");

StreamReader streamReader = new StreamReader("input.txt");

bool secondTask = true;

int xMin = 9999;
int xMax = 0;
int yMin = 9999;
int yMax = 0;

List<List<(int, int)>> allPaths = new List<List<(int, int)>>();

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();
    var pieces = line.Split(" -> ");
    List<(int, int)> path = new List<(int, int)>();
    foreach (string piece in pieces)
    {
        var coordinates = piece.Split(',');
        int x = int.Parse(coordinates[0]);
        int y = int.Parse(coordinates[1]);
        #region Minmax
        if (x < xMin)
        {
            xMin = x;
        }
        if (x > xMax)
        {
            xMax = x;
        }
        if (y < yMin)
        {
            yMin = y;
        }
        if (y > yMax)
        {
            yMax = y;
        }
        #endregion

        path.Add((x, y));
    }
    allPaths.Add(path);
}

xMin--;
xMax++;

if (secondTask)
{
    yMax += 2;
    xMin = 500 - yMax - 2;
    xMax = 500 + yMax + 2;
}

Console.WriteLine($"X : {xMin} - {xMax}");
Console.WriteLine($"Y : {yMin} - {yMax}");

foreach (List<(int, int)> path in allPaths)
{
    for (int i = 0; i < path.Count; i++)
    {
        path[i] = (path[i].Item1 - xMin, path[i].Item2);
    }
}

xMax -= xMin;

int[,] map = new int[xMax + 1, yMax + 1];

foreach (var path in allPaths)
{
    for (int i = 0; i < path.Count - 1; i++)
    {
        int speedX = path[i + 1].Item1 - path[i].Item1;
        int speedY = path[i + 1].Item2 - path[i].Item2;

        int steps = Math.Max(Math.Abs(speedY), Math.Abs(speedX));

        speedX = Math.Sign(speedX);
        speedY = Math.Sign(speedY);

        for (int j = 0; j <= steps; j++)
        {
            map[path[i].Item1 + j * speedX, path[i].Item2 + j * speedY] = ROCK;
        }
    }
}

if (secondTask)
{
    for (int i = 0; i < xMax; i++)
    {
        map[i, yMax] = ROCK;
    }
}

bool fallout = false;
int pourX = 500 - xMin;
int pourY = 0;

int sandCount = 0;

while (!fallout && map[pourX, pourY] != SAND)
{
    int sandX = pourX;
    int sandY = pourY;

    bool moved = true;

    while (moved)
    {
        moved = false;

        if (map[sandX, sandY + 1] == EMPTY)
        {
            sandY++;
            moved = true;
        }
        else if (map[sandX - 1, sandY + 1] == EMPTY)
        {
            sandX--;
            sandY++;
            moved = true;
        }
        else if (map[sandX + 1, sandY + 1] == EMPTY)
        {
            sandX++;
            sandY++;
            moved = true;
        }
        else
        {
            map[sandX, sandY] = SAND;
        }

        if (sandY == yMax)
        {
            fallout = true;
            break;
        }
    }
    if (!fallout)
        sandCount++;
}

for (int y = 0; y < yMax + 1; y++)
{
    for (int x = 0; x < xMax + 1; x++)
    {
        switch (map[x,y])
        {
            case 0:
                Console.Write('.');
                break;
            case ROCK:
                Console.Write('#');
                break;
            case SAND:
                Console.Write('O');
                break;
            default:
                break;
        }
    }
    Console.WriteLine();
}

Console.WriteLine($"Sand count: {sandCount}");