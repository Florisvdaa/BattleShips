public class ScoreSystem
{
    public int PlayerHits { get; private set; }
    public int AIHits { get; private set; }

    public void AddPlayerHit() => PlayerHits++;
    public void AddAIHit() => AIHits++;
}
