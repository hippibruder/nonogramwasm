namespace nonogramwasm.Data;

public readonly record struct Score(TimeSpan GameDuration, int RevealedWrongCount);
