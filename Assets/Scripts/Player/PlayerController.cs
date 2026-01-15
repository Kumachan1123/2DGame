using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float m_moveSpeed;

    bool m_isMoving;
    Vector2 m_input;
    PlayerInputReceiver m_inputReceiver;
    Animator m_animator;
    // 当たり判定を行うレイヤーマスク
    [SerializeField] LayerMask m_solidObjects;
    // 草むら判定のレイヤーマスク
    [SerializeField] LayerMask m_longGrass;
    // フェード管理クラス
    [SerializeField, Header("フェード")]
    private UIFade m_fade;
    // 戦闘システムオブジェクト
    [SerializeField] GameObject m_battleSystem;
    // エンカウントフラグ
    private bool m_isEncountered = false;

    public bool IsEncountered { get => m_isEncountered; set => m_isEncountered = value; }

    private void OnEnable()
    {
        m_fade.FadeInWithCallback(() =>
        {
            // フェードイン完了後の処理（必要ならここに追加）
        });
    }

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_inputReceiver = GetComponent<PlayerInputReceiver>();
        if (m_inputReceiver == null)
        {
            Debug.LogError("PlayerInputReceiverコンポーネントがアタッチされていません。");
        }
    }

    void Update()
    {
        // エンカウント済みなら何もしない
        if (IsEncountered) return;
        // 動いていない時
        if (!m_isMoving)
        {
            // キーボード入力を受け付ける
            m_input = m_inputReceiver.GetInputValue<Vector2>(PlayerInputReceiver.Actions.MOVE);
            // 斜め移動禁止:横方向の入力があれば, 縦は0にする
            if (m_input.x != 0)
            {
                m_input.y = 0;
            }
            // 入力があったら
            if (m_input != Vector2.zero)
            {
                CheckForEncounters();
                // アニメーション更新
                m_animator.SetFloat("moveX", m_input.x);
                m_animator.SetFloat("moveY", m_input.y);
                // 入力分を追加
                Vector2 targetPos = transform.position;
                targetPos += m_input;
                if (IsWalkable(targetPos)) StartCoroutine(Move(targetPos));
            }
        }
        m_animator.SetBool("isMoving", m_isMoving);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        m_isMoving = true;

        // targetPosと現在のpisitionの差がある間は、MoveTowardsでtargetPosに近く
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, m_moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;

        m_isMoving = false;
    }

    // TargetPosに移動可能かどうかを調べる
    bool IsWalkable(Vector2 targetPos)
    {

        return !Physics2D.OverlapCircle(targetPos, 0.2f, m_solidObjects);
    }
    // 自分の場所から円のレイを飛ばして、草むらに当たったらランダムエンカウント
    void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, m_longGrass))
        {
            // ランダムエンカウント
            if (Random.Range(0, 100) < 10)
            {
                Debug.Log("エンカウント発生！");
                IsEncountered = true;
                m_fade.FadeOutWithCallback(() =>
                {
                    StartCoroutine(EnterToBattle(0.2f));
                });
            }
        }
    }

    private IEnumerator EnterToBattle(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_battleSystem.SetActive(true);
    }

}