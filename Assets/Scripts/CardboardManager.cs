using UnityEngine;
using Google.XR.Cardboard;

public class CardBoardManager: MonoBehaviour
{
    private Google.XR.Cardboard.XRLoader cardboardLoader;

    void Start()
    {
        cardboardLoader = ScriptableObject.CreateInstance<Google.XR.Cardboard.XRLoader>();
    }

    public void LaunchGoogleCardboard() 
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            gameObject.SetActive(false);
            cardboardLoader.Initialize();
            cardboardLoader.Start();
        }
    }

    void Update()
    {
        if (Google.XR.Cardboard.Api.IsCloseButtonPressed)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                cardboardLoader.Stop();
                cardboardLoader.Deinitialize();
                gameObject.SetActive(true);
            }
        }
    }
}
