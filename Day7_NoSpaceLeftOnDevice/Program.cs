using System.Net.NetworkInformation;

StreamReader streamReader = new StreamReader("input.txt");

streamReader.ReadLine();
Directory outer = new Directory(null, "/");
Directory cursor = outer;

while (!streamReader.EndOfStream)
{
    string command = streamReader.ReadLine();

    if (command.StartsWith("$ cd"))
    {
        string dirName = command.Substring(5);

        if (dirName == "/")
        {
            cursor = outer;
            break;
        }

        if (dirName == "..")
        {
            cursor = cursor.Parent;
        }

        foreach (Element element in cursor.Children)
        {
            if (element is Directory directory && directory.Name == dirName)
            {
                cursor = directory;
                break;
            }
        }
    }

    else if (command.StartsWith("$ ls"))
    {
        
    }

    else if (command.StartsWith("dir "))
    {
        var result = command.Split(' ');
        if (cursor.Children.All(element => element.Name != result[1]))
        {
            cursor.Children.Add(new Directory(cursor, result[1]));
        }
    }

    else
    {
        var result = command.Split(' ');
        if (cursor.Children.All(element => element.Name != result[1]))
        {
            cursor.Children.Add(new File(cursor, int.Parse(result[0]), result[1]));
        }
    }
}

int sumSize = 0;
int fullSize = outer.RecursiveSize(ref sumSize);
Console.WriteLine(sumSize);

int freeSpace = 70000000 - fullSize;
int toDelete = 30000000 - freeSpace;

int smallestToDelete = 90000000;
outer.SmallestLargerThan(toDelete, ref smallestToDelete);

Console.WriteLine(smallestToDelete);

public abstract class Element
{
    public Directory Parent;
    public string Name;

    public Element(Directory parent, string name)
    {
        Parent = parent;
        Name = name;
    }

    public abstract int RecursiveSize(ref int sum);
    public abstract int SmallestLargerThan(int threshold, ref int smallest);
}

public class File : Element
{
    public int Size;


    public File(Directory parent, string name) : base(parent, name)
    {
    }

    public File(Directory parent, int size, string name) : this(parent, name)
    {
        Size = size;
    }

    public override int RecursiveSize(ref int sum)
    {
        return this.Size;
    }
    public override int SmallestLargerThan(int threshold, ref int smallest)
    {
        return this.Size;
    }
}

public class Directory : Element
{
    public List<Element> Children = new List<Element>();

    public Directory(Directory parent, string name) : base(parent, name)
    {
    }

    public override int RecursiveSize(ref int sum)
    {
        int sumSize = 0;

        foreach (var element in Children)
        {
            int childSize = element.RecursiveSize(ref sum);

            sumSize += childSize;
        }

        if (sumSize < 100000)
        {
            sum += sumSize;
        }

        return sumSize;
    }

    public override int SmallestLargerThan(int threshold, ref int smallest)
    {
        int sumSize = 0;

        foreach (var element in Children)
        {
            int childSize = element.SmallestLargerThan(threshold, ref smallest);

            sumSize += childSize;
        }

        if (sumSize >= threshold && sumSize < smallest)
        {
            smallest = sumSize;
        }

        return sumSize;
    }
}