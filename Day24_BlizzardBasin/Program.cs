using System.Globalization;
using System.Text;

BlizzardStateHandler.GetState(0);

List<List<(int, int)>> paths = new List<List<(int, int)>>();
paths.Add(new List<(int, int)>(){(BlizzardStateHandler.StartPoint)});

int minute = 0;
bool[,] reachable = new bool[BlizzardState.Rows, BlizzardState.Cols];

reachable[BlizzardStateHandler.StartPoint.Item1, BlizzardStateHandler.StartPoint.Item2] = true;

for (int i = 0; i < 3; i++)
{
    while (!reachable[BlizzardStateHandler.EndPoint.Item1, BlizzardStateHandler.EndPoint.Item2] && i% 2 == 0 
           ||
           !reachable[BlizzardStateHandler.StartPoint.Item1, BlizzardStateHandler.StartPoint.Item2] && i% 2 == 1)
    {
        minute++;
        bool[,] newReachable = new bool[BlizzardState.Rows, BlizzardState.Cols];

        for (int row = 0; row < BlizzardState.Rows; row++)
        {
            for (int col = 0; col < BlizzardState.Cols; col++)
            {
                if (reachable[row, col])
                {
                    foreach (var direction in new Direction[] {Direction.East, Direction.North, Direction.West, Direction.South, Direction.Stay})
                    {
                        int nRow = row;
                        int nCol = col;

                        switch (direction)
                        {
                            case Direction.East:
                                nCol++;
                                break;
                            case Direction.North:
                                nRow--;
                                break;
                            case Direction.West:
                                nCol--;
                                break;
                            case Direction.South:
                                nRow++;
                                break;
                        }

                        if (nCol <= 0 || nCol >= BlizzardState.Cols - 1)
                        {
                            continue;
                        }

                        if (nRow <= 0 && (nRow, nCol) != BlizzardStateHandler.StartPoint)
                        {
                            continue;
                        }

                        if (nRow >= BlizzardState.Rows - 1 && (nRow, nCol) != BlizzardStateHandler.EndPoint)
                        {
                            continue;
                        }

                        newReachable[nRow, nCol] = !BlizzardStateHandler.GetState(minute).BlizzardMap[nRow, nCol];
                    }
                }
            }
        }

        for (int row = 0; row < BlizzardState.Rows; row++)
        {
            for (int col = 0; col < BlizzardState.Cols; col++)
            {
                reachable[row, col] = newReachable[row, col];
            }
        }
    }
    reachable = new bool[BlizzardState.Rows, BlizzardState.Cols];
    if (i % 2 == 0)
    {
        reachable[BlizzardStateHandler.EndPoint.Item1, BlizzardStateHandler.EndPoint.Item2] = true;
    }
    else
    {
        reachable[BlizzardStateHandler.StartPoint.Item1, BlizzardStateHandler.StartPoint.Item2] = true;
    }
}

Console.WriteLine(minute.ToString());

public static class BlizzardStateHandler
{
    private static List<BlizzardState> _blizzardStates = new List<BlizzardState>();
    public static (int, int) StartPoint;
    public static (int, int) EndPoint;

    static BlizzardStateHandler()
    {
        StreamReader streamReader = new StreamReader("input.txt");
        List<string> lines = new List<string>();
        while (!streamReader.EndOfStream)
        {
            lines.Add(streamReader.ReadLine());
        }

        _blizzardStates.Add(new BlizzardState(lines));

        StartPoint.Item1 = 0;
        EndPoint.Item1 = BlizzardState.Rows - 1;

        StartPoint.Item2 = lines.First().IndexOf('.');
        EndPoint.Item2 = lines.Last().IndexOf('.');
    }

    public static BlizzardState GetState(int minute)
    {
        while (_blizzardStates.Count < minute + 1)
        {
            _blizzardStates.Add(new BlizzardState(_blizzardStates.Last()));
        }

        return _blizzardStates[minute];
    }
}

public class BlizzardState
{
    public static int Rows;
    public static int Cols;

    public int Minute { get; set; }
    public List<Blizzard> Blizzards;
    public bool[,] BlizzardMap;

    public BlizzardState(List<string> blizzards)
    {
        Minute = 0;
        Blizzards = new List<Blizzard>();

        Rows = blizzards.Count;
        Cols = blizzards[0].Length;

        BlizzardMap = new bool[Rows, Cols];
        for (int row = 1; row < Rows - 1; row++)
        {
            for (int col = 1; col < Cols - 1; col++)
            {
                char bc = blizzards[row][col];
                switch (bc)
                {
                    case '>':
                        Blizzards.Add(new Blizzard(row, col, Direction.East));
                        break;
                    case '^':
                        Blizzards.Add(new Blizzard(row, col, Direction.North));
                        break;
                    case '<':
                        Blizzards.Add(new Blizzard(row, col, Direction.West));
                        break;
                    case 'v':
                        Blizzards.Add(new Blizzard(row, col, Direction.South));
                        break;
                    default:
                        break;
                }
            }
        }
        FillMap();
    }

    public BlizzardState(BlizzardState previousState)
    {
        Blizzards = new List<Blizzard>();
        for (int i = 0; i < previousState.Blizzards.Count; i++)
        {
            Blizzards.Add(new Blizzard(previousState.Blizzards[i]));
        }
        FillMap();
        Minute = previousState.Minute + 1;
    }

    public void FillMap()
    {
        BlizzardMap = new bool[Rows, Cols];
        foreach (Blizzard blizzard in Blizzards)
        {
            BlizzardMap[blizzard.Row, blizzard.Col] = true;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Cols; col++)
            {
                int numBlizzardsHere = Blizzards.Count(b =>b.Row == row && b.Col == col);
                if (numBlizzardsHere > 1)
                {
                    sb.Append(numBlizzardsHere.ToString());
                }
                else if (numBlizzardsHere == 1)
                {
                    switch (Blizzards.First(b =>b.Row == row && b.Col == col).Direction)
                    {
                        case Direction.East:
                            sb.Append('>');
                            break;
                        case Direction.North:
                            sb.Append('^');
                            break;
                        case Direction.West:
                            sb.Append('<');
                            break;
                        case Direction.South:
                            sb.Append('v');
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    sb.Append('.');
                }
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}

public class Blizzard
{
    public int Row { get; set; }
    public int Col { get; set; }

    public Direction Direction { get; set; }

    public Blizzard(int row, int col, Direction direction)
    {
        Row = row;
        Col = col;
        Direction = direction;
    }

    public Blizzard(Blizzard previousLocation)
    {
        Row = previousLocation.Row;
        Col = previousLocation.Col;
        Direction = previousLocation.Direction;

        switch (Direction)
        {
            case Direction.East:
                Col++;
                if (Col >= BlizzardState.Cols - 1)
                {
                    Col = 1;
                }
                break;
            case Direction.North:
                Row--;
                if (Row <= 0)
                {
                    Row = BlizzardState.Rows - 2;
                }
                break;
            case Direction.West:
                Col--;
                if (Col <= 0)
                {
                    Col = BlizzardState.Cols - 2;
                }
                break;
            case Direction.South:
                Row++;
                if (Row >= BlizzardState.Rows - 1)
                {
                    Row = 1;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum Direction
{
    East,
    North,
    West,
    South,
    Stay
}