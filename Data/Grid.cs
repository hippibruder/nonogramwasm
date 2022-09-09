namespace nonogramwasm.Data;
using System.Diagnostics.CodeAnalysis;
using nonogramwasm.Data.Extensions;

public class Grid
{
    public int Height { get; }
    public int Width { get; }
    public Hint[][] ColumnHints;
    public Hint[][] RowHints;
    private Cell[,] cells;

    public Grid(int height, int width)
    {
        this.Height = height;
        this.Width = width;
        InitCells();
        InitHints();
    }

    public void RevealGame() {
        foreach (var cell in Cells)
        {
            if (cell.State == CellState.Hidden || cell.State == CellState.Marked)
            {
                cell.State = CellState.Revealed;
            }
        }
    }

    public bool CheckGameOver() {
        bool areAllEmptyRevealed = Cells.Where(c => c.IsEmpty).All(c => c.State == CellState.Revealed);
        if (areAllEmptyRevealed)
        {
            RevealGame();
        }
        return areAllEmptyRevealed;
    }

    public IEnumerable<Cell> Cells
    {
        get
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    yield return this.cells[y, x];
                }
            }
        }
    }

    public IEnumerable<IEnumerable<Cell>> Rows
    {
        get
        {
            for (int y = 0; y < this.Height; y++)
            {
                yield return EnumerateRow(y);
            }
        }
    }

    public IEnumerable<IEnumerable<Cell>> Columns
    {
        get
        {
            for (int x = 0; x < this.Width; x++)
            {
                yield return EnumerateColumn(x);
            }
        }
    }

    /// Generate random empty cells. Candidates with already empty neighbours have higher chance to be empty. 
    [MemberNotNull(nameof(cells))]
    private void InitCells()
    {
        this.cells = new Cell[this.Height, this.Width];
        var indicies = new List<(int, int)>();
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                indicies.Add((y, x));
            }
        }
        indicies.Shuffle();
        foreach (var index in indicies)
        {
            var (y, x) = index;
            var isEmptyChance = 0.5;
            bool hasEmptyNeighbour = GetNeighbours(y, x).Any(c => c.IsEmpty);
            if (hasEmptyNeighbour)
            {
                isEmptyChance *= 1.5;
            }
            this.cells[y, x] = new Cell(Random.Shared.NextDouble() < isEmptyChance);
        }
    }

    [MemberNotNull(nameof(ColumnHints), nameof(RowHints))]
    private void InitHints()
    {
        this.ColumnHints = this.Columns.Select(cells => GetHints(cells).ToArray()).ToArray();
        this.RowHints = this.Rows.Select(cells => GetHints(cells).ToArray()).ToArray();
    }

    private IEnumerable<Cell> GetNeighbours(int y, int x)
    {
        var neighbourCoords = new (int, int)[] {
            (y+1, x),
            (y-1, x),
            (y, x+1),
            (y, x-1),
        }.Where(c => c.Item1 > 0 && c.Item2 > 0 
                    && c.Item1 < this.Height && c.Item2 < this.Width);
        return neighbourCoords.Select(c => this.cells[c.Item1, c.Item2])
                              .Where(cell => cell != null);
    }


    private IEnumerable<Cell> EnumerateRow(int y)
    {
        for (int x = 0; x < this.Width; x++)
        {
            yield return this.cells[y, x];
        }
    }

    private IEnumerable<Cell> EnumerateColumn(int x)
    {
        for (int y = 0; y < this.Height; y++)
        {
            yield return this.cells[y, x];
        }
    }

    static private IEnumerable<Hint> GetHints(IEnumerable<Cell> cells)
    {
        if (cells.All(c => !c.IsEmpty))
        {
            yield return new Hint(0);
            yield break;
        }

        int currentStreak = 0;
        foreach (var cell in cells)
        {
            if (cell.IsEmpty)
            {
                currentStreak++;
            }
            else if (currentStreak != 0)
            {
                yield return new Hint(currentStreak);
                currentStreak = 0;
            }
        }
        if (currentStreak != 0)
        {
            yield return new Hint(currentStreak);
        }
    }
}