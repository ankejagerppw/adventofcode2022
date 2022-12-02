namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly List<int> _input;

    public Day01()
    {
        _input = File
            .ReadAllText(InputFilePath)
            .Split("\r\n\r\n")
            .Select(elfCalories =>
                elfCalories
                    .Split("\r\n")
                    .Select(int.Parse)
                    .Sum()
            )
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        return new ValueTask<string>($"Max nbr calories: {_input.Max()}");
    }

    public override ValueTask<string> Solve_2()
    {
        int sumTopCalories = _input
            .OrderByDescending(i => i)
            .Take(3)
            .Sum();

        return new ValueTask<string>($"Sum of top 3 calories: {sumTopCalories}");
    }
}
