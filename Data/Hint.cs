namespace nonogramwasm.Data;

public record class Hint(int Count)
{
    public bool Deactivated { get; set; } = false;
};