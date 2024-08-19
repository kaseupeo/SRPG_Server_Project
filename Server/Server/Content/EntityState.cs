namespace Server;

[Flags]
public enum EntityState
{
    ShowRange   = 1 << 0,
    Move        = 1 << 1,
    Attack      = 1 << 2,
    EndTurn     = 1 << 3,
    Waiting     = 1 << 4,
}