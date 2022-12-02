namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly string[] _input;

    public Day01()
    {
        _input = File
            .ReadLines(InputFilePath)
            .ToArray();
    }

    public override ValueTask<string> Solve_1()
    {
        int maxCalories = 0;
        int sumCalories = 0;
        foreach (string line in _input)
        {
            int nbrCalories;
            if (int.TryParse(line, out nbrCalories))
            {
                sumCalories += nbrCalories;
            }
            else
            {
                if (sumCalories > maxCalories)
                {
                    maxCalories = sumCalories;
                }

                sumCalories = 0;
            }
        }

        return new ValueTask<string>($"Max nbr calories: {maxCalories}");
    }

    public override ValueTask<string> Solve_2()
    {
        List<int> calories = new List<int>();

        List<int> caloriesPerElf = new List<int>();
        foreach (string line in _input)
        {
            if (int.TryParse(line, out int nbrCalories))
            {
                caloriesPerElf.Add(nbrCalories);
            }
            else
            {
                calories.Add(caloriesPerElf.Sum());
                caloriesPerElf.Clear();
            }
        }

        int sumTopCalories = calories
            .OrderByDescending(i => i)
            .Take(3)
            .Sum();

        return new($"Sum of top 3 calories: {sumTopCalories}");
    }
}
