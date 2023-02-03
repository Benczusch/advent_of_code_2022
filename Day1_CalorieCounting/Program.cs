StreamReader streamReader = new StreamReader("input1.txt");

int thisElfCalorie = 0;

List<int> Calories = new List<int>();

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();

    if (line.Length>0)
    {
        thisElfCalorie += int.Parse(line);
    }
    else
    {
        int insertIndex = 0;
        for (int i = 0; i < Calories.Count; i++)
        {
            if (Calories[i] > thisElfCalorie)
            {
                insertIndex++;
            }
            else
            {
                break;
            }
        }
        Calories.Insert(insertIndex, thisElfCalorie);
        thisElfCalorie = 0;
    }
}
Console.WriteLine(Calories[0]);
Console.WriteLine(Calories[1]);
Console.WriteLine(Calories[2]);
Console.WriteLine();
Console.WriteLine(Calories[0] + Calories[1] + Calories[2]);