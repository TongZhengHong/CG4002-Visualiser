using UnityEngine;
using TMPro;

public class KillDeathSection : MonoBehaviour
{
    private int killCount = 0;

    private int deathCount = 0;

    [SerializeField] private TMP_Text killText;

    [SerializeField] private TMP_Text deathText;

    public void IncrementKillCount()
    {
        killCount++;
        killText.text = killCount.ToString();
    }

    public void IncrementDeathCount()
    {
        deathCount++;
        deathText.text = deathCount.ToString();
    }
    
    public void UpdateKillCount(int count)
    {
        killCount = count;
    }

    public void UpdateDeathCount(int count)
    {
        deathCount = count;
    }
}
