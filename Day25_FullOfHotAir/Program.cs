using System.Numerics;

StreamReader streamReader = new StreamReader("input.txt");

BigInteger sum = BigInteger.Zero;

Dictionary<char, BigInteger> snafuDictionary = new Dictionary<char, BigInteger>()
{
    {'=', -2},
    {'-', -1},
    {'0', 0},
    {'1', 1},
    {'2', 2},
};

while (!streamReader.EndOfStream)
{
    string snafu = streamReader.ReadLine();

    BigInteger multiplier = BigInteger.One;

    for (int i = snafu.Length - 1; i >= 0; i--)
    {
        sum += multiplier * snafuDictionary[snafu[i]];
        multiplier *= 5;
    }

}

Console.WriteLine(sum);

Dictionary<BigInteger, char> ToSnafu = new Dictionary<BigInteger, char>()
{
    {-2, '='},
    {-1, '-'},
    {0, '0'},
    {1, '1'},
    {2, '2'},
};
List<char> snafuAnswer = new List<char>();
while (sum > 0)
{
    BigInteger digit = sum % 5;
    if (digit>2)
    {
        digit -= 5;
    }
    snafuAnswer.Add(ToSnafu[digit]);
    sum -= digit;
    sum /= 5;
}

snafuAnswer.Reverse();
foreach (char c in snafuAnswer)
{
    Console.Write(c);
}
Console.WriteLine();