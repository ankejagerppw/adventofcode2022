namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly List<Tuple<Tuple<long, long>, Tuple<long, long>>> _input;
    
    public Day04()
    {
        _input = File
            .ReadAllLines(InputFilePath)
            .Select(s => s.Split(","))
            .Select(s =>
            {
                long[] firstPair = s[0].Split("-").Select(long.Parse).ToArray();
                long[] secondPair = s[1].Split("-").Select(long.Parse).ToArray();
                return new Tuple<Tuple<long, long>, Tuple<long, long>>(
                    new Tuple<long, long>(firstPair.Min(), firstPair.Max()),
                    new Tuple<long, long>(secondPair.Min(), secondPair.Max()));
            })
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        return new ValueTask<string>($"Fully contains: {_input.Count(i => FullyContains(i.Item1, i.Item2))}");
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>($"FOverlaps: {_input.Count(i => Overlaps(i.Item1, i.Item2))}");
    }

    private static bool FullyContains(Tuple<long, long> tuple1, Tuple<long, long> tuple2)
    {
        return tuple1.Item1 >= tuple2.Item1 && tuple1.Item2 <= tuple2.Item2
               || tuple2.Item1 >= tuple1.Item1 && tuple2.Item2 <= tuple1.Item2;
    }

    private static bool Overlaps(Tuple<long, long> tuple1, Tuple<long, long> tuple2)
    {
        return tuple1.Item1 <= tuple2.Item2 && tuple1.Item2 >= tuple2.Item1;
    }
}