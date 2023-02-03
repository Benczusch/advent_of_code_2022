using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

StreamReader streamReader = new StreamReader("input.txt");

List<Number> originalOrder = new List<Number>();
CircularList<Number> currentOrder = new CircularList<Number>();

bool secondTask = false;
BigInteger multiplier = secondTask ? 811589153 : 1;

for (string line = streamReader.ReadLine(); !(line is null); line = streamReader.ReadLine())
{
    var number = new Number(int.Parse(line) * multiplier, originalOrder.Count);
    originalOrder.Add(number);
    currentOrder.Add(number);
}

for (int turn = 0; turn < (secondTask ? 10 : 1); turn++)
{
    foreach (Number number in originalOrder)
    {
        BigInteger newPosition = number.Position + number.Value;
        while (newPosition <= 0)
        {
            newPosition += (currentOrder.Count - 1) * multiplier;
        }
        if (newPosition >= currentOrder.Count)
        {
            newPosition %= currentOrder.Count - 1;
        }

        currentOrder.RemoveAt(number.Position);
        currentOrder.Insert((int)newPosition, number);

        number.Position = (int)newPosition;

        for (int i = 0; i < currentOrder.Count; i++)
        {
            currentOrder[i].Position = i;
        }

        //Console.WriteLine(currentOrder.ToString());
    }
}

int zeroIndex = originalOrder.Find(x => x.Value == 0).Position;
Console.WriteLine(currentOrder[zeroIndex + 1000].Value + currentOrder[zeroIndex + 2000].Value + currentOrder[zeroIndex + 3000].Value);
StreamWriter sw = new StreamWriter("output.txt");
sw.WriteLine(currentOrder.ToString());
sw.Close();

class Number
{
    public BigInteger Value { get; set; }
    public int Position { get; set; }
    public Number(BigInteger value, int position)
    {
        Value = value;
        Position = position;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

class CircularList<T> : List<T>
{
    public new T this[int index]
    {
        get => base[index % this.Count];
        set => base[index % this.Count] = value;
    }

    public override string ToString()
    {
        StringBuilder _sb = new StringBuilder();
        foreach (T num in this)
        {
            _sb.Append(num.ToString() + ", ");
        }
        _sb.AppendLine();
        return _sb.ToString();
    }
}