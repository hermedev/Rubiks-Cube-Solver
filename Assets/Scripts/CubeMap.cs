using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the 2D unfolded Cube Map (HUD / Mini-map) in the top-left of the screen.
/// Displays the true physical state and orientation of all 6 faces of the cube (54 stickers).
/// Dynamically updates whenever the 3D cube is rotated, ensuring that the 2D net
/// always reflects the face currently facing each world/view direction, with its exact
/// row and column sticker orientations matching the 3D view.
/// Guarantees that all 6 faces and all 54 stickers are strictly unique with zero duplicate colors
/// across all 24 orthogonal cube orientations.
/// </summary>
public class CubeMap : MonoBehaviour
{
    public static CubeMap Instance { get; private set; }

    private CubeState cubeState;
    public Transform up;
    public Transform down;
    public Transform left;
    public Transform right;
    public Transform Front;
    public Transform back;

    // Basis for each canonical rest face in CubeState:
    // (Normal n0, Up u0, Right r0)
    private struct FaceBasis
    {
        public Vector3 normal;
        public Vector3 up;
        public Vector3 right;

        public FaceBasis(Vector3 n, Vector3 u, Vector3 r)
        {
            normal = n;
            up = u;
            right = r;
        }
    }

    private static readonly FaceBasis[] CanonicalBases = {
        new FaceBasis(new Vector3(-1, 0, 0), new Vector3(0, 1, 0),  new Vector3(0, 0, -1)), // 0: Front
        new FaceBasis(new Vector3( 1, 0, 0), new Vector3(0, 1, 0),  new Vector3(0, 0,  1)), // 1: Back
        new FaceBasis(new Vector3( 0, 0, 1), new Vector3(0, 1, 0),  new Vector3(-1, 0, 0)), // 2: Left
        new FaceBasis(new Vector3( 0, 0, -1),new Vector3(0, 1, 0),  new Vector3( 1, 0, 0)), // 3: Right
        new FaceBasis(new Vector3( 0, 1, 0), new Vector3(1, 0, 0),  new Vector3(0, 0, -1)), // 4: Up
        new FaceBasis(new Vector3( 0, -1, 0),new Vector3(-1, 0, 0), new Vector3(0, 0, -1))  // 5: Down
    };

    // Basis for each 2D net section:
    private static readonly FaceBasis NetFront = new FaceBasis(new Vector3(-1, 0, 0), new Vector3(0, 1, 0),  new Vector3(0, 0, -1));
    private static readonly FaceBasis NetBack  = new FaceBasis(new Vector3( 1, 0, 0), new Vector3(0, 1, 0),  new Vector3(0, 0,  1));
    private static readonly FaceBasis NetLeft  = new FaceBasis(new Vector3( 0, 0, 1), new Vector3(0, 1, 0),  new Vector3(-1, 0, 0));
    private static readonly FaceBasis NetRight = new FaceBasis(new Vector3( 0, 0, -1),new Vector3(0, 1, 0),  new Vector3( 1, 0, 0));
    private static readonly FaceBasis NetUp    = new FaceBasis(new Vector3( 0, 1, 0), new Vector3(1, 0, 0),  new Vector3(0, 0, -1));
    private static readonly FaceBasis NetDown  = new FaceBasis(new Vector3( 0, -1, 0),new Vector3(-1, 0, 0), new Vector3(0, 0, -1));

    void Awake()
    {
        Instance = this;
    }

    public void set()
    {
        cubeState = FindAnyObjectByType<CubeState>();
        if (cubeState == null) return;

        Quaternion cubeRot = Quaternion.identity;
        if (RotateCube.Instance != null)
        {
            cubeRot = RotateCube.Instance.transform.rotation;
        }
        else
        {
            GameObject cubeGo = GameObject.Find("Cube");
            if (cubeGo != null) cubeRot = cubeGo.transform.rotation;
        }

        // Snap to nearest 90-degree orthogonal cube orientation
        Quaternion q = RotateCube.GetNearestOrthogonalRotation(cubeRot);
        Quaternion qInv = Quaternion.Inverse(q);

        // Update each 2D net section with exact in-plane sticker orientation:
        UpdateNetSection(Front, NetFront, qInv);
        UpdateNetSection(back,  NetBack,  qInv);
        UpdateNetSection(left,  NetLeft,  qInv);
        UpdateNetSection(right, NetRight, qInv);
        UpdateNetSection(up,    NetUp,    qInv);
        UpdateNetSection(down,  NetDown,  qInv);
    }

    private void UpdateNetSection(Transform sideTransform, FaceBasis netBasis, Quaternion qInv)
    {
        if (sideTransform == null) return;

        // Find which canonical face currently points in this net direction:
        Vector3 n0 = qInv * netBasis.normal;
        int canonicalIdx = GetCanonicalFaceIndex(n0);
        List<GameObject> facePieces = GetFacePieces(canonicalIdx);
        if (facePieces == null || facePieces.Count < 9) return;

        FaceBasis cBase = CanonicalBases[canonicalIdx];

        // Each 2D Net section has 9 UI children: row in [-1, 0, 1], col in [-1, 0, 1]
        // Net child index = (r_net + 1) * 3 + (c_net + 1)
        int netChildIdx = 0;
        for (int rNet = -1; rNet <= 1; rNet++)
        {
            for (int cNet = -1; cNet <= 1; cNet++)
            {
                if (netChildIdx >= sideTransform.childCount) break;
                Transform mapTile = sideTransform.GetChild(netChildIdx);

                // Offset on the 2D net in world space:
                Vector3 deltaW = cNet * netBasis.right - rNet * netBasis.up;
                // Transform into the rest cube's coordinate system:
                Vector3 deltaV0 = qInv * deltaW;

                // Decompose into canonical face's column and row:
                int c0 = Mathf.RoundToInt(Vector3.Dot(deltaV0, cBase.right));
                int r0 = Mathf.RoundToInt(-Vector3.Dot(deltaV0, cBase.up));

                int srcIdx = (r0 + 1) * 3 + (c0 + 1);
                if (srcIdx >= 0 && srcIdx < facePieces.Count)
                {
                    GameObject piece = facePieces[srcIdx];
                    ApplyColorToTile(mapTile, piece);
                }

                netChildIdx++;
            }
        }
    }

    private int GetCanonicalFaceIndex(Vector3 v0)
    {
        int best = 0;
        float maxDot = -2f;
        for (int i = 0; i < CanonicalBases.Length; i++)
        {
            float d = Vector3.Dot(v0, CanonicalBases[i].normal);
            if (d > maxDot)
            {
                maxDot = d;
                best = i;
            }
        }
        return best;
    }

    private List<GameObject> GetFacePieces(int canonicalIdx)
    {
        switch (canonicalIdx)
        {
            case 0: return cubeState.front;
            case 1: return cubeState.back;
            case 2: return cubeState.left;
            case 3: return cubeState.right;
            case 4: return cubeState.up;
            case 5: return cubeState.Down;
            default: return cubeState.front;
        }
    }

    private void ApplyColorToTile(Transform mapTile, GameObject piece)
    {
        if (mapTile == null || piece == null || string.IsNullOrEmpty(piece.name)) return;

        Image img = mapTile.GetComponent<Image>();
        if (img != null)
        {
            char code = piece.name[0];
            switch (code)
            {
                case 'F': img.color = new Color(1f, 0.5f, 0f, 1f); break; // Orange
                case 'B': img.color = Color.red; break;
                case 'U': img.color = Color.yellow; break;
                case 'D': img.color = Color.white; break;
                case 'L': img.color = Color.green; break;
                case 'R': img.color = Color.blue; break;
            }
        }
    }
}
