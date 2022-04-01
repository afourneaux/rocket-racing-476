public class ScoreData
{
    private string name;
    private int accuracyScore = 0;
    private int timeScore = 0;

    public ScoreData(string name) { this.name = name; }

    public string GetName() { return name; }
    public int GetTotalScore() { return accuracyScore - timeScore; }

    public int GetAccuracyScore() { return accuracyScore; }
    public void SetAccuracyScore(int accuracy)
    {
        accuracyScore = accuracy;
    }

    public int GetTimeScore() { return timeScore; }
    public void SetTimeScore(int time)
    {
        timeScore = time;
    }
}
