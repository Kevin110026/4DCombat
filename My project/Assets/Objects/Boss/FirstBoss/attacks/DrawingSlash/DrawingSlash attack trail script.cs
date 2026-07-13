using UnityEngine;

public class DrawingSlashattacktrailscript : MonoBehaviour
{
    private float createTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= createTime + 0.5)
            Destroy(gameObject);
    }
}
