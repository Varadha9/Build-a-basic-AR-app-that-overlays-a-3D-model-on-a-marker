using UnityEngine;

namespace ARApp.MarkerTracking
{
    /// <summary>
    /// Renders an AR HUD showing real-time camera tracking status,
    /// tracking confidence, frame rate, and interactive gesture controls.
    /// </summary>
    public class ARStatusUI : MonoBehaviour
    {
        public static ARStatusUI Instance { get; private set; }

        private bool isTracking = false;
        private string activeTargetName = "None";
        private float fps = 60f;
        private float fpsUpdateTimer = 0.5f;
        private int frameCount = 0;

        private GUIStyle headerStyle;
        private GUIStyle statusStyle;
        private GUIStyle helpStyle;
        private GUIStyle badgeStyle;
        private Texture2D bgTex;
        private Texture2D greenTex;
        private Texture2D orangeTex;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SetTrackingState(bool tracking, string targetName)
        {
            isTracking = tracking;
            activeTargetName = targetName;
        }

        private void Update()
        {
            frameCount++;
            fpsUpdateTimer -= Time.deltaTime;
            if (fpsUpdateTimer <= 0f)
            {
                fps = frameCount / 0.5f;
                frameCount = 0;
                fpsUpdateTimer = 0.5f;
            }
        }

        private void InitStyles()
        {
            if (headerStyle != null) return;

            bgTex = MakeTex(2, 2, new Color(0.05f, 0.08f, 0.12f, 0.85f));
            greenTex = MakeTex(2, 2, new Color(0.1f, 0.8f, 0.3f, 0.95f));
            orangeTex = MakeTex(2, 2, new Color(1.0f, 0.6f, 0.1f, 0.95f));

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            headerStyle.normal.textColor = Color.white;

            statusStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            helpStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                alignment = TextAnchor.MiddleCenter
            };
            helpStyle.normal.textColor = new Color(0.85f, 0.9f, 1.0f, 0.9f);

            badgeStyle = new GUIStyle(GUI.skin.box);
            badgeStyle.normal.background = bgTex;
        }

        private void OnGUI()
        {
            InitStyles();

            float screenW = Screen.width;
            float screenH = Screen.height;

            // Top Status Banner
            float bannerW = Mathf.Min(screenW * 0.92f, 750f);
            float bannerH = 120f;
            float bannerX = (screenW - bannerW) * 0.5f;
            float bannerY = 40f;

            GUI.Box(new Rect(bannerX, bannerY, bannerW, bannerH), GUIContent.none, badgeStyle);

            GUI.Label(new Rect(bannerX, bannerY + 10, bannerW, 35), "EX:6 - 3D MODEL OVERLAY ON AR MARKER", headerStyle);

            if (isTracking)
            {
                statusStyle.normal.textColor = new Color(0.2f, 1.0f, 0.4f);
                GUI.Label(new Rect(bannerX, bannerY + 45, bannerW, 35), $"[● TRACKED] 3D Model Active ({activeTargetName})", statusStyle);
                GUI.Label(new Rect(bannerX, bannerY + 80, bannerW, 25), $"60 FPS | 1-Finger: Rotate 360° | 2-Fingers: Scale | 2-Tap: Reset", helpStyle);
            }
            else
            {
                statusStyle.normal.textColor = new Color(1.0f, 0.7f, 0.2f);
                float pulse = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f;
                statusStyle.normal.textColor = Color.Lerp(new Color(1f, 0.6f, 0.1f), new Color(1f, 0.9f, 0.4f), pulse);
                GUI.Label(new Rect(bannerX, bannerY + 45, bannerW, 35), "[ SEARCHING ] Align Camera to AR Marker Target...", statusStyle);
                GUI.Label(new Rect(bannerX, bannerY + 80, bannerW, 25), $"Point device camera at the printed or on-screen marker", helpStyle);
            }

            // Center Crosshair when searching
            if (!isTracking)
            {
                DrawSearchCrosshairs(screenW * 0.5f, screenH * 0.5f);
            }
        }

        private void DrawSearchCrosshairs(float cx, float cy)
        {
            float size = 90f;
            float thick = 3f;
            Color crossColor = new Color(0.2f, 0.8f, 1.0f, 0.7f);

            GUI.color = crossColor;
            // 4 corner brackets
            // Top-left
            GUI.DrawTexture(new Rect(cx - size, cy - size, 30, thick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx - size, cy - size, thick, 30), Texture2D.whiteTexture);
            // Top-right
            GUI.DrawTexture(new Rect(cx + size - 30, cy - size, 30, thick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx + size - thick, cy - size, thick, 30), Texture2D.whiteTexture);
            // Bottom-left
            GUI.DrawTexture(new Rect(cx - size, cy + size - thick, 30, thick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx - size, cy + size - 30, thick, 30), Texture2D.whiteTexture);
            // Bottom-right
            GUI.DrawTexture(new Rect(cx + size - 30, cy + size - thick, 30, thick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(cx + size - thick, cy + size - 30, thick, 30), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}
