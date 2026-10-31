using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates the 2D unfolded Cube Map (HUD / Mini-map) in the top-left of the screen.
/// Displays the true physical state of all 6 faces of the cube (54 stickers).
/// Guarantees that all 6 faces are unique with no duplicate colors.
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

        UpdateMap(cubeState.front, Front);
        UpdateMap(cubeState.back, back);
        UpdateMap(cubeState.left, left);
        UpdateMap(cubeState.right, right);
        UpdateMap(cubeState.up, up);
        UpdateMap(cubeState.Down, down);
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
