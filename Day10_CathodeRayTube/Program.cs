using System.Numerics;

StreamReader streamReader = new StreamReader("input.txt");

List<Command> commands = new List<Command>();

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();

    if (line.StartsWith("noop"))
    {
        commands.Add(new Command(){number = 0, type = Command.CommandType.Skip});
    }

    if (line.StartsWith("addx"))
    {
        string[] split = line.Split(' ');
        commands.Add(new Command() { number = 0, type = Command.CommandType.Skip });
        commands.Add(new Command() { number = int.Parse(split[1]), type = Command.CommandType.Add });
    }
}

int register = 1;
BigInteger result = 0;

for (int cycle = 0; cycle < commands.Count; cycle++)
{
    switch (commands[cycle].type)
    {
        case Command.CommandType.Skip:
            break;
        case Command.CommandType.Add:
            register += commands[cycle].number;
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    if ((cycle - 18) % 40 == 0)
    {
        result += (cycle + 2) * register;
    }
}

Console.WriteLine(result);

register = 1;
bool[] pixels = new bool[40];

for (int i = 0; i < commands.Count; i++)
{
    int cursor = i % 40;

    pixels[cursor] = register - 1 <= cursor && register + 1 >= cursor;
    switch (commands[i].type)
    {
        case Command.CommandType.Skip:
            break;
        case Command.CommandType.Add:
            register += commands[i].number;
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }


    if (cursor == 39)
    {
        for (int j = 0; j < pixels.Length; j++)
        {
            Console.Write(pixels[j]?'#':'.');
        }
        Console.WriteLine();
        pixels = new bool[40];
    }
}



class Command
{
    public int number;
    public CommandType type;

    public enum CommandType
    {
        Skip,
        Add
    }
}