using System.Collections;
using UnityEngine;
using TMPro;

public class ShieldBombSection : MonoBehaviour
{
    [SerializeField] private TMP_Text shieldCountText;

    private Color shieldTextColor;
    
    [SerializeField] private TMP_Text bombCountText;

    private Color bombTextColor;

    void Start()
    {
        shieldTextColor = shieldCountText.color;
        bombTextColor = bombCountText.color;

        ShieldController.UpdateShieldCount += OnShieldChanged;
        BombController.UpdateBombCount += OnBombChanged;
    }

    private void OnShieldChanged(int newCount)
    {
        if (newCount < 0) 
        {
            StartCoroutine(ShakeText(0.3f, 4f, shieldCountText, shieldTextColor));
            return;
        }
        shieldCountText.text = newCount.ToString();
    }

    private void OnBombChanged(int newCount)
    {
        if (newCount < 0) 
        {
            StartCoroutine(ShakeText(0.3f, 4f, bombCountText, bombTextColor));
            return;
        }
        bombCountText.text = newCount.ToString();
    }

    public IEnumerator ShakeText(float duration, float magnitude, TMP_Text text, Color originalColor)
    {
        Vector3 originalPosition = transform.localPosition;
        text.color = Color.red; 

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        text.color = shieldTextColor; 
        transform.localPosition = originalPosition;
    }
}
