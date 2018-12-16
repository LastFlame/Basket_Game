public class GetWinnerEntity : BaseEntity
{
    public void NotifyRewardScene()
    {
        ScoreManager.Score matchScore = ScoreManager.RetriveMatchScore();

        WinnerGUI.Winner matchWinner = WinnerGUI.Winner.DRAW;

        if(matchScore.AIScore > matchScore.playerScore)
        {
            matchWinner = WinnerGUI.Winner.AI;
        }
        else if(matchScore.AIScore < matchScore.playerScore)
        {
            matchWinner = WinnerGUI.Winner.PLAYER;
        }

        WinnerGUI.SetWinnerGUI(matchWinner);
    }

    


}
