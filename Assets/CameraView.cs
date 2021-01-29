using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask obstacleMask;

    public float meshResolution;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    private void LateUpdate()
    {
        DrawFieldOfView ();
    }

    void DrawFieldOfView()
    {
        //hoeveel lijnen wil je in je radius
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        for (int i=0; i<= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        //hoeveel driehoeken moeten we maken
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];

        // het aantal driehoeken is het aantal punten -2, de array is 3 keer zo lang voor de hoeken van de driehoeken
        int[] triangles = new int[(vertexCount - 2) * 3];

        //de eerste punt in de array is de origin van alle driehoeken
        vertices[0] = Vector3.zero;
        // de -1  is omdat we al een punt hebben bepaald in de array. zie hierboven
        for (int i=0; i< vertexCount - 1; i++)
        {
            // dit zijn de punten van de driehoek, hier kunnen we de positie van die punten vinden. omdat we in een array driehoeken gaan maken, moeten we in de array ze per 3 hebbem
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            // zodat we niet out of bounds gaan
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear ();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals ();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        //kijk of we iets raken
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        // draait alles mee met het character?
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        // hoe werken posities in Unity, tellen anders dan gewone radians
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        // what do you see boy
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;

        }
    }
}
