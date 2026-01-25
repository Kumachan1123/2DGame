/*
 *      CameraTracker.cs
 *      プレイヤーの動きを追従するカメラスクリプト
 */
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(m_player.transform.position.x, m_player.transform.position.y, transform.position.z);
    }
}
