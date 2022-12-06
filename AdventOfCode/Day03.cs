namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly List<Rucksack> _input;

    private class Rucksack
    {
        public char[] Compartment1 { get; }
        public char[] Compartment2 { get; }
        
        public char[] Content { get; }

        public Rucksack(string content)
        {
            int contentLength = content.Length;
            Content = content.ToCharArray();
            Compartment1 = content[..(contentLength / 2)].ToCharArray();
            Compartment2 = content[(contentLength / 2)..].ToCharArray();
        }
    }

    public Day03()
    {
        _input = File
            .ReadAllLines(InputFilePath)
            .Select(s => new Rucksack(s))
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        int sumPriorities = _input
            .Select(r => r.Compartment1.Intersect(r.Compartment2).Single())
            .Select(CalcPriority)
            .Sum();

        return new ValueTask<string>($"Sum of priorities: {sumPriorities}");
    }

    public override ValueTask<string> Solve_2()
    {
        const int ChunkSize = 3;
        List<List<Rucksack>> groupedElves = _input
            .Select((rucksack, index) => new
            {
                rucksack,
                index
            })
            .GroupBy(x => x.index / ChunkSize)
            .Select(grp => grp.Select(x => x.rucksack).ToList())
            .ToList();

        int sumPriorities = groupedElves.Sum(
            rucksacks => rucksacks
                .Aggregate(rucksacks[0].Content, (current, r) => current.Intersect(r.Content).ToArray())
                .Select(CalcPriority)
                .Single());

        return new ValueTask<string>($"Sum of priorities: {sumPriorities}");
    }

    private static int CalcPriority(char c)
    {
        return c % 32 + (char.IsUpper(c) ? 26 : 0);
    }
}