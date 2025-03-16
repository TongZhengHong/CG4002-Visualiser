using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class MazagineController: MonoBehaviour
{
    [SerializeField] private TMP_Text bulletCountText;

    private Color originalTextColor;

    [SerializeField] private GameObject[] bulletList;

    private int bulletCount = 0;

    private void Start()
    {
        bulletCount = bulletList.Length;
        foreach (GameObject bullet in bulletList)
        {
            bullet.SetActive(true);
        }
        originalTextColor = bulletCountText.color;
    }

    public void ShootBullet()
    {
        if (bulletCount == 0)
        {
            StartCoroutine(ShakeText(0.3f, 4f));
            return;
        }
        bulletCount--;
        bulletList[bulletCount].SetActive(false);
        bulletCountText.text = bulletCount.ToString();
    }

    public void ReloadBullets()
    {
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        bulletCountText.color = Color.green; 
        float interval = GunController.RELOAD_TIME / bulletList.Length;
        for (int i = 0; i < bulletList.Length; i++)
        {
            yield return new WaitForSeconds(interval);
            bulletList[i].SetActive(true);
            bulletCountText.text = (i + 1).ToString();
        }
        bulletCount = bulletList.Length;
        bulletCountText.color = originalTextColor; 
    }

    public void OnRespawn()
    {
        StopAllCoroutines();
        foreach (GameObject bullet in bulletList)
        {
            bullet.SetActive(true);
        }
        bulletCount = bulletList.Length;
        bulletCountText.text = bulletList.Length.ToString();
        bulletCountText.color = originalTextColor; 
    }

    public void SyncBullets(int bulletCount)
    {
        this.bulletCount = bulletCount;
        bulletCountText.text = bulletCount.ToString();
        for (int i = 0; i < bulletList.Length; i++)
        {
            bulletList[i].SetActive(i < bulletCount);
        }
    }

    public IEnumerator ShakeText(float duration, float magnitude)
    {
        Vector3 originalPosition = bulletCountText.gameObject.transform.localPosition;
        bulletCountText.color = Color.red; 

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            Vector3 newPos = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            bulletCountText.gameObject.transform.localPosition = newPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        bulletCountText.color = originalTextColor; 
        bulletCountText.gameObject.transform.localPosition = originalPosition;
    }
}
