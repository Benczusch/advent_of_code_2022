using System.Numerics;

bool realData = true;

string fileName = realData ? "input.txt" : "testInput.txt";
int examinedRow = realData ? 2000000 : 10;
int boundary = realData ? 4000000 : 20;

StreamReader streamReader = new StreamReader(fileName);

List<(int, int)> sensors = new List<(int, int)>();
List<(int, int)> beacons = new List<(int, int)>();

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();
    var lineSplit = line.Split(", ");
    string[] tempSplit;

    tempSplit = lineSplit[0].Split("x=");
    int sensorX = int.Parse(tempSplit[1]);

    tempSplit = lineSplit[1].Substring(2).Split(":");
    int sensorY = int.Parse(tempSplit[0]);

    tempSplit = lineSplit[1].Split("x=");
    int beaconX = int.Parse(tempSplit[1]);
    
    int beaconY = int.Parse(lineSplit[2].Substring(2));

    //Console.WriteLine(line);
    //Console.WriteLine("->");
    //Console.WriteLine($"Sensor at x={sensorX}, y={sensorY}: closest beacon is at x={beaconX}, y={beaconY}");
    //Console.WriteLine("-----------------------------");

    sensors.Add((sensorX, sensorY));
    beacons.Add((beaconX, beaconY));
}

List<(int, int)> exclusionIntervals = new List<(int, int)>();
int minValue = 0;
int maxValue = 0;

for (int i = 0; i < sensors.Count; i++)
{
    int radius = Math.Abs(sensors[i].Item1 - beacons[i].Item1) + Math.Abs(sensors[i].Item2 - beacons[i].Item2);
    int rowRadius = radius - Math.Abs(sensors[i].Item2 - examinedRow);
    if (rowRadius >= 0)
    {
        int leftBorder = sensors[i].Item1 - rowRadius;
        int rightBorder = sensors[i].Item1 + rowRadius;
        exclusionIntervals.Add((leftBorder, rightBorder));

        if (leftBorder < minValue)
        {
            minValue = leftBorder;
        }
        if (rightBorder > maxValue)
        {
            maxValue = rightBorder;
        }
    }
}

List<int> beaconsInExaminedRow = new List<int>();
foreach ((int, int) beacon in beacons)
{
    if (beacon.Item2 == examinedRow)
    {
        beaconsInExaminedRow.Add(beacon.Item1);
    }
}

bool[] excluded = new bool[maxValue - minValue + 1];

foreach ((int, int) exclusionInterval in exclusionIntervals)
{
    for (int i = exclusionInterval.Item1 - minValue; i <= exclusionInterval.Item2 - minValue; i++)
    {
        if (beaconsInExaminedRow.All(x => x != i - minValue))
        {
            excluded[i] = true;
        }
    }
}

Console.WriteLine(excluded.Count(x => x));

List<int> radii = new List<int>();

for (int i = 0; i < sensors.Count; i++)
{
    int radius = Math.Abs(sensors[i].Item1 - beacons[i].Item1) + Math.Abs(sensors[i].Item2 - beacons[i].Item2);
    radii.Add(radius);
}

for (int i = 0; i < sensors.Count; i++)
{
    int searchRadius = radii[i] + 1;

    for (int dx = -searchRadius; dx <= searchRadius; dx++)
    {
        int x = sensors[i].Item1 + dx;
        int yRadius = searchRadius - Math.Abs(dx);
        int y1 = sensors[i].Item2 + yRadius;
        int y2 = sensors[i].Item2 - yRadius;

        if (InBoundary(x, y1))
        {
            bool outsideAll = true;
            for (int j = 0; j < sensors.Count; j++)
            {
                int distance = Math.Abs(sensors[j].Item1 - x) + Math.Abs(sensors[j].Item2 - y1);
                if (distance <= radii[j])
                {
                    outsideAll = false;
                    break;
                }
            }
            if (outsideAll && beacons.All(beacon => (beacon.Item1 != x) || (beacon.Item2 != y1)))
            {
                Console.WriteLine($"X: {x}, Y: {y1}");
                Console.WriteLine(4000000 * (BigInteger)x + (BigInteger)y1);
            }
        }

        if (InBoundary(x, y2))
        {
            bool outsideAll = true;
            for (int j = 0; j < sensors.Count; j++)
            {
                int distance = Math.Abs(sensors[j].Item1 - x) + Math.Abs(sensors[j].Item2 - y2);
                if (distance <= radii[j])
                {
                    outsideAll = false;
                    break;
                }
            }

            if (outsideAll && beacons.All(beacon => (beacon.Item1 != x) || (beacon.Item2 != y2)))
            {
                Console.WriteLine($"X: {x}, Y: {y2}");
                Console.WriteLine(4000000*(BigInteger)x+ (BigInteger)y2);
            }
        }
    }
}

bool InBoundary(int x, int y)
{
    return x >= 0 && y >= 0 && x <= boundary && y <= boundary;
}
