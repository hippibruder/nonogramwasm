namespace nonogramwasm.Data;

public enum CellState
{
    Hidden,
    Revealed,
    RevealedWrong,
    Marked
}

public class Cell
{
    public bool IsEmpty { get; }
    public CellState State { get; set;}

    public Cell(bool isEmpty)
    {
        this.IsEmpty = isEmpty;
        this.State = CellState.Hidden;
    }
}