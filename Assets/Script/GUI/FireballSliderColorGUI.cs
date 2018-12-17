using UnityEngine;
using UnityEngine.UI;

public class FireballSliderColorGUI : BaseEntity
{
    [SerializeField]
    private Image m_ImageToChangeColor = null;

    [SerializeField]
    private Color m_DefaultColor = Color.yellow;

    [SerializeField]
    private Color m_OnFireStatusColor = Color.red;

    public void SetDefaultColor()
    {
        SetColor(m_DefaultColor);
    }

    public void SetOnFireColor()
    {
        SetColor(m_OnFireStatusColor);
    }

    private void SetColor(Color color)
    {
        m_ImageToChangeColor.color = color;
    }

}
