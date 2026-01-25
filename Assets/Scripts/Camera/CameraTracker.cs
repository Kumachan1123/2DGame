/*
 *      CameraTracker.cs
 *      プレイヤーを少し遅れて追従するカメラ
 */
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [SerializeField] private GameObject m_player;     // 追従対象のプレイヤー
    [SerializeField] private float m_followSpeed = 5.0f; // 追従のなめらかさ

    void Update()
    {
        // プレイヤーの位置を取得
        Vector3 targetPos = new Vector3(
            m_player.transform.position.x,
            m_player.transform.position.y,
            transform.position.z
        );

        // 現在位置から目標位置へなめらかに移動
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            m_followSpeed * Time.deltaTime
        );
    }
}
