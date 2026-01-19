using UnityEngine;
using System.Collections;

public class BackgroundController : MonoBehaviour
{
    void Update()
    {
        transform.Translate(-0.1f, 0, 0);
        if (transform.position.x < -13.8f)
        {
            transform.position = new Vector3(13.8f, 0, 0);
        }
    }
}