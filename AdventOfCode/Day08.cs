namespace AdventOfCode;

public class Day08 : BaseDay
{
    private byte[,] _input;
    private int _nbrOfRows;
    private int _nbrOfCols;

    public Day08()
    {
        string[] readAllLines = File.ReadAllLines(InputFilePath);
        _nbrOfRows = readAllLines.Length;
        _nbrOfCols = readAllLines[0].Length;
        _input = new byte[_nbrOfRows, _nbrOfCols];
        for (int rowIndex = 0; rowIndex < _nbrOfRows; rowIndex++)
        {
            for (int colIndex = 0; colIndex < _nbrOfCols; colIndex++)
            {
                _input[rowIndex, colIndex] = byte.Parse(readAllLines[rowIndex][colIndex].ToString());
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        bool IsVisible((int row, int col) tree)
        {
            if (TreeIsOnEdge(tree))
            {
                return true;
            }

            List<List<byte>> allTrees = TreesOnSameRowOrCol(tree).ToList();
            byte currentTreeHeight = _input[tree.row, tree.col];
            // Console.WriteLine($"{tree} (height = {currentTreeHeight}): {isVisible}");
            return allTrees
                .Any(at => at.All(t => t < currentTreeHeight));
        }

        int nbrOfTreesVisible = Enumerable.Range(0, _nbrOfRows)
            .SelectMany(rowIndex => Enumerable.Range(0, _nbrOfCols).Select(colIndex => IsVisible((rowIndex, colIndex))))
            .Count(b => b);

        return new ValueTask<string>($"Nbr of trees visible: {nbrOfTreesVisible}");
    }

    public override ValueTask<string> Solve_2()
    {
        long ScenicScore((int row, int col) tree)
        {
            if (TreeIsOnEdge(tree))
            {
                return 0;
            }

            List<List<byte>> allTrees = TreesOnSameRowOrCol(tree).ToList();
            List<long> treesVisibleFromCurrentTree = new List<long>();
            byte currentTreeHeight = _input[tree.row, tree.col];
            foreach (List<byte> treeInLine in allTrees)
            {
                int index = treeInLine.FindIndex(t => t >= currentTreeHeight) + 1;
                if (index == 0)
                {
                    // none found => all trees are smaller
                    index = treeInLine.Count;
                }
                treesVisibleFromCurrentTree.Add(index);
            }
            
            return treesVisibleFromCurrentTree.Aggregate(1L, (current, nbrOfTrees) => current * nbrOfTrees);
        }

        long maxScenicScore = Enumerable.Range(0, _nbrOfRows)
            .SelectMany(rowIndex => Enumerable.Range(0, _nbrOfCols).Select(colIndex => ScenicScore((rowIndex, colIndex))))
            .Max();

        return new ValueTask<string>($"Highest scenic score: {maxScenicScore}");
    }

    private bool TreeIsOnEdge((int row, int col) tree)
    {
        return tree.row == 0 || tree.row == _nbrOfRows - 1 || tree.col == 0 || tree.col == _nbrOfCols - 1;
    }
    
    private IEnumerable<List<byte>> TreesOnSameRowOrCol((int row, int col) currentTree)
    {
        // reverse, so we can get a list from the tree towards the border
        if (currentTree.row > 0) yield return Enumerable.Range(0, currentTree.row).Reverse().Select(rowIdx => _input[rowIdx, currentTree.col]).ToList();
        if (currentTree.row < _nbrOfRows - 1) yield return Enumerable.Range(currentTree.row + 1, _nbrOfRows - currentTree.row - 1).Select(rowIdx => _input[rowIdx, currentTree.col]).ToList();
        // reverse, so we can get a list from the tree towards the border
        if (currentTree.col > 0) yield return Enumerable.Range(0, currentTree.col).Reverse().Select(colIdx => _input[currentTree.row, colIdx]).ToList();
        if (currentTree.col < _nbrOfCols - 1) yield return Enumerable.Range(currentTree.col + 1, _nbrOfCols - currentTree.col - 1).Select(colIdx => _input[currentTree.row, colIdx]).ToList();
    }

}