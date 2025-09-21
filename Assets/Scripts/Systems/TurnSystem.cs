public enum Turn { Player, AI }
public class TurnSystem
{
    public Turn Current { get; private set; } = Turn.Player;
    public void Next() => Current = (Current == Turn.Player) ? Turn.AI : Turn.Player;
}