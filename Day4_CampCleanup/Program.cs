StreamReader streamReader = new StreamReader("input.txt");

int inclusionCount = 0;
int overlapCount = 0;

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();
    var pieces = line.Split(new []{',', '-'});

    (int, int) firstinterval = (int.Parse(pieces[0]), int.Parse(pieces[1]));
    (int, int) secondinterval = (int.Parse(pieces[2]), int.Parse(pieces[3]));

    if (firstinterval.Item1 <= secondinterval.Item1 && firstinterval.Item2>= secondinterval.Item2
        || firstinterval.Item1 >= secondinterval.Item1 && firstinterval.Item2 <= secondinterval.Item2)
    {
        inclusionCount++;
    }

    if (secondinterval.Item1 <= firstinterval.Item2 && secondinterval.Item1 >= firstinterval.Item1 
        || firstinterval.Item1 <= secondinterval.Item2 && firstinterval.Item1 >= secondinterval.Item1)
    {
        overlapCount++;
    }
}

Console.WriteLine(inclusionCount);
Console.WriteLine(overlapCount);