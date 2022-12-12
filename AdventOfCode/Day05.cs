using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly Dictionary<int, List<string>> _inputPiles;
    private readonly List<(int nbrOfCrates, int from, int to)> _inputMoves;

    public Day05()
    {
        string[] inputLines = File.ReadAllLines(InputFilePath);
        int indexOfBlankLine = Array.IndexOf(inputLines, string.Empty);
        int indexOfStackNumbers = indexOfBlankLine - 1;
        List<int> stackNumbers = inputLines[indexOfStackNumbers]
            .Trim()
            .Split("   ")
            .Select(int.Parse)
            .ToList();
        const int ChunkSize = 4;
        List<string[]> crates = new List<string[]>();
        for (int idx = indexOfStackNumbers - 1; idx >= 0; idx--)
        {
            string inputLine = inputLines[idx];
            crates.Add(Enumerable.Range(0, inputLine.Length / ChunkSize + 1)
                .Select(i => inputLine.Length <= i * ChunkSize + ChunkSize ?
                    inputLine[(i * ChunkSize)..]
                    : inputLine.Substring(i * ChunkSize, ChunkSize))
                .Select(s => s
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Trim())
                .ToArray());
        }

        _inputPiles = new Dictionary<int, List<string>>();
        for (int i = 0; i < stackNumbers.Count; i++)
        {
            _inputPiles.Add(stackNumbers[i], crates
                .Select(c => c[i])
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList());
        }

        Regex regex = new Regex("^move (?<nbrOfCrates>\\d+) from (?<pileNbrFrom>\\d+) to (?<pileNbrTo>\\d+)$");
        _inputMoves = inputLines
            .Select(inputLine => regex.Match(inputLine))
            .Where(m => m.Success)
            .Select(m => (
                int.Parse(m.Groups["nbrOfCrates"].Value), 
                int.Parse(m.Groups["pileNbrFrom"].Value), 
                int.Parse(m.Groups["pileNbrTo"].Value)))
            .ToList();
    }

    public override ValueTask<string> Solve_1()
    {
        Dictionary<int, List<string>> inputCopy = CreateCopyOfInput();

        foreach ((int nbrOfCrates, int from, int to) move in _inputMoves)
        {
            for (int i = 0; i < move.nbrOfCrates; i++)
            {
                string crate = inputCopy[move.from].Last();
                inputCopy[move.from].RemoveAt(inputCopy[move.from].LastIndexOf(crate));
                inputCopy[move.to].Add(crate);
            }
        }

        return new ValueTask<string>($"Crates on top: {CratesOnTop(inputCopy)}");
    }

    public override ValueTask<string> Solve_2()
    {
        Dictionary<int, List<string>> inputCopy = CreateCopyOfInput();

        foreach ((int nbrOfCrates, int from, int to) move in _inputMoves)
        {
            List<string> last = inputCopy[move.from].TakeLast(move.nbrOfCrates).ToList();
            inputCopy[move.to].AddRange(last);
            inputCopy[move.from].RemoveRange(inputCopy[move.from].Count - move.nbrOfCrates, move.nbrOfCrates);
        }

        return new ValueTask<string>($"Crates on top: {CratesOnTop(inputCopy)}");
    }

    private Dictionary<int, List<string>> CreateCopyOfInput()
    {
        return _inputPiles.ToDictionary(pile => 
                pile.Key, 
                pile => pile.Value.Select(s => new string(s)).ToList());
    }

    private static string CratesOnTop(Dictionary<int, List<string>> inputCopy)
    {
        List<string> cratesOnTop = inputCopy
            .Select(i => i.Value.LastOrDefault())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        return string.Join(string.Empty, cratesOnTop);
    }
}