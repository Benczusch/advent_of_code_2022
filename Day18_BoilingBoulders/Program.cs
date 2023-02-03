using System.Runtime.CompilerServices;

List<string> lines = new List<string>();
HashSet<string> voxels = new HashSet<string>();

int[][] directions = new[]
{
    new int[] {0, 0, 1},
    new int[] {0, 0, -1},
    new int[] {0, 1, 0},
    new int[] {0, -1, 0},
    new int[] {1, 0, 0},
    new int[] {-1, 0, 0},
};

StreamReader streamReader = new StreamReader("input.txt");
while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();
    lines.Add(line);
    voxels.Add(line);
}

int[] coordinateMaxima = new int[] {0, 0, 0};
int[] coordinateMinima = new int[] {9999, 9999, 9999 };

Console.WriteLine(GetSurface(voxels));

int maxVoxels = 1;
maxVoxels *= coordinateMaxima[0] - coordinateMinima[0] + 3;
maxVoxels *= coordinateMaxima[1] - coordinateMinima[1] + 3;
maxVoxels *= coordinateMaxima[2] - coordinateMinima[2] + 3;

HashSet<string> inverse = new HashSet<string>(maxVoxels);

for (int x = coordinateMinima[0] - 1; x <= coordinateMaxima[0] + 1; x++)
{
    for (int y = coordinateMinima[1] - 1; y <= coordinateMaxima[1] + 1; y++)
    {
        for (int z = coordinateMinima[2] - 1; z <= coordinateMaxima[2] + 1; z++)
        {
            string descriptor = $"{x},{y},{z}";
            if (!voxels.Contains(descriptor))
            {
                inverse.Add(descriptor);
            }
        }
    }
}

HashSet<string> outerBlock = new HashSet<string>();

Collect(inverse.First());

int outerSurface = 0;

outerSurface += 2 * (coordinateMaxima[0] - coordinateMinima[0] + 3) * (coordinateMaxima[1] - coordinateMinima[1] + 3);
outerSurface += 2 * (coordinateMaxima[1] - coordinateMinima[1] + 3) * (coordinateMaxima[2] - coordinateMinima[2] + 3);
outerSurface += 2 * (coordinateMaxima[2] - coordinateMinima[2] + 3) * (coordinateMaxima[0] - coordinateMinima[0] + 3);

Console.WriteLine(GetSurface(outerBlock) - outerSurface);

int GetSurface(HashSet<string> form)
{
    int surface = 0;

    foreach (string voxel in form)
    {
        string[] coordinates = voxel.Split(',');

        int x = int.Parse(coordinates[0]);
        int y = int.Parse(coordinates[1]);
        int z = int.Parse(coordinates[2]);

        coordinateMinima[0] = Math.Min(coordinateMinima[0], x);
        coordinateMinima[1] = Math.Min(coordinateMinima[1], y);
        coordinateMinima[2] = Math.Min(coordinateMinima[2], z);
        coordinateMaxima[0] = Math.Max(coordinateMaxima[0], x);
        coordinateMaxima[1] = Math.Max(coordinateMaxima[1], y);
        coordinateMaxima[2] = Math.Max(coordinateMaxima[2], z);

        foreach (int[] direction in directions)
        {
            if (!form.Contains($"{x + direction[0]},{y + direction[1]},{z + direction[2]}"))
            {
                surface++;
            }
        }
    }

    return surface;
}

void RecursiveCollect(string seed)
{
    outerBlock.Add(seed);

    string[] coordinates = seed.Split(',');

    int x = int.Parse(coordinates[0]);
    int y = int.Parse(coordinates[1]);
    int z = int.Parse(coordinates[2]);

    foreach (int[] direction in directions)
    {
        if (InBoundary(x,y,z))
        {
            string neighbour = $"{x + direction[0]},{y + direction[1]},{z + direction[2]}";

            if (inverse.Contains(neighbour) && !outerBlock.Contains(neighbour))
            {
                RecursiveCollect(neighbour);
            }
        }
    }
}


void Collect(string seed)
{
    int counter = 0;
    Queue<string> toCollect = new Queue<string>();
    HashSet<string> queued = new HashSet<string>();

    toCollect.Enqueue(seed);

    string element;

    while (toCollect.Count > 0)
    {
        counter++;
        if (counter % 100000 == 0)
        {
            Console.WriteLine(toCollect.Count);
        }

        element = toCollect.Dequeue();
        outerBlock.Add(element);

        string[] coordinates = element.Split(',');

        int x = int.Parse(coordinates[0]);
        int y = int.Parse(coordinates[1]);
        int z = int.Parse(coordinates[2]);

        foreach (int[] direction in directions)
        {
            if (InBoundary(x, y, z))
            {
                string neighbour = $"{x + direction[0]},{y + direction[1]},{z + direction[2]}";

                if (inverse.Contains(neighbour) && !queued.Contains(neighbour))
                {
                    toCollect.Enqueue(neighbour);
                    queued.Add(neighbour);
                }
            }
        }
    }
}

bool InBoundary(int x, int y, int z)
{
    return x >= coordinateMinima[0] - 1 
        && x <= coordinateMaxima[0] + 1
        && y >= coordinateMinima[1] - 1
        && y <= coordinateMaxima[1] + 1
        && z >= coordinateMinima[2] - 1
        && z <= coordinateMaxima[2] + 1;
}