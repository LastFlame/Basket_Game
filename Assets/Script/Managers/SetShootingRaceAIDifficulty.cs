using UnityEngine;

public class SetShootingRaceAIDifficulty : BaseEntity
{
    [SerializeField]
    private Constant.AIDifficulty m_DifficultyToSet = Constant.AIDifficulty.EASY;
    
    public void SetDifficulty()
    {
        AIBallThrower.ThrowerDifficulty = m_DifficultyToSet;
    }
}