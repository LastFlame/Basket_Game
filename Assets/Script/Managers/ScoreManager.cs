
[System.Flags]
public enum ShotStatus
{
    NONE = 0,
    PERFECT = 1 << 0,
    NORMAL = 1 << 1,
    BONUS = 1 << 2,
    FAILED = 1 << 3
}

public struct ScoreData
{
    public BallThrower thrower;
    public int nCurrentShotScore;
    public int nTotalThrowerScore;

    public ScoreData(BallThrower thrower, int nCurrentShotScore, int nTotalThrowerScore)
    {
        this.thrower = thrower;
        this.nCurrentShotScore = nCurrentShotScore;
        this.nTotalThrowerScore = nTotalThrowerScore;
    }

}

public class ScoreManager : BaseEntity
{
    public static event System.Action<ScoreData> OnScoreDataSet = null;

    private struct Score
    {
        public static Score Default
        {
            get { return new Score(0, 0); }
        }
        
        public Score(int playerScore, int AIScore)
        {
            this.playerScore = 0;
            this.AIScore = 0;
        }

        public int playerScore;
        public int AIScore;
    }

    private Score m_Score = Score.Default;

    private void Start()
    {
        BallEntity.OnScore += OnBallScore;
    }

    private void OnDestroy()
    {
        BallEntity.OnScore -= OnBallScore;
    }

    private int GetScoreFromStatus(ShotStatus shotStauts)
    {
        if(shotStauts == ShotStatus.FAILED)
        {
            return 0;
        }

        if(shotStauts == ShotStatus.PERFECT)
        {
            return (int)Constant.ShotScores.PERFECT;
        }

        if(shotStauts == ((ShotStatus.BONUS | ShotStatus.NORMAL) ) )
        {
            return (int)BackBoardEntity.GetCurrentScoreBonus;
        }

        return (int)Constant.ShotScores.NORMAL;
    }

    private void OnBallScore(BallShotData scoreInformation)
    {
        int nShotScore = GetScoreFromStatus(scoreInformation.status);
        BallThrower thrower = scoreInformation.thrower;
        
        int nThrowerTotalScore = 0;
        if(thrower.GetType() == typeof(UserBallThrower))
        {
            m_Score.playerScore += nShotScore;
            nThrowerTotalScore = m_Score.playerScore;
        }
        else
        {
            m_Score.AIScore += nShotScore;
            nThrowerTotalScore = m_Score.AIScore;
        }

        CallEvent(OnScoreDataSet, new ScoreData(thrower, nShotScore, nThrowerTotalScore));

    }



}
