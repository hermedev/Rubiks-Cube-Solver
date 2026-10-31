using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the 2D unfolded Cube Map (HUD / Mini-map) in the top-left of the screen.
/// In Camera-View mode, the center square on the 2D net dynamically reflects
/// whatever face the player is currently looking directly at in 3D!
/// </summary>
public class CubeMap : MonoBehaviour
{
    private CubeState cubeState;
    public Transform up;
    public Transform down;
    public Transform left;
    public Transform right;
    public Transform Front;
    public Transform back;

    public void set()
    {
        cubeState = FindAnyObjectByType<CubeState>();
        if (cubeState == null) return;

        UpdateMap(GetFace('F'), Front);
        UpdateMap(GetFace('B'), back);
        UpdateMap(GetFace('L'), left);
        UpdateMap(GetFace('R'), right);
        UpdateMap(GetFace('U'), up);
        UpdateMap(GetFace('D'), down);
    }

    private List<GameObject> GetFace(char logicalFace)
    {
        char physicalFace = logicalFace;
        if (CubeMoveExecutor.Instance != null)
        {
            physicalFace = CubeMoveExecutor.Instance.ResolveFaceByCamera(logicalFace);
        }

        switch (physicalFace)
        {
            case 'U': return cubeState.up;
            case 'D': return cubeState.Down;
            case 'F': return cubeState.front;
            case 'B': return cubeState.back;
            case 'L': return cubeState.left;
            case 'R': return cubeState.right;
            default:  return cubeState.front;
        }
    }

    private void UpdateMap(List<GameObject> face, Transform side)
    {
        if (face == null || side == null) return;

        int i = 0;
        foreach (Transform map in side)
        {
            if (i >= face.Count) break;

            GameObject piece = face[i];
            if (piece != null && !string.IsNullOrEmpty(piece.name))
            {
                Image img = map.GetComponent<Image>();
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
            i++;
        }
    }
}
