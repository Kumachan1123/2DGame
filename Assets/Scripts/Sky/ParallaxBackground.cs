/*
*       ParallaxBackground.cs
*       引用元：https://www.zkn0hr.com/unity-2d-parallax-background/
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ParallaxBackground : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]
    bool isInitialized = false;
    //あらかじめ各画像を紐付けしておく。
    [Header("背景画像 (0が最奥、順に手前)")]
    [SerializeField]
    Sprite[] backgroundSprites = new Sprite[3];
    //左右スクロール対応の場合は、各X値を-1920(1画像幅分)に設定してください。
    [Header("背景画像のオフセット (ズラす値)(左右スクロール対応の場合は1画像分、左にズラす)")]
    [SerializeField]
    Vector2[] backgroundOffsets = new Vector2[] {
        new Vector2(0, 0),
        new Vector2(0, -400),
        new Vector2(0, -350),
    };
    //サンプルで使用した、[Ansimuz] GothicVania Cemeteryの場合の画像サイズなので、自分の使用アセットに応じて調整してください。
    [Header("背景画像のサイズ")]
    [SerializeField]
    Vector2[] backgroundSpriteSizes = new Vector2[] {
        new Vector2(1920, 1120),
        new Vector2(1920, 1790),
        new Vector2(1920, 615),
    };
    [Header("背景画像のスクロール率 (奥(0)の物程小さめに指定)")]
    [SerializeField]
    float[] scrollRates = new float[] {
        1.0f,
        3.0f,
        5.0f,
    };
    //3Dオブジェクト(キャラクター等)より奥になるように調整。カメラの位置や3Dオブジェクトのサイズによるが30～設定すれば良い。
    [Header("カメラからUIへの距離 (カメラの位置や3Dオブジェクトのサイズによるが30～指定)")]
    [Range(30.0f, 100.0f)]
    [SerializeField]
    float planeDistance = 30.0f;
    [Header("背景画像を左右に何個配置するか (右スクロールのみなら2、左右スクロール対応なら3)")]
    [Range(2, 3)]
    [SerializeField]
    int imageMax = 2;
    [Header("スクロール時間")]
    [Range(0.1f, 3.0f)]
    [SerializeField]
    float scrollDuration = 1.0f;
    float smoothTime;
    [Header("スクロール速度の上限 (多分deltaTimeが掛かるので大きめに指定)")]
    [Range(500.0f, 10000.0f)]
    [SerializeField]
    float scrollSpeedMax = 1000.0f;
    //各背景画像のRectTransform。
    [HideInInspector]
    [SerializeField]
    RectTransform[] backgroundsRt;
    //背景画像数。
    [HideInInspector]
    [SerializeField]
    int backgroundMax;
    //各背景画像がスクロールした量。
    [HideInInspector]
    [SerializeField]
    float[] backgroundScrollValues;
    //RectMask2Dを有効にした状態で実行すると、スクロールしても画面外に設置した画像が非表示になる仕様っぽいので、実行時に有効化している。
    [HideInInspector]
    [SerializeField]
    RectMask2D parallaxBackgroundRectMask2D;
    //スクロール経過時間。
    float scrollElapsedTime;
    //スクロール加速度。SmoothDampに必要。
    [HideInInspector]
    [SerializeField]
    Vector2[] scrollVelocities;
    //コルーチンの管理に使用。
    Coroutine scroll;
    //前にスクロールが呼ばれた時のプレイヤーの位置。
    Vector3 previousPlayerPosition = Vector3.zero;
    //一時的に使用。
    Canvas parallaxBackgroundCanvas;
    GameObject parallaxBackgroundGo;
    RectTransform parallaxBackgroundRt;
    GameObject tempBackgroundGo;
    RectTransform tempBackgroundRt;
    Image tempBackgroundImg;
    Vector2 tempBackgroundPosition;
    Vector2 tempBackgroundsPosition;
    void Awake()
    {
        if (!isInitialized)
            CreateParallaxBackground();
        parallaxBackgroundRectMask2D.enabled = true;
        //SmoothDampのsmoothTimeと、スクロールの長さが厳密には違うので、一回り小さく計算しておく。
        smoothTime = scrollDuration * 0.85f;
    }
    //背景画像をスクロールしたい場合にコレを呼ぶ。引数にはプレイヤーの位置を渡す(位置差でなく)。
    public void StartScroll(Vector3 playerPosition)
    {
        //「右スクロールのみに対応」モードの時、プレイヤーが左に進んだ場合は無視する。
        if (imageMax == 2 && playerPosition.x - previousPlayerPosition.x < 0)
            return;
        //1画像分進んだ時、スクロールが繋がるように良い感じに戻している。
        for (int i = 0; i < backgroundMax; i++)
        {
            backgroundScrollValues[i] -= (playerPosition.x - previousPlayerPosition.x) * scrollRates[i];
            if (backgroundSpriteSizes[i].x < backgroundsRt[i].anchoredPosition.x)
            {
                backgroundScrollValues[i] -= backgroundSpriteSizes[i].x;
                tempBackgroundsPosition.Set(backgroundSpriteSizes[i].x, 0);
                backgroundsRt[i].anchoredPosition -= tempBackgroundsPosition;
            }
            else if (backgroundsRt[i].anchoredPosition.x < -backgroundSpriteSizes[i].x)
            {
                backgroundScrollValues[i] += backgroundSpriteSizes[i].x;
                tempBackgroundsPosition.Set(backgroundSpriteSizes[i].x, 0);
                backgroundsRt[i].anchoredPosition += tempBackgroundsPosition;
            }
        }
        //多重実行防止。
        if (scroll != null)
        {
            StopCoroutine(scroll);
        }
        scroll = StartCoroutine(Scroll());
        previousPlayerPosition = playerPosition;
    }
    IEnumerator Scroll()
    {
        scrollElapsedTime = 0;
        while (true)
        {
            scrollElapsedTime += Time.deltaTime;
            for (int i = 0; i < backgroundMax; i++)
            {
                tempBackgroundsPosition.Set(backgroundScrollValues[i], backgroundOffsets[i].y);
                backgroundsRt[i].anchoredPosition = Vector2.SmoothDamp(backgroundsRt[i].anchoredPosition, tempBackgroundsPosition, ref scrollVelocities[i], smoothTime, scrollSpeedMax);
            }
            if (scrollDuration <= scrollElapsedTime)
            {
                //SmoothDampはVelocityの値を参考にして現在の速度を出す為、初期化しておかないと次回実行時に動きが残る。
                for (int i = 0; i < backgroundMax; i++)
                {
                    scrollVelocities[i] = Vector2.zero;
                }
                scroll = null;
                yield break;
            }
            yield return null;
        }
    }
    //ステージクリア等で画像位置を強制的にリセットする時用。
    public void Reset()
    {
        for (int i = 0; i < backgroundMax; i++)
        {
            backgroundScrollValues[i] = 0;
            tempBackgroundsPosition.Set(backgroundScrollValues[i], backgroundOffsets[i].y);
            backgroundsRt[i].anchoredPosition = tempBackgroundsPosition;
        }
        for (int i = 0; i < backgroundMax; i++)
        {
            scrollVelocities[i] = Vector2.zero;
        }
        previousPlayerPosition = Vector3.zero;
        if (scroll != null)
        {
            StopCoroutine(scroll);
            scroll = null;
        }
    }
    //各種コンポーネントをアタッチし、背景画像等を生成。
    public void CreateParallaxBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0)
            return;
        backgroundMax = backgroundSprites.Length;
        parallaxBackgroundCanvas = GetComponent<Canvas>();
        if (parallaxBackgroundCanvas == null)
            return;
        backgroundsRt = new RectTransform[backgroundMax];
        scrollVelocities = new Vector2[backgroundMax];
        backgroundScrollValues = new float[backgroundMax];
        parallaxBackgroundCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        parallaxBackgroundCanvas.worldCamera = Camera.main;
        parallaxBackgroundCanvas.planeDistance = planeDistance;
        //ボタンを設置しないので、このCanvasへのタッチ判定を無効化しておく(インスペクターから削除しても良い)。
        GetComponent<GraphicRaycaster>().enabled = false;
        parallaxBackgroundGo = new GameObject("ParallaxBackground");
        parallaxBackgroundRt = parallaxBackgroundGo.AddComponent<RectTransform>();
        parallaxBackgroundRectMask2D = parallaxBackgroundGo.AddComponent<RectMask2D>();
        parallaxBackgroundRectMask2D.enabled = false;
        parallaxBackgroundRt.SetParent(transform);
        parallaxBackgroundRt.localScale = Vector3.one;
        parallaxBackgroundRt.localPosition = Vector3.zero;
        parallaxBackgroundRt.sizeDelta = gameObject.GetComponent<RectTransform>().sizeDelta;
        for (int i = 0; i < backgroundMax; i++)
        {
            backgroundsRt[i] = new GameObject(System.String.Format("Backgrounds{0}", i + 1)).AddComponent<RectTransform>();
            backgroundsRt[i].SetParent(parallaxBackgroundRt);
            backgroundsRt[i].localScale = Vector3.one;
            backgroundsRt[i].localPosition = Vector3.zero;
            tempBackgroundPosition.Set(0, backgroundOffsets[i].y);
            backgroundsRt[i].anchoredPosition = tempBackgroundPosition;
            for (int j = 0; j < imageMax; j++)
            {
                tempBackgroundGo = new GameObject(System.String.Format("Background{0}", i + 1));
                tempBackgroundRt = tempBackgroundGo.AddComponent<RectTransform>();
                tempBackgroundImg = tempBackgroundGo.AddComponent<Image>();
                tempBackgroundImg.sprite = backgroundSprites[i];
                tempBackgroundImg.raycastTarget = false;
                tempBackgroundRt.SetParent(backgroundsRt[i]);
                tempBackgroundRt.localScale = Vector3.one;
                tempBackgroundRt.localPosition = Vector3.zero;
                tempBackgroundRt.sizeDelta = backgroundSpriteSizes[i];
                tempBackgroundPosition.Set(backgroundOffsets[i].x + backgroundSpriteSizes[i].x * j, 0);
                tempBackgroundRt.anchoredPosition = tempBackgroundPosition;
            }
        }
        //カメラと平行に設置したい場合には、localRotationをリセットしておく。
        parallaxBackgroundRt.localRotation = Quaternion.identity;
        isInitialized = true;
    }
}