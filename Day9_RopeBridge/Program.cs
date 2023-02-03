StreamReader streamReader = new StreamReader("input.txt");
List<string> commands = new List<string>();
while (!streamReader.EndOfStream)
{
    commands.Add(streamReader.ReadLine());
}

List<Snapshot> snapshots = new List<Snapshot>();
Snapshot lastSnapshot = new Snapshot();
snapshots.Add(lastSnapshot);

foreach (string command in commands)
{
    var line = command.Split(' ');
    char direction = line[0][0];
    int times = int.Parse(line[1]);

    for (int i = 0; i < times; i++)
    {
        lastSnapshot = new Snapshot(lastSnapshot, direction);
        snapshots.Add(lastSnapshot);
    }
}

Console.WriteLine(lastSnapshot.Positions.Count);

List<LargeRopeSnapshot> largeSnapshots = new List<LargeRopeSnapshot>();
LargeRopeSnapshot lastLargeSnapshot = new LargeRopeSnapshot();
largeSnapshots.Add(lastLargeSnapshot);

foreach (string command in commands)
{
    var line = command.Split(' ');
    char direction = line[0][0];
    int times = int.Parse(line[1]);

    for (int i = 0; i < times; i++)
    {
        lastLargeSnapshot = new LargeRopeSnapshot(lastLargeSnapshot, direction);
        largeSnapshots.Add(lastLargeSnapshot);
    }
}

Console.WriteLine(lastLargeSnapshot.Positions.Count);

Console.WriteLine();

public class LargeRopeSnapshot
{
    public Position Head;
    public List<Position> Tails;
    public int NumberOfTails = 9;

    public List<Position> Positions = new List<Position>();

    public LargeRopeSnapshot()
    {
        Head = new Position(0, 0);
        Tails = new List<Position>();
        for (int i = 0; i < NumberOfTails; i++)
        {
            Tails.Add(new Position(0, 0));
        }
    }

    public LargeRopeSnapshot(LargeRopeSnapshot previous, char movement)
    {
        Head = new Position(previous.Head);
        Tails = new List<Position>();
        foreach (var tail in previous.Tails)
        {
            Tails.Add(new Position(tail));
        }

        switch (movement)
        {
            case 'U':
                Head.Y++;
                break;
            case 'D':
                Head.Y--;
                break;
            case 'L':
                Head.X--;
                break;
            case 'R':
                Head.X++;
                break;
        }

        for (int i = 0; i < NumberOfTails; i++)
        {
            Position first = i > 0 ? Tails[i - 1] : Head;
            Position second = Tails[i];
            if (!DoTouch(i))
            {
                Position tailMovement = new Position(0, 0);
                tailMovement.X = Math.Sign(first.X - second.X);
                tailMovement.Y = Math.Sign(first.Y - second.Y);
                second.X += tailMovement.X;
                second.Y += tailMovement.Y;
            }
        }

        Positions.AddRange(previous.Positions);
        if (!Positions.Contains(Tails.Last()))
        {
            Positions.Add(Tails.Last());
        }
    }

    public bool DoTouch(int numTail)
    {
        bool touch = true;
        Position first = numTail > 0 ? Tails[numTail - 1] : Head;
        Position second = Tails[numTail];
        touch &= first.X - second.X >= -1;
        touch &= first.X - second.X <= 1;
        touch &= first.Y - second.Y >= -1;
        touch &= first.Y - second.Y <= 1;
        return touch;
    }
}

public class Snapshot
{
    public Position Head;
    public Position Tail;

    public List<Position> Positions = new List<Position>();

    public Snapshot()
    {
        Head = new Position(0, 0);
        Tail = new Position(0, 0);
    }

    public Snapshot(Snapshot previous, char movement)
    {
        Head = new Position(previous.Head);
        Tail = new Position(previous.Tail);

        switch (movement)
        {
            case 'U':
                Head.Y++;
                break;
            case 'D':
                Head.Y--;
                break;
            case 'L':
                Head.X--;
                break;
            case 'R':
                Head.X++;
                break;
        }

        if (!DoTouch())
        {
            Position tailMovement = new Position(0, 0);
            tailMovement.X = Math.Sign(Head.X - Tail.X);
            tailMovement.Y = Math.Sign(Head.Y - Tail.Y);
            Tail.X += tailMovement.X;
            Tail.Y += tailMovement.Y;
        }

        Positions.AddRange(previous.Positions);
        if (!Positions.Contains(Tail))
        {
            Positions.Add(Tail);
        }
    }

    public bool DoTouch()
    {
        bool touch = true;
        touch &= Head.X - Tail.X >= -1;
        touch &= Head.X - Tail.X <= 1;
        touch &= Head.Y - Tail.Y >= -1;
        touch &= Head.Y - Tail.Y <= 1;
        return touch;
    }
}

public class Position
{
    public int X;
    public int Y;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Position(Position pos)
    {
        X = pos.X;
        Y = pos.Y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Position p)
        {
            return p.X == X && p.Y == Y;
        }
        return base.Equals(obj);
    }
}