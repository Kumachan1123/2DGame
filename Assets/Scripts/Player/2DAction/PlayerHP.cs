using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [Header("プレイヤーの初期残機")]
    [SerializeField] private int m_initialLives = 3;


    public void ResetLives()
    {
        m_initialLives = 3;
    }

    // 残機を減らす
    public void TakeDamage(int damage = 1)
    {
        m_initialLives -= damage;
        if (m_initialLives < 0) m_initialLives = 0;
    }

    public int InitialLives { get => m_initialLives; set => m_initialLives = value; }
}
