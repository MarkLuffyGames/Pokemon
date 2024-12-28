using UnityEngine;

public class MaintainAspectRatio : MonoBehaviour
{
    private float targetAspectRatio = 3f/2f;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        AdjustAspectRatio(); // Ajusta al iniciar el juego
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            AdjustAspectRatio(); // Reajusta cuando cambia la resolución
        }
    }

    private void AdjustAspectRatio()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspectRatio;

        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            // Bordes horizontales (letterbox)
            Rect rect = camera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            camera.rect = rect;
        }
        else
        {
            // Bordes verticales (pillarbox)
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            camera.rect = rect;
        }
    }

}


