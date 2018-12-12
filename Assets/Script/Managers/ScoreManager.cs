
[System.Flags]
public enum ShotStatus
{
    NONE = 0,
    PERFECT = 1 << 0,
    NORMAL = 1 << 1,
    BONUS = 1 << 2,
    FAILED = 1 << 3
}

public class ScoreManager : BaseEntity
{
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

    private void OnBallScore(BallScoreData scoreInformation)
    {
        string debugMessage = " scored. {0}";
        if(scoreInformation.thrower.GetType() == typeof(UserBallThrower))
        {
            m_Score.playerScore += GetScoreFromStatus(scoreInformation.status);
            LogMessage("player" + debugMessage, m_Score.playerScore);
            return;
        }

        m_Score.AIScore += GetScoreFromStatus(scoreInformation.status);
        LogMessage("ai" + debugMessage, m_Score.AIScore);
    }

}
