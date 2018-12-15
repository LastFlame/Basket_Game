using UnityEngine;
using System.Linq;

public class AIBallThrower : BallThrower
{
    [System.Serializable]
    private struct AIThrowerData
    {
        public Constant.AIDifficulty difficulty;
        public float successfulRate;
        public float minMagnitudeMultiplier;
        public float maxMagnitudeMultipleir;
        public float minXOffset;
        public float maxXOffset;

        public AIThrowerData(Constant.AIDifficulty difficulty, float successfulRate,
                             float minMagnitudeMultiplier, float maxMagnitudeMultipleir,
                             float minXOffset, float maxXOffset)
        {
            this.difficulty = difficulty;
            this.successfulRate = successfulRate;
            this.minMagnitudeMultiplier = minMagnitudeMultiplier;
            this.maxMagnitudeMultipleir = maxMagnitudeMultipleir;
            this.minXOffset = minXOffset;
            this.maxXOffset = maxXOffset;
        }
    }

    private static Constant.AIDifficulty m_ThrowerDifficulty = Constant.AIDifficulty.EASY;
    public static Constant.AIDifficulty ThrowerDifficulty
    {
        get { return m_ThrowerDifficulty; }

        set { m_ThrowerDifficulty = value; }
    }

    [SerializeField]
    private AIThrowerData[] m_AIData = new AIThrowerData[3];

    private AIThrowerData m_SelectedAIData = new AIThrowerData();

    public override void SetNextShot(ShotData data)
    {
        base.SetNextShot(data);

        Vector3 perfectForceV3 = data.forceToScoreV3;

        if (Random.Range(0f, 1f) <= m_SelectedAIData.successfulRate)
        {
            m_CurrentForceV3 = perfectForceV3;
        }
        else
        {

            float minMagnitude = data.forceMinMagnitude + m_SelectedAIData.minMagnitudeMultiplier;
            float maxMagnitude = data.forceMaxMagnitude - m_SelectedAIData.maxMagnitudeMultipleir;

            float minXOffset = perfectForceV3.x + m_SelectedAIData.maxXOffset;
            float maxXOffset = perfectForceV3.x - m_SelectedAIData.minXOffset;

            m_CurrentForceV3 = perfectForceV3.normalized * Random.Range(minMagnitude, maxMagnitude) + Vector3.right * Random.Range(minXOffset, maxXOffset);
        }

        base.ThrowEntity(m_CurrentForceV3);

    }

    protected override void Awake()
    {
        base.Awake();

        m_SelectedAIData = m_AIData
                           .FirstOrDefault(aiData => aiData.difficulty == m_ThrowerDifficulty);
                               
    }
}
