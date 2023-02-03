using System.Runtime.InteropServices.ComTypes;
using System.Text;

StreamReader streamReader = new StreamReader("input.txt");

int sum = 0;
int index = 0;

List<IData> allPackets = new List<IData>();

while (true)
{
    index++;
    string firstLine = streamReader.ReadLine();
    string secondLine = streamReader.ReadLine();

    ListData firstData = new ListData(firstLine);
    ListData secondData = new ListData(secondLine);

    var result = CompareData(firstData, secondData);

    Console.WriteLine(result);
    Console.WriteLine();
    sum += result == OrderResult.RightOrder ? index : 0;

    allPackets.Add(firstData);
    allPackets.Add(secondData);

    if (streamReader.EndOfStream)
    {
        break;
    }

    streamReader.ReadLine();
}
Console.WriteLine(sum);

allPackets.Add(new DecoderKey("[[2]]"));
allPackets.Add(new DecoderKey("[[6]]"));

bool reordered = false;

do
{
    reordered = false;
    for (int i = 0; i < allPackets.Count - 1; i++)
    {
        var result = CompareData(allPackets[i], allPackets[i + 1]);
        if (result != OrderResult.RightOrder)
        {
            (allPackets[i + 1], allPackets[i]) = (allPackets[i], allPackets[i + 1]);
            reordered = true;
        }
    }
} while (reordered);

foreach (IData packet in allPackets)
{
    Console.WriteLine(packet.ToString());
}
Console.WriteLine();

Console.WriteLine((allPackets.FindIndex(data => data is DecoderKey) + 1) *
                  (allPackets.FindLastIndex(data => data is DecoderKey) + 1));

OrderResult CompareData(IData firstData, IData secondData)
{
    if (firstData is IntData firstInt)
    {
        if (secondData is IntData secondInt)
        {
            if (firstInt.Value > secondInt.Value)
            {
                return OrderResult.NotRightOrder;
            }
            else if (firstInt.Value < secondInt.Value)
            {
                return OrderResult.RightOrder;
            }
        }

        if (secondData is ListData secondList)
        {
            var convertedFirstList = new ListData(firstInt);

            var result = CompareData(convertedFirstList, secondList);
            if (result != OrderResult.Undecided)
            {
                return result;
            }
        }
    }
    if (firstData is ListData firstList)
    {
        if (secondData is IntData secondInt)
        {
            var convertedSecondList = new ListData(secondInt);

            var result = CompareData(firstList, convertedSecondList);
            if (result != OrderResult.Undecided)
            {
                return result;
            }
        }

        if (secondData is ListData secondList)
        {
            for (int i = 0; i < firstList.Content.Count; i++)
            {
                if (i >= secondList.Content.Count)
                {
                    return OrderResult.NotRightOrder;
                }

                var result = CompareData(firstList.Content[i], secondList.Content[i]);

                if (result != OrderResult.Undecided)
                {
                    return result;
                }
            }

            if (secondList.Content.Count > firstList.Content.Count)
            {
                return OrderResult.RightOrder;
            }
        }
    }

    return OrderResult.Undecided;
}

enum OrderResult
{
    RightOrder,
    NotRightOrder,
    Undecided
}

public interface IData
{
}

public class IntData : IData
{
    public int Value { get; set; }
    public IntData(int value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class ListData : IData
{
    public List<IData> Content;

    public ListData(IntData intToConvert)
    {
        Content = new List<IData>();
        Content.Add(intToConvert);
    }

    public ListData(string contentString)
    {
        Content = new List<IData>();

        string insideString = contentString.Substring(1, contentString.Length - 2);

        int listLevel = 0;
        StringBuilder register = new StringBuilder();

        for (int i = 0; i < insideString.Length; i++)
        {
            switch (insideString[i])
            {
                case ',':
                    if (listLevel == 0 && register.Length > 0)
                    {
                        Content.Add(new IntData(int.Parse(register.ToString())));
                        register.Clear();
                    }

                    if (listLevel > 0)
                    {
                        register.Append(',');
                    }
                    break;
                case '[':
                    register.Append('[');
                    listLevel++;
                    break;
                case ']':
                    register.Append(']');
                    listLevel--;
                    if (listLevel == 0)
                    {
                        Content.Add(new ListData(register.ToString()));
                        register.Clear();
                    }
                    break;
                default:
                    register.Append(insideString[i]);
                    break;
            }
        }

        if (register.Length > 0)
        {
            Content.Add(new IntData(int.Parse(register.ToString())));
        }
    }

    public override string ToString()
    {
        List<string> contentStrings = new List<string>();
        foreach (IData data in Content)
        {
            contentStrings.Add(data.ToString());
        }

        return $"[{string.Join(',', contentStrings)}]";
    }
}

public class DecoderKey : ListData
{
    public DecoderKey(IntData intToConvert) : base(intToConvert)
    {
    }

    public DecoderKey(string contentString) : base(contentString)
    {
    }
}