using UnityEngine;

public class ActionPlayer : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField]
    [Tooltip("プレイヤーの移動速度")]
    private float m_moveSpeed = 5f;

    [SerializeField]
    [Tooltip("ジャンプ力")]
    private float m_jumpForce = 10f;

    [Header("地面判定")]
    [SerializeField]
    [Tooltip("地面判定用のタグ")]
    private string m_groundTag = "Ground";

    [SerializeField]
    [Tooltip("地面と判定する法線の角度範囲（度）")]
    private float m_groundAngleThreshold = 45f;

    [Header("物理マテリアル")]
    [SerializeField]
    [Tooltip("通常時の物理マテリアル（摩擦あり）")]
    private PhysicsMaterial2D m_normalMaterial;

    [SerializeField]
    [Tooltip("空中時の物理マテリアル（摩擦なし）")]
    private PhysicsMaterial2D m_airMaterial;

    [Header("背景")]
    [SerializeField]
    private ParallaxBackground m_parallaxBackground;

    // 内部変数
    private bool m_isMoving;
    private bool m_isGrounded;
    private bool m_wasGrounded;
    private float m_inputX;
    private float m_lastMoveDirectionX;
    private ActionPlayerInputReceiver m_inputReceiver;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2d;
    private SpriteRenderer m_spriteRenderer;
    private Collider2D m_collider2d;
    private float m_jumpTime;  // ジャンプしてからの経過時間

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_inputReceiver = GetComponent<ActionPlayerInputReceiver>();
        m_rigidbody2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider2d = GetComponent<Collider2D>();

        // Rigidbody2Dの設定を強制
        m_rigidbody2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        m_rigidbody2d.interpolation = RigidbodyInterpolation2D.Interpolate;

        // 物理マテリアルが設定されていない場合は作成
        CreatePhysicsMaterials();
    }

    private void Start()
    {
        // 初期向きを右に設定
        m_lastMoveDirectionX = 1f;

        // 初期状態は通常マテリアル
        m_collider2d.sharedMaterial = m_normalMaterial;
    }

    private void Update()
    {
        // 入力の取得と処理
        GetInput();

        // ジャンプ処理
        HandleJump();

        // アニメーションの更新
        UpdateAnimation();

        // 物理マテリアルの切り替え
        UpdatePhysicsMaterial();
    }

    private void FixedUpdate()
    {
        // 物理演算による移動
        Move();
    }

    /// <summary>
    /// 物理マテリアルを作成（Inspector未設定時用）
    /// </summary>
    private void CreatePhysicsMaterials()
    {
        if (m_normalMaterial == null)
        {
            m_normalMaterial = new PhysicsMaterial2D("Normal");
            m_normalMaterial.friction = 0.4f;
            m_normalMaterial.bounciness = 0f;
        }

        if (m_airMaterial == null)
        {
            m_airMaterial = new PhysicsMaterial2D("Air");
            m_airMaterial.friction = 0f;
            m_airMaterial.bounciness = 0f;
        }
    }

    /// <summary>
    /// 物理マテリアルを状況に応じて切り替え
    /// </summary>
    private void UpdatePhysicsMaterial()
    {
        // ジャンプ直後は必ず空中マテリアル
        if (Time.time - m_jumpTime < 0.1f)
        {
            m_collider2d.sharedMaterial = m_airMaterial;
            return;
        }

        // 地面にいる → 通常マテリアル（摩擦あり）
        // 空中にいる → 空中マテリアル（摩擦なし）
        if (m_isGrounded && !m_wasGrounded)
        {
            // 着地した瞬間
            m_collider2d.sharedMaterial = m_normalMaterial;
        }
        else if (!m_isGrounded && m_wasGrounded)
        {
            // 離陸した瞬間
            m_collider2d.sharedMaterial = m_airMaterial;
        }

        m_wasGrounded = m_isGrounded;
    }

    /// <summary>
    /// 地面と接触した時
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(m_groundTag))
        {
            CheckGroundContact(collision);
        }
    }

    /// <summary>
    /// 地面と接触している間
    /// </summary>
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(m_groundTag))
        {
            CheckGroundContact(collision);
        }
    }

    /// <summary>
    /// 地面から離れた時
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(m_groundTag))
        {
            m_isGrounded = false;
        }
    }

    /// <summary>
    /// 接触点の法線をチェックして地面判定
    /// </summary>
    private void CheckGroundContact(Collision2D collision)
    {
        bool foundGround = false;

        // 接触点をチェック
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 法線ベクトルが上向き（Vector2.up）に近いかチェック
            float angle = Vector2.Angle(contact.normal, Vector2.up);

            // 指定角度以内なら地面として判定
            if (angle <= m_groundAngleThreshold)
            {
                foundGround = true;
                break;
            }
        }

        m_isGrounded = foundGround;
    }

    /// <summary>
    /// 入力を取得して処理する
    /// </summary>
    private void GetInput()
    {
        // X方向の入力のみを取得
        Vector2 moveInput = m_inputReceiver.GetInputValue<Vector2>(ActionPlayerInputReceiver.Actions.MOVE);
        m_inputX = moveInput.x;

        // 移動判定
        if (Mathf.Abs(m_inputX) > 0.01f)
        {
            m_isMoving = true;
            m_lastMoveDirectionX = m_inputX;
        }
        else
        {
            m_isMoving = false;
        }
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void HandleJump()
    {
        // ジャンプ入力を取得（ボタンが押された瞬間）
        bool jumpInput = m_inputReceiver.GetActionInput(ActionPlayerInputReceiver.Actions.JUMP, ActionPlayerInputReceiver.InputType.PRESSED);

        // 地面にいる時のみジャンプ可能
        if (jumpInput && m_isGrounded)
        {
            Jump();
        }
    }

    /// <summary>
    /// ジャンプを実行
    /// </summary>
    private void Jump()
    {
        // ジャンプ時刻を記録
        m_jumpTime = Time.time;

        // 即座に空中マテリアルに切り替え（壁との摩擦を無効化）
        m_collider2d.sharedMaterial = m_airMaterial;

        // Y方向の速度をリセットしてからジャンプ力を加える
        Vector2 currentVelocity = m_rigidbody2d.linearVelocity;
        m_rigidbody2d.linearVelocity = new Vector2(currentVelocity.x, 0f);
        m_rigidbody2d.AddForce(new Vector2(0f, m_jumpForce), ForceMode2D.Impulse);
    }

    /// <summary>
    /// Rigidbody2Dを使用した移動処理
    /// </summary>
    private void Move()
    {
        // X方向の移動速度を計算
        float moveVelocityX = m_inputX * m_moveSpeed;

        // Y方向の速度は維持（重力とジャンプのため）
        Vector2 currentVelocity = m_rigidbody2d.linearVelocity;
        m_rigidbody2d.linearVelocity = new Vector2(moveVelocityX, currentVelocity.y);

        // 背景の更新
        if (m_parallaxBackground != null)
        {
            m_parallaxBackground.StartScroll(transform.position);
        }
    }

    /// <summary>
    /// アニメーションの更新
    /// </summary>
    private void UpdateAnimation()
    {
        m_animator.SetBool("isMoving", m_isMoving);

        // キャラクターの向きを変更（SpriteRendererのFlipXで反転）
        if (Mathf.Abs(m_inputX) > 0.01f)
        {
            // 左に移動している場合はFlipXをtrue（反転）、右の場合はfalse（通常）
            m_spriteRenderer.flipX = m_inputX < 0;
        }
    }

    /// <summary>
    /// 移動速度を設定（外部から変更可能）
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        m_moveSpeed = Mathf.Max(0, speed);
    }

    /// <summary>
    /// ジャンプ力を設定
    /// </summary>
    public void SetJumpForce(float force)
    {
        m_jumpForce = Mathf.Max(0, force);
    }

    /// <summary>
    /// 現在の移動速度を取得
    /// </summary>
    public float GetMoveSpeed()
    {
        return m_moveSpeed;
    }

    /// <summary>
    /// 現在移動中かどうか
    /// </summary>
    public bool IsMoving()
    {
        return m_isMoving;
    }

    /// <summary>
    /// 地面にいるかどうか
    /// </summary>
    public bool IsGrounded()
    {
        return m_isGrounded;
    }
}