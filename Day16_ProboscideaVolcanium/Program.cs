DateTime start = DateTime.Now;

StreamReader streamReader = new StreamReader("input.txt");

Dictionary<string, Pipe> pipes = new Dictionary<string, Pipe>();

List<string> nonZeroValves = new List<string>();

while (!streamReader.EndOfStream)
{
    string[] lineSplit = streamReader.ReadLine().Split(';');

    var first = lineSplit[0].Split(' ');
    var second = lineSplit[1].Split("valve");

    if (second[1][0] == 's')
    {
        second[1] = second[1].Substring(2);
    }
    else
    {
        second[1] = second[1].Substring(1);
    }

    Pipe newPipe = new Pipe();
    newPipe.Id = first[1];
    newPipe.FlowRate = int.Parse(first[4].Substring(5));
    newPipe.Tunnels = second[1].Split(", ");

    pipes.Add(newPipe.Id, newPipe);

    if (newPipe.FlowRate > 0)
    {
        nonZeroValves.Add(newPipe.Id);
    }
}

foreach (var pipe in pipes)
{
    pipe.Value.DistanceTo = new Dictionary<string, int>();
    foreach (var targetPipe in pipes)
    {
        if (pipe.Key != targetPipe.Key)
        {
            pipe.Value.DistanceTo.Add(targetPipe.Key, DistanceTo(pipe.Key, targetPipe.Key, new List<string>()));
        }
    }
}

Console.WriteLine("Distances filled");

Console.WriteLine((DateTime.Now-start).TotalMilliseconds);
start = DateTime.Now;

//Console.WriteLine(GreedyBest());
Console.WriteLine(RecursiveBest("AA", 30, nonZeroValves));

Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
start = DateTime.Now;

Console.WriteLine(TwoPlayerRecursiveBest("AA", "AA", 26, 26, nonZeroValves, false));

Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
start = DateTime.Now;

int RecursiveBest(string location, int minutesLeft, List<string> unopenedValves)
{
    int bestYield = 0;

    foreach (string unopenedValve in unopenedValves)
    {
        int distance = pipes[location].DistanceTo[unopenedValve];
        int yield = 0;

        if (distance + 1 < minutesLeft)
        {
            List<string> newUnopened = new List<string>();
            newUnopened.AddRange(unopenedValves);
            newUnopened.Remove(unopenedValve);

            yield += pipes[unopenedValve].FlowRate * (minutesLeft - distance - 1);
            yield += RecursiveBest(unopenedValve, minutesLeft - distance - 1, newUnopened);
        }

        if (bestYield < yield)
        {
            bestYield = yield;
        }
    }
    return bestYield;
}

int TwoPlayerRecursiveBest(string elephantLocation, string myLocation, int elephantMinutesLeft, int myMinutesLeft, List<string> unopenedValves, bool elephantTurn)
{
    int bestYield = 0;

    string location = elephantTurn ? elephantLocation : myLocation;
    int minutesLeft = elephantTurn ? elephantMinutesLeft : myMinutesLeft;

    foreach (string unopenedValve in unopenedValves)
    {
        int distance = pipes[location].DistanceTo[unopenedValve];
        int yield = 0;

        if (distance + 1 < minutesLeft)
        {
            List<string> newUnopened = new List<string>();
            newUnopened.AddRange(unopenedValves);
            newUnopened.Remove(unopenedValve);

            yield += pipes[unopenedValve].FlowRate * (minutesLeft - distance - 1);
            if (elephantTurn)
            {
                yield += TwoPlayerRecursiveBest(unopenedValve, myLocation, elephantMinutesLeft - distance - 1,
                    myMinutesLeft, newUnopened, true);
            }
            else
            {
                int myYield = TwoPlayerRecursiveBest(elephantLocation, unopenedValve, elephantMinutesLeft,
                    myMinutesLeft - distance - 1, newUnopened, false);
                int elephantYield = TwoPlayerRecursiveBest(elephantLocation, unopenedValve, elephantMinutesLeft,
                    myMinutesLeft - distance - 1, newUnopened, true);
                yield += Math.Max(myYield, elephantYield);
            }
        }

        if (bestYield < yield)
        {
            bestYield = yield;
        }
    }

    return bestYield;
}

int GreedyBest()
{
    string location = "AA";
    List<string> unopened = new List<string>();
    unopened.AddRange(nonZeroValves);
    int minutesLeft = 30;

    int allYield = 0;

    bool goOn = true;
    do
    {
        int bestYield = 0;
        string bestValve = "";
        foreach (string s in unopened)
        {
            int distance = DistanceTo(location, s, new List<string>());
            if (distance < minutesLeft)
            {
                int yield = (minutesLeft - distance - 1) * pipes[s].FlowRate;
                if (yield > bestYield)
                {
                    bestYield = yield;
                    bestValve = s;
                }
            }
        }

        if (bestValve == "")
        {
            goOn = false;
        }
        else
        {
            allYield += bestYield;
            minutesLeft -= DistanceTo(location, bestValve, new List<string>()) + 1;
            unopened.Remove(bestValve);
            location = bestValve;
        }
    } while (goOn);

    return allYield;
}

int DistanceTo(string from, string to, List<string> route)
{
    if (from == to)
    {
        return 0;
    }
    if (pipes[from].Tunnels.Contains(to))
    {
        return 1;
    }

    int minDist = 10000;
    foreach (string tunnel in pipes[from].Tunnels)
    {
        if (!route.Contains(tunnel))
        {
            List<string> newRoute = new List<string>();
            newRoute.AddRange(route);
            newRoute.Add(tunnel);

            int dist = DistanceTo(tunnel, to, newRoute) + 1;
            if (dist < minDist)
            {
                minDist = dist;
            }
        }
    }

    return minDist;
}

class Pipe
{
    public string Id;
    public int FlowRate;
    public string[] Tunnels;

    public Dictionary<string, int> DistanceTo;
}