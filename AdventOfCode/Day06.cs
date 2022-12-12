namespace AdventOfCode.Inputs;

public class Day06 : BaseDay
{
    private readonly string _input;
    
    public Day06()
    {
        _input = File.ReadAllText(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        return new ValueTask<string>($"4 unique chars after reading {IndexOfFirstXUniqueCharacters(4)} chars");
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>($"14 unique chars after reading {IndexOfFirstXUniqueCharacters(14)} chars");
    }

    private int IndexOfFirstXUniqueCharacters(int nbrOfUniqueChars)
    {
        bool found = false;
        int idx = 0;
        for (int i = 0; i < _input.Length - nbrOfUniqueChars && !found; i++)
        {
            found = UniqueChars(_input.Substring(i, nbrOfUniqueChars));
            if (found)
            {
                idx = i + nbrOfUniqueChars;
            }
        }

        return idx;
    }

    private static bool UniqueChars(string inputChars)
    {
        return inputChars.Distinct().Count() == inputChars.Length;
    }
}