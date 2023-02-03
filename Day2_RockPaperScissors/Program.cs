Dictionary<Outcome, int> PointValue = new Dictionary<Outcome, int>() {{Outcome.Rock, 1}, {Outcome.Paper, 2},
 {Outcome.Scissors, 3}};

StreamReader streamReader = new StreamReader("input.txt");

int myPoints = 0;

while (!streamReader.EndOfStream)
{
    var line = streamReader.ReadLine();

    Outcome opponentPick = (Outcome)(line[0] - 'A');
    Outcome myPick = (Outcome)(line[2] - 'X');

    bool secondTask = true;

    if (secondTask)
    {
        switch (line[2])
        {
            case 'X':
                myPick = (Outcome) (((int) opponentPick + 2) % 3);
                break;
            case 'Y':
                myPick = opponentPick;
                break;
            case 'Z':
                myPick = (Outcome)(((int)opponentPick + 1) % 3);
                break;
            default:
                break;
        }
    }

    myPoints += (int)myPick + 1;

    if (myPick == opponentPick)
    {
        myPoints += 3;
    }
    else if (((int)opponentPick + 1) % 3 == (int)myPick)
    {
        myPoints += 6;
    }
}
Console.WriteLine(myPoints);

enum Outcome
{
    Rock = 0,
    Paper = 1,
    Scissors = 2
}

