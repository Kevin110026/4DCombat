using System;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]

[RequireComponent(typeof(MeshRenderer))]
public class SwordTrail : MonoBehaviour
{
    public GameObject _tip;
    public GameObject _base;
    public int trailLength = 15;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private int verticesEndIndex = 1;
    private int trianglesEndIndex = 1;
    private bool activated = false;

    public TrailRenderer trailRenderer;

    void Awake()
    {
        // transform.parent = transform.parent.parent;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        GameObject blade = transform.parent.Find("blade").gameObject;
        //  visual sword trail using trail renderer is discarded, however, it might still be used in hit-detection, so I just set material to invisible
        trailRenderer = blade.GetComponent<TrailRenderer>();
        trailRenderer.material = Resources.Load<Material>("Invisible");
        trailRenderer.widthMultiplier = 3f;
        trailRenderer.time = 0.12f;
        trailRenderer.minVertexDistance = 0.025f;
        trailRenderer.enabled = false;
    }

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[2 * (trailLength + 1)];
        triangles = new int[2 * 4 * 3 * trailLength];
        DeactivateAll();
    }

    // Update is called once per frame
    void LateUpdate()
    {

        int i = (verticesEndIndex - 1 + trailLength + 1) % (trailLength + 1);
        int j = verticesEndIndex;
        int k = trianglesEndIndex;
        vertices[j * 2 + 0] = _base.transform.position;
        vertices[j * 2 + 1] = _tip.transform.position;

        // Debug.Log(activated);
        if (activated)
        {
            triangles[k * 24 + 0] = i * 2 + 0;
            triangles[k * 24 + 1] = i * 2 + 1;
            triangles[k * 24 + 2] = j * 2 + 0;
            triangles[k * 24 + 5] = j * 2 + 0;
            triangles[k * 24 + 4] = i * 2 + 1;
            triangles[k * 24 + 3] = i * 2 + 0;

            triangles[k * 24 + 6] = i * 2 + 0;
            triangles[k * 24 + 7] = i * 2 + 1;
            triangles[k * 24 + 8] = j * 2 + 1;
            triangles[k * 24 + 9] = j * 2 + 1;
            triangles[k * 24 + 10] = i * 2 + 1;
            triangles[k * 24 + 11] = i * 2 + 0;

            triangles[k * 24 + 12] = j * 2 + 0;
            triangles[k * 24 + 13] = j * 2 + 1;
            triangles[k * 24 + 14] = i * 2 + 0;
            triangles[k * 24 + 15] = i * 2 + 0;
            triangles[k * 24 + 16] = j * 2 + 1;
            triangles[k * 24 + 17] = j * 2 + 0;

            triangles[k * 24 + 18] = j * 2 + 0;
            triangles[k * 24 + 19] = j * 2 + 1;
            triangles[k * 24 + 20] = i * 2 + 1;
            triangles[k * 24 + 21] = i * 2 + 1;
            triangles[k * 24 + 22] = j * 2 + 1;
            triangles[k * 24 + 23] = j * 2 + 0;
        }
        else
        {
            triangles[k * 24 + 0] = 0;
            triangles[k * 24 + 1] = 0;
            triangles[k * 24 + 2] = 0;
            triangles[k * 24 + 5] = 0;
            triangles[k * 24 + 4] = 0;
            triangles[k * 24 + 3] = 0;

            triangles[k * 24 + 6] = 0;
            triangles[k * 24 + 7] = 0;
            triangles[k * 24 + 8] = 0;
            triangles[k * 24 + 9] = 0;
            triangles[k * 24 + 10] = 0;
            triangles[k * 24 + 11] = 0;

            triangles[k * 24 + 12] = 0;
            triangles[k * 24 + 13] = 0;
            triangles[k * 24 + 14] = 0;
            triangles[k * 24 + 15] = 0;
            triangles[k * 24 + 16] = 0;
            triangles[k * 24 + 17] = 0;

            triangles[k * 24 + 18] = 0;
            triangles[k * 24 + 19] = 0;
            triangles[k * 24 + 20] = 0;
            triangles[k * 24 + 21] = 0;
            triangles[k * 24 + 22] = 0;
            triangles[k * 24 + 23] = 0;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        verticesEndIndex = (verticesEndIndex + 1) % (trailLength + 1);
        trianglesEndIndex = (trianglesEndIndex + 1) % trailLength;

        // transform.localPosition = -transform.parent.position;
        // transform.localRotation = Quaternion.Euler(-transform.parent.rotation.eulerAngles);
        // transform.localScale = new Vector3(1 / transform.parent.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = new Vector3(1 / transform.parent.lossyScale.x, 1 / transform.lossyScale.y, 1 / transform.lossyScale.z);
    }


    public void ActivateCharged()
    {
        verticesEndIndex = 1;
        trianglesEndIndex = 1;
        for (int i = 0; i < trailLength + 1; i++)
        {
            vertices[i * 2 + 0] = _base.transform.position;
            vertices[i * 2 + 1] = _tip.transform.position;
        }
        Array.Fill(triangles, 0);
        activated = true;
        GetComponent<MeshRenderer>().enabled = true;
    }

    public void DeactivateCharged()
    {
        activated = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void ActivateNormal()
    {
        trailRenderer.enabled = true;
        trailRenderer.Clear();
    }
    public void DeactivateNormal()
    {
        trailRenderer.enabled = false;
    }

    public void ActivateAll()
    {
        ActivateCharged();
        ActivateNormal();
    }
    public void DeactivateAll()
    {
        DeactivateCharged();
        DeactivateNormal();
    }
}
