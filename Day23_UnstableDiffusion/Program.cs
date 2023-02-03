using System.Diagnostics.CodeAnalysis;

StreamReader streamReader = new StreamReader("input.txt");

List<Elf> elves = new List<Elf>();
HashSet<(int, int)> CurrentPositions = new HashSet<(int, int)>();

int row = 0;

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();

    for (int col = 0; col < line.Length; col++)
    {
        if (line[col] == '#')
        {
            elves.Add(new Elf(row, col));
            CurrentPositions.Add((row, col));
        }
    }

    row++;
}

Queue<Direction> ProposedDirections = new Queue<Direction>();
ProposedDirections.Enqueue(Direction.North);
ProposedDirections.Enqueue(Direction.South);
ProposedDirections.Enqueue(Direction.West);
ProposedDirections.Enqueue(Direction.East);


Dictionary<Direction, (int, int)> DirectionVectors = new Dictionary<Direction, (int, int)>
{
    {Direction.North, (-1, 0)},
    {Direction.NorthWest, (-1, -1)},
    {Direction.West, (0, -1)},
    {Direction.SouthWest, (1, -1)},
    {Direction.South, (1, 0)},
    {Direction.SouthEast, (1, 1)},
    {Direction.East, (0, 1)},
    {Direction.NorthEast, (-1, 1)},
};

Dictionary<Direction, List<Direction>> positionsToCheck = new Dictionary<Direction, List<Direction>>()
{
    {Direction.North, new List<Direction>() {Direction.North, Direction.NorthEast, Direction.NorthWest}},
    {Direction.South, new List<Direction>() {Direction.South, Direction.SouthEast, Direction.SouthWest}},
    {Direction.West, new List<Direction>() {Direction.West, Direction.SouthWest, Direction.NorthWest}},
    {Direction.East, new List<Direction>() {Direction.East, Direction.NorthEast, Direction.SouthEast}},
};
Dictionary<(int, int), Elf> ProposedPositions = new Dictionary<(int, int), Elf>();

bool someOneMoved = true;
int rounds = 0;

while(someOneMoved)
{
    someOneMoved = false;
    rounds++;

    ProposedPositions.Clear();
    foreach (Elf elf in elves)
    {
        if (!elf.DoesMove(CurrentPositions))
        {
            continue;
        }

        someOneMoved = true;
        foreach (Direction proposedDirection in ProposedDirections)
        {
            if (CheckPosition(proposedDirection, elf))
            {
                int proposedRow = elf.Position.Item1 + DirectionVectors[proposedDirection].Item1;
                int proposedCol = elf.Position.Item2 + DirectionVectors[proposedDirection].Item2;
                elf.ProposedPosition = (proposedRow, proposedCol);
                if (ProposedPositions.ContainsKey(elf.ProposedPosition))
                {
                    var proposingElf = ProposedPositions[elf.ProposedPosition];
                    proposingElf.ProposedPosition = (proposingElf.Position.Item1, proposingElf.Position.Item2);
                    elf.ProposedPosition = (elf.Position.Item1, elf.Position.Item2);
                }
                else
                {
                    ProposedPositions.Add(elf.ProposedPosition, elf);
                }
                break;
            }
        }
    }

    CurrentPositions.Clear();

    foreach (var elf in elves)
    {
        CurrentPositions.Add(elf.ProposedPosition);
        elf.Position = (elf.ProposedPosition.Item1, elf.ProposedPosition.Item2);
    }

    ProposedDirections.Enqueue(ProposedDirections.Dequeue());

    //Console.WriteLine($"Turn {i + 1} done");
    //for (int pRow = -3; pRow < 15; pRow++)
    //{
    //    for (int pCol = -3; pCol < 15; pCol++)
    //    {
    //        Console.Write(CurrentPositions.Contains((pRow, pCol))?'#':'.');
    //    }
    //    Console.WriteLine();
    //}
}

int rowMin = int.MaxValue;
int colMin = int.MaxValue;
int rowMax = int.MinValue;
int colMax = int.MinValue;

foreach ((int, int) elfPosition in CurrentPositions)
{
    rowMin = Math.Min(rowMin, elfPosition.Item1);
    rowMax = Math.Max(rowMax, elfPosition.Item1);
    colMin = Math.Min(colMin, elfPosition.Item2);
    colMax = Math.Max(colMax, elfPosition.Item2);
}

Console.WriteLine($"After {rounds} rounds there are {(rowMax - rowMin + 1) * (colMax - colMin + 1) - CurrentPositions.Count} empty places");

bool CheckPosition(Direction direction, Elf elf)
{
    foreach (Direction checkDirection in positionsToCheck[direction])
    {
        int row = elf.Position.Item1 + DirectionVectors[checkDirection].Item1;
        int col = elf.Position.Item2 + DirectionVectors[checkDirection].Item2;

        if (CurrentPositions.Contains((row, col)))
        {
            return false;
        }
    }

    return true;
}

public class Elf
{
    public (int, int) Position;
    public (int, int) ProposedPosition;

    public Elf(int row, int col)
    {
        Position = (row, col);
        ProposedPosition = (row, col);
    }

    public bool DoesMove(HashSet<(int, int)> currentPositons)
    {
        for (int dRow = -1; dRow <= 1; dRow++)
        {
            for (int dCol = -1; dCol <= 1; dCol++)
            {
                if ((dRow != 0 || dCol != 0) &&
                    currentPositons.Contains((this.Position.Item1 + dRow, this.Position.Item2 + dCol)))
                {
                    return true;
                }
            }
        }

        return false;
    }
}

public enum Direction
{
    North,
    South,
    West,
    East,
    NorthWest,
    SouthWest,
    NorthEast,
    SouthEast
}