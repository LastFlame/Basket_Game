using UnityEngine;
using System.Linq;

public class AIBallThrower : BallThrower
{
    [System.Serializable]
    private struct AIThrowerData
    {
        public Constant.AIDifficulty difficulty;

        [Range(0.5f, 3f)]
        public float throwDelay;

        [Range(0f,1f)]
        public float successfulRate;

        [Range(0f, 1f)]
        public float minMagnitudeMultiplier;

        [Range(0f, 1f)]
        public float maxMagnitudeMultipleir;

        [Range(0f, 1f)]
        public float minXOffset;

        [Range(0f, 1f)]
        public float maxXOffset;

        public AIThrowerData(Constant.AIDifficulty difficulty, float throwDelay,
                             float successfulRate, float minMagnitudeMultiplier, float maxMagnitudeMultipleir,
                             float minXOffset, float maxXOffset)
        {
            this.difficulty = difficulty;

            this.throwDelay = throwDelay;
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

    private System.Collections.IEnumerator m_OnThrowDelay = null;
    private AIThrowerData m_SelectedAIData = new AIThrowerData();

    public override void SetNextShot(ShotData data)
    {
        base.SetNextShot(data);

        m_IsReadyToBeStopped = true;
        Vector3 perfectForceV3 = data.forceToScoreV3;
        m_CurrentForceV3 = perfectForceV3;

        //set an offset to the final force if the successful % to
        //make a perfect shot isn't met.
        if (Random.Range(0f, 1f) > m_SelectedAIData.successfulRate)
        {
            //the min magnitude to apply on the ball is calculated from
            //the min force possible from the shooting point (we don't want the min force to be 0).
            //then do an addition to that magnitude (this parameter is set from editor based on the ai difficulty),
            //to make the ai seems more precise.
            float minMagnitude = data.forceMinMagnitude + m_SelectedAIData.minMagnitudeMultiplier;

            //we do the same things with the min magnitude to the max magnitude, but we insted lower the 
            //value of the max magnitude.
            float maxMagnitude = data.forceMaxMagnitude - m_SelectedAIData.maxMagnitudeMultipleir;

            //to make the ai seems more "human" we set an offset on the x axis of the final force.
            //this make the ai shoot more "randomly" on the x axis too.
            float minXOffset = perfectForceV3.x + m_SelectedAIData.maxXOffset;
            float maxXOffset = perfectForceV3.x - m_SelectedAIData.minXOffset;


            //the final force formula is almost the same as the player. we get the direction of the perfect shot vector,
            //and then we add a random range on the magnitude with the previous calculated values, and a random
            //offset on the x value.
            m_CurrentForceV3 = perfectForceV3.normalized * Random.Range(minMagnitude, maxMagnitude) + Vector3.right * Random.Range(minXOffset, maxXOffset);
        }


        //we delay the throw ball method invoke, otherwise,
        //the ai will be too fast compared with the user.
        //this will "mimic" the aiming stage of the user.
        StartCoroutine(m_OnThrowDelay = DelayThrowEntity());
    }

    protected override void Awake()
    {
        base.Awake();

        m_SelectedAIData = m_AIData
                           .FirstOrDefault(aiData => aiData.difficulty == m_ThrowerDifficulty);
                               
    }

    private System.Collections.IEnumerator DelayThrowEntity()
    {
        float waitingTime = m_SelectedAIData.throwDelay;

        //avoid the game manager to invoke change scene,
        //when the ai is doing his last shot.
        m_IsReadyToBeStopped = false;

        //we wait to subtract the delay time
        //when this entity is on pause.
        while (waitingTime >= 0)
        {
            if (!m_IsPaused)
            {
                waitingTime -= Time.deltaTime;
            }

            yield return null;
        }

        base.ThrowEntity(m_CurrentForceV3);
    }
}
