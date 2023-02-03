using System.Collections;
using System.Net.Http.Headers;
using System.Numerics;

bool secondTask = true;

StreamReader streamReader = new StreamReader("input.txt");

List<Monkey> monkeys = new List<Monkey>();
int commondivisor = 1;
while (!streamReader.EndOfStream)
{
    Monkey monkey = new Monkey();
    string[] lineSplit = streamReader.ReadLine().Split(' ');
    lineSplit[1] = lineSplit[1].Substring(0, lineSplit[1].Length - 1);
    monkey.Id = int.Parse(lineSplit[1]);

    lineSplit = streamReader.ReadLine().Split(':');
    string[] listSplit = lineSplit[1].Split(',');
    foreach (var item in listSplit)
    {
        monkey.Items.Add(int.Parse(item));
    }
    
    lineSplit = streamReader.ReadLine().Split("new =");

    monkey.Operation = new Expression(lineSplit[1]);

    lineSplit = streamReader.ReadLine().Split("by");
    monkey.TestDivisor = int.Parse(lineSplit[1]);

    if (commondivisor % monkey.TestDivisor != 0)
    {
        commondivisor *= monkey.TestDivisor;
    }

    lineSplit = streamReader.ReadLine().Split("monkey");
    monkey.TrueTarget = int.Parse(lineSplit[1]);
    lineSplit = streamReader.ReadLine().Split("monkey");
    monkey.FalseTarget = int.Parse(lineSplit[1]);

    streamReader.ReadLine();

    if (monkeys.Count == monkey.Id)
    {
        monkeys.Add(monkey);
    }
    else
    {
        throw new Exception("Id mismatch");
    }
}

for (int round = 0; round < (secondTask ? 10000 : 20); round++)
{
    for (int monkeyId = 0; monkeyId < monkeys.Count; monkeyId++)
    {
        var monkey = monkeys[monkeyId];
        for (int i = 0; i < monkey.Items.Count; i++)
        {
            monkey.Items[i] = monkey.Operation.Evaluate(monkey.Items[i]);
            if (!secondTask)
                monkey.Items[i] /= 3;
            else
                monkey.Items[i] %= commondivisor;
            monkey.InspectionCount++;
            if (monkey.Items[i] % monkey.TestDivisor == 0)
            {
                monkeys[monkey.TrueTarget].Items.Add(monkey.Items[i]);
            }
            else
            {
                monkeys[monkey.FalseTarget].Items.Add(monkey.Items[i]);
            }
            monkey.Items.RemoveAt(i);
            i--;
        }
    }

    if (round % 100 == 0)
    {
        Console.WriteLine(round);
    }
}

List<BigInteger> inspectionCounts = new List<BigInteger>();
foreach (Monkey monkey in monkeys)
{
    Console.WriteLine(string.Join(", ", monkey.Items));
    inspectionCounts.Add(monkey.InspectionCount);
}
inspectionCounts.Sort();
Console.WriteLine(inspectionCounts[^1] * inspectionCounts[^2]);

public class Monkey
{
    public int Id;
    public List<BigInteger> Items = new List<BigInteger>();
    public Expression Operation;
    public int TestDivisor;
    public int TrueTarget;
    public int FalseTarget;

    public BigInteger InspectionCount = 0;
}

public class Expression
{
    public Expression FirstHalf;
    public Expression SecondHalf;
    public Operation Operation;

    public Expression(string expressionString)
    {
        string[] split = new []{"0", "0"};
        if (expressionString.Contains('+'))
        {
            split = expressionString.Split('+');
            Operation = Operation.Add;
        }
        else if (expressionString.Contains('-'))
        {
            split = expressionString.Split('-');
            Operation = Operation.Multiply;
        }
        else if (expressionString.Contains('*'))
        {
            split = expressionString.Split('*');
            Operation = Operation.Multiply;
        }

        int num;
        if (int.TryParse(split[0], out num))
        {
            FirstHalf = new Number() {Value = num};
        }
        else if (split[0].Trim(' ') == "old")
        {
            FirstHalf = new Variable();
        }
        else
        {
            FirstHalf = new Expression(split[0]);
        }
        if (int.TryParse(split[1], out num))
        {
            SecondHalf = new Number() {Value = num};
        }
        else if (split[1].Trim() == "old")
        {
            SecondHalf = new Variable();
        }
        else
        {
            SecondHalf = new Expression(split[1]);
        }

        
    }
    public Expression()
    {

    }

    public virtual BigInteger Evaluate(BigInteger variable)
    {
        switch (Operation)
        {
            case Operation.Multiply:
                return FirstHalf.Evaluate(variable) * SecondHalf.Evaluate(variable);
                break;
            case Operation.Add:
                return FirstHalf.Evaluate(variable) + SecondHalf.Evaluate(variable);
                break;
            case Operation.Subtract:
                return FirstHalf.Evaluate(variable) - SecondHalf.Evaluate(variable);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
public class Number : Expression
{
    public BigInteger Value { get; set; }

    public override BigInteger Evaluate(BigInteger variable)
    {
        return Value;
    }

    public Number()
    {
    }
}
public class Variable : Expression
{
    public override BigInteger Evaluate(BigInteger variable)
    {
        return variable;
    }

    public Variable()
    {
    }
}
public enum Operation
{
    Multiply,
    Add,
    Subtract,
}