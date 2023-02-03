// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

StreamReader streamReader = new StreamReader("input.txt");

int priorities = 0;
List<string> lines = new List<string>();

while (!streamReader.EndOfStream)
{
    lines.Add(streamReader.ReadLine());
}

foreach (var inputLine in lines)
{
    
    bool[,] seen = new bool[52, 2];

    string firstCompartment = inputLine.Substring(0, inputLine.Length / 2);
    string secondCompartment = inputLine.Substring(inputLine.Length / 2);

    foreach (char c in firstCompartment)
    {
        seen[Priority(c) - 1, 0] = true;
    }
    foreach (char c in secondCompartment)
    {
        seen[Priority(c) - 1, 1] = true;
    }

    for (int i = 0; i < 52; i++)
    {
        if (seen[i, 0] && seen[i, 1])
        {
            priorities += i + 1;
        }
    }
}

Console.WriteLine(priorities);

priorities = 0;

for (int i = 0; i < lines.Count / 3; i++)
{
    bool[,] seen = new bool[52, 3];

    string firstElf = lines[i * 3];
    string secondElf = lines[i * 3 + 1];
    string thirdElf = lines[i * 3 + 2];
    foreach (char c in firstElf)
    {
        seen[Priority(c) - 1, 0] = true;
    }
    foreach (char c in secondElf)
    {
        seen[Priority(c) - 1, 1] = true;
    }
    foreach (char c in thirdElf)
    {
        seen[Priority(c) - 1, 2] = true;
    }
    for (int j = 0; j < 52; j++)
    {
        if (seen[j, 0] && seen[j, 1] && seen[j, 2])
        {
            priorities += j + 1;
        }
    }
}

Console.WriteLine(priorities);

int Priority(char c)
{
    if (c >= 'a' && c <= 'z')
    {
        return c - 'a' + 1;
    }

    return c - 'A' + 27;
}
