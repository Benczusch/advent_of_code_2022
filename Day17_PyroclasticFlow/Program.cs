// Indexing: ROW / COL

using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;

List<List<(int, int)>> rockForms = new List<List<(int, int)>>();

//####

List<(int,int)> minusShape = new List<(int, int)>();
minusShape.Add((0, 0));
minusShape.Add((0, 1));
minusShape.Add((0, 2));
minusShape.Add((0, 3));
rockForms.Add(minusShape);

//.#.
//###
//.#.

List<(int, int)> plusShape = new List<(int, int)>();
plusShape.Add((0, 1));
plusShape.Add((1, 0));
plusShape.Add((1, 1));
plusShape.Add((1, 2));
plusShape.Add((2, 1));
rockForms.Add(plusShape);


//..#
//..#
//###

List<(int, int)> arrowShape = new List<(int, int)>();
arrowShape.Add((0, 0));
arrowShape.Add((0, 1));
arrowShape.Add((0, 2));
arrowShape.Add((1, 2));
arrowShape.Add((2, 2));
rockForms.Add(arrowShape);


//#
//#
//#
//#

List<(int, int)> iShape = new List<(int, int)>();
iShape.Add((0, 0));
iShape.Add((1, 0));
iShape.Add((2, 0));
iShape.Add((3, 0));
rockForms.Add(iShape);

//##
//##

List<(int, int)> squareShape = new List<(int, int)>();
squareShape.Add((0, 0));
squareShape.Add((0, 1));
squareShape.Add((1, 0));
squareShape.Add((1, 1));
rockForms.Add(squareShape);

StreamReader streamReader = new StreamReader("input.txt");
string directionsString = streamReader.ReadLine();

var directionsReel = new Reel<(int, int)>();
var rockReel = new Reel<List<(int, int)>>();

List<(int,int)> directionList = new List<(int, int)>();

foreach (char c in directionsString)
{
    switch (c)
    {
        case '<':
            directionList.Add((0, -1));
            break;
        case '>':
            directionList.Add((0, 1));
            break;
        default:
            break;
    }
}

var directionArray = directionList.ToArray();
directionsReel.Array = directionArray;
rockReel.Array = rockForms.ToArray();

List<char[]> map = new List<char[]>();

const char EMPTY = '.';
const char ROCK = '#';
const char BOTTOM = '-';
bool collide;

const int WIDTH = 7;

const string EMPTYROW = ".......";

map.Add("-------".ToCharArray());

int topRock = 0;

int leadingRounds = 1000000;

DoRounds(leadingRounds);

int leadingHeight = topRock;

int cycleLength = 0;
int directionPhase = directionsReel.Index;

do
{
    DoRounds(rockForms.Count);
    cycleLength += rockForms.Count;
} while (directionsReel.Index != directionPhase);

Console.WriteLine($"Cycle start height: {leadingHeight}, batch size: {cycleLength}");

Console.WriteLine($"1. batch: {topRock}");
long perBatchHeight = topRock - leadingHeight;

    long remainder = (1_000_000_000_000 - leadingRounds) % cycleLength;
int heightBeforeRemainder = topRock;
DoRounds(remainder);
int remainderHeight = topRock - heightBeforeRemainder;
Console.WriteLine($"Extra height during remainder: {remainderHeight}");

Console.WriteLine(leadingHeight + ((1_000_000_000_000 - leadingRounds) / cycleLength) * perBatchHeight + remainderHeight);

void DoRounds(long n)
{
    for (int nRock = 0; nRock < n; nRock++)
    {
        int targetHeight = topRock + 4;

        while (map.Count <= targetHeight + 4)
        {
            map.Add(EMPTYROW.ToCharArray());
        }

        var nextRock = rockReel.Next();

        (int, int) basePosition = (targetHeight, 2);
        bool stops = false;
        bool moveRight = true;

        while (!stops)
        {
            (int, int) direction;
            if (moveRight)
            {
                direction = directionsReel.Next();
            }
            else
            {
                direction = (-1, 0);
            }

            (int, int) newPosition = (basePosition.Item1 + direction.Item1, basePosition.Item2 + direction.Item2);

            collide = false;

            foreach ((int, int) rockPosition in nextRock)
            {
                int row = newPosition.Item1 + rockPosition.Item1;
                int col = newPosition.Item2 + rockPosition.Item2;

                collide |= !InBoundary(col);
                if (!collide)
                {
                    collide |= map[row][col] != EMPTY;
                }
            }

            if (collide)
            {
                if (!moveRight)
                {
                    stops = true;
                    foreach ((int, int) rockPosition in nextRock)
                    {
                        int row = basePosition.Item1 + rockPosition.Item1;
                        int col = basePosition.Item2 + rockPosition.Item2;

                        map[row][col] = ROCK;
                        if (row > topRock)
                        {
                            topRock = row;
                        }
                    }
                }
            }
            else
            {
                basePosition.Item1 = newPosition.Item1;
                basePosition.Item2 = newPosition.Item2;
            }

            moveRight = !moveRight;
        }
    }
}
bool InBoundary(int col)
{
    return col >= 0 && col < 7;
}

string MapToString()
{
    StringBuilder sb = new StringBuilder();
    for (int row = map.Count - 1; row >= 0; row--)
    {
        foreach (char c in map[row])
        {
            sb.Append(c);
        }

        sb.AppendLine();
    }

    return sb.ToString();
}

class Reel<T>
{
    public T[] Array;
    private int _arrayLength => Array.Length;
    public int Index { get; private set; } = 0;

    public bool Around => Index == 0;
    
    public T Next()
    {
        var retVal = Array[Index];
        Index++;
        Index %= _arrayLength;
        return retVal;
    }
}