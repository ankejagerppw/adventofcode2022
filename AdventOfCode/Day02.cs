namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly List<string[]> _input;

    private enum Shape
    {
        ROCK = 1,
        PAPER = 2,
        SCISSORS = 3
    }

    private static Dictionary<Shape, Shape> s_WinningMoves = new Dictionary<Shape, Shape>
    {
        { Shape.ROCK, Shape.SCISSORS },
        { Shape.SCISSORS, Shape.PAPER },
        { Shape.PAPER, Shape.ROCK }
    };

    private class Round
    {
        private Shape Player1 { get; }
        private Shape? Player2 { get; }

        public Round(Shape player1, Shape player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public Round(Shape player1, string outcome)
        {
            Player1 = player1;
            Player2 = outcome switch
            {
                "X" => s_WinningMoves[player1],
                "Y" => player1,
                "Z" => s_WinningMoves.Single(wm => wm.Value == player1).Key,
                _ => Player2
            };
        }

        private int? ScoreForGameOutcomeForPlayer2()
        {
            if (!Player2.HasValue)
            {
                return null;
            }
            if (Player1 == Player2)
            {
                return 3;
            }

            return s_WinningMoves[Player2.Value] == Player1 ? 6 : 0;
        }
        
        public int? ScoreForPlayer2()
        {
            return Player2.HasValue ? ScoreForGameOutcomeForPlayer2() + (int)Player2 : null;
        }
    }
    
    public Day02()
    {
        _input = File
            .ReadLines(InputFilePath)
            .Select(inputLine => inputLine.Split(" "))
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        List<Round> rounds = _input
            .Select(inputLine => new Round(DetermineShape(inputLine[0]), DetermineShape(inputLine[1])))
            .ToList();
        return new ValueTask<string>($"Score: {rounds.Select(i => i.ScoreForPlayer2()).Sum()}");
    }

    public override ValueTask<string> Solve_2()
    {
        List<Round> rounds = _input
            .Select(inputLine => new Round(DetermineShape(inputLine[0]), inputLine[1]))
            .ToList();
        return new ValueTask<string>($"Score: {rounds.Select(i => i.ScoreForPlayer2()).Sum()}");
    }
    
    private static Shape DetermineShape(string s)
    {
        switch (s)
        {
            case "A":
            case "X":
                return Shape.ROCK;
            case "B":
            case "Y":
                return Shape.PAPER;
            case "C":
            case "Z":
                return Shape.SCISSORS;
            default:
                throw new ApplicationException($"Wrong input: {s}");
        }
    }
}