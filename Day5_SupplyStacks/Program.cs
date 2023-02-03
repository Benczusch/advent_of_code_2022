// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

StreamReader streamReader = new StreamReader("input.txt");
bool secondTask = true;

string line = "";
Stack<string> crateStrings = new Stack<string>();
do
{
    line = streamReader.ReadLine();
    crateStrings.Push(line);
} while (!line.Contains('1'));

int stacks = line.Last(c => c != ' ') - '0';
Stack<char>[] CrateStacks = new Stack<char>[stacks];
for (int i = 0; i < stacks; i++)
{
    CrateStacks[i] = new Stack<char>();
}

string crateLine = crateStrings.Pop();
while(crateStrings.Count > 0)
{
    crateLine = crateStrings.Pop();
    for (int j = 0; j < stacks; j++)
    {
        char crate = crateLine[1 + j * 4];
        if (crate != ' ')
        {
            CrateStacks[j].Push(crate);
        }
    }
}
line = streamReader.ReadLine();
while (!streamReader.EndOfStream)
{
    line = streamReader.ReadLine();
    var firstSplit = line.Split("from");
    var secondSplit = firstSplit[1].Split("to");

    int times = int.Parse(firstSplit[0].Substring(4));
    int from = int.Parse(secondSplit[0]);
    int to = int.Parse(secondSplit[1]);

    if (secondTask)
    {
        Stack<char> movingStack = new Stack<char>();

        for (int i = 0; i < times; i++)
        {
            movingStack.Push(CrateStacks[from - 1].Pop());
        }
        for (int i = 0; i < times; i++)
        {
            CrateStacks[to - 1].Push(movingStack.Pop());
        }
    }
    else
    {
        for (int i = 0; i < times; i++)
        {
            CrateStacks[to - 1].Push(CrateStacks[from - 1].Pop());
        }
    }
}

for (int i = 0; i < CrateStacks.Length; i++)
{
    Console.Write(CrateStacks[i].Pop());
}