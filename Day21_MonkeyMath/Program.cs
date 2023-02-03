using System.Numerics;

Dictionary<string, IMonkey> Monkeys = new Dictionary<string, IMonkey>();

StreamReader streamReader = new StreamReader("input.txt");

while (!streamReader.EndOfStream)
{
    string line = streamReader.ReadLine();

    var lineSplit = line.Split(' ');

    IMonkey newMonkey;

    if (lineSplit.Length <= 2)
    {
        newMonkey = new NumberMonkey();
        (newMonkey as NumberMonkey).Number = int.Parse(lineSplit[1]);
    }
    else
    {
        newMonkey = new OperationMonkey();
        (newMonkey as OperationMonkey).FirstMonkey = lineSplit[1];
        (newMonkey as OperationMonkey).SecondMonkey = lineSplit[3];
        switch (lineSplit[2])
        {
            case "+":
                (newMonkey as OperationMonkey).MonkeyOperation = Operation.Add;
                break;
            case "-":
                (newMonkey as OperationMonkey).MonkeyOperation = Operation.Subtract;
                break;
            case "*":
                (newMonkey as OperationMonkey).MonkeyOperation = Operation.Multiply;
                break;
            case "/":
                (newMonkey as OperationMonkey).MonkeyOperation = Operation.Divide;
                break;
        }
    }

    Monkeys.Add(lineSplit[0].Substring(0, lineSplit[0].Length - 1), newMonkey);
}

Console.WriteLine(Monkeys["root"].Shout(Monkeys));
Monkeys["root"].FlushResults(Monkeys);

Monkeys["humn"] = new NumberMonkey() {Number = null};
string rootFirst = (Monkeys["root"] as OperationMonkey).FirstMonkey;
string rootSecond = (Monkeys["root"] as OperationMonkey).SecondMonkey;
Monkeys["root"] = new EqualityMonkey() {FirstMonkey = rootFirst, SecondMonkey = rootSecond};
Console.WriteLine(Monkeys["root"].Shout(Monkeys));


public interface IMonkey
{
    public BigInteger? Shout(Dictionary<string, IMonkey> Monkeys);
    public BigInteger? ForceValue(BigInteger forcedValue, Dictionary<string, IMonkey> Monkeys);
    public void FlushResults(Dictionary<string, IMonkey> Monkeys);
}

public class NumberMonkey : IMonkey
{
    public BigInteger? Number { get; set; }

    public BigInteger? Shout(Dictionary<string, IMonkey> Monkeys)
    {
        return Number;
    }

    public BigInteger? ForceValue(BigInteger forcedValue, Dictionary<string, IMonkey> Monkeys)
    {
        return forcedValue;
    }

    public void FlushResults(Dictionary<string, IMonkey> Monkeys)
    {
    }
}

public class OperationMonkey : IMonkey
{
    public string FirstMonkey { get; set; }
    public string SecondMonkey { get; set; }
    public Operation MonkeyOperation { get; set; }

    private BigInteger? _result;

    public BigInteger? Shout(Dictionary<string, IMonkey> monkeys)
    {
        if (_result != null)
        {
            return _result;
        }

        BigInteger? firstMonkeyValue = monkeys[FirstMonkey].Shout(monkeys);
        BigInteger? secondMonkeyValue = monkeys[SecondMonkey].Shout(monkeys);

        switch (MonkeyOperation)
        {
            case Operation.Add:
                _result = firstMonkeyValue + secondMonkeyValue;
                break;
            case Operation.Subtract:
                _result = firstMonkeyValue - secondMonkeyValue;
                break;
            case Operation.Multiply:
                _result = firstMonkeyValue * secondMonkeyValue;
                break;
            case Operation.Divide:
                _result = firstMonkeyValue / secondMonkeyValue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return _result;
    }

    public BigInteger? ForceValue(BigInteger forcedValue, Dictionary<string, IMonkey> monkeys)
    {
        BigInteger? firstMonkeyValue = monkeys[FirstMonkey].Shout(monkeys);
        BigInteger? secondMonkeyValue = monkeys[SecondMonkey].Shout(monkeys);

        if (firstMonkeyValue == null && secondMonkeyValue != null)
        {
            BigInteger? inverted;
            switch (MonkeyOperation)
            {
                case Operation.Add:
                    inverted = forcedValue - secondMonkeyValue;
                    break;
                case Operation.Subtract:
                    inverted = forcedValue + secondMonkeyValue;
                    break;
                case Operation.Multiply:
                    inverted = forcedValue / secondMonkeyValue;
                    break;
                case Operation.Divide:
                    inverted = forcedValue * secondMonkeyValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return monkeys[FirstMonkey].ForceValue(inverted ?? 0, monkeys);
        }
        if (firstMonkeyValue != null && secondMonkeyValue == null)
        {
            BigInteger? inverted;
            switch (MonkeyOperation)
            {
                case Operation.Add:
                    inverted = forcedValue - firstMonkeyValue;
                    break;
                case Operation.Subtract:
                    inverted = firstMonkeyValue - forcedValue;
                    break;
                case Operation.Multiply:
                    inverted = forcedValue / firstMonkeyValue;
                    break;
                case Operation.Divide:
                    inverted = firstMonkeyValue / forcedValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return monkeys[SecondMonkey].ForceValue(inverted ?? 0, monkeys);
        }

        throw new Exception("Two unknown values in one operation monkey");
        return 0;
    }

    public void FlushResults(Dictionary<string, IMonkey> Monkeys)
    {
        _result = null;
        Monkeys[FirstMonkey].FlushResults(Monkeys);
        Monkeys[SecondMonkey].FlushResults(Monkeys);
    }
}

public class EqualityMonkey : IMonkey
{
    public string FirstMonkey { get; set; }
    public string SecondMonkey { get; set; }

    public BigInteger? Shout(Dictionary<string, IMonkey> monkeys)
    {
        BigInteger? firstMonkeyValue = monkeys[FirstMonkey].Shout(monkeys);
        BigInteger? secondMonkeyValue = monkeys[SecondMonkey].Shout(monkeys);

        if (firstMonkeyValue == null && secondMonkeyValue != null)
        {
            return monkeys[FirstMonkey].ForceValue(secondMonkeyValue ?? 0, monkeys);
        }
        if (firstMonkeyValue != null && secondMonkeyValue == null)
        {
            return monkeys[SecondMonkey].ForceValue(firstMonkeyValue ?? 0, monkeys);
        }
        return 0;
    }

    public BigInteger? ForceValue(BigInteger forcedValue, Dictionary<string, IMonkey> Monkeys)
    {
        throw new NotImplementedException();
    }

    public void FlushResults(Dictionary<string, IMonkey> Monkeys)
    {
        Monkeys[FirstMonkey].FlushResults(Monkeys);
        Monkeys[SecondMonkey].FlushResults(Monkeys);
    }
}

public enum Operation
{
    Add,
    Subtract,
    Multiply,
    Divide,
}