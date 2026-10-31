using UnityEngine;
using DG.Tweening;

/// <summary>
/// Controls whole-cube view rotation — smooth free 3D orbit and nearest 90° orthogonal snapping.
/// Works identically on PC (right-click drag) and mobile/touch (2-finger drag).
/// Players can freely rotate the cube around multiple axes, and upon release it seamlessly snaps
/// to the closest 90° face orientation right where they left it, rather than snapping backward.
/// </summary>
public class RotateCube : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────
    /// <summary>
    /// Optional logical target reference for inspector backwards-compatibility.
    /// </summary>
    public GameObject target;

    // ── Tween state ───────────────────────────────────────────────────────────
    private Tween snapTween;
    private const float SnapDuration = 0.28f; // snappy, smooth feel

    // Cardinal directions for 90° cube orientations
    private static readonly Vector3[] Cardinals = {
        Vector3.forward, Vector3.back,
        Vector3.up,      Vector3.down,
        Vector3.right,   Vector3.left
    };

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (CubeInputReader.Instance == null) return;
        CubeInputReader.Instance.OnSecondaryDelta    += HandleOrbit;
        CubeInputReader.Instance.OnSecondaryReleased += HandleSecondaryReleased;
    }

    void OnDestroy()
    {
        if (CubeInputReader.Instance == null) return;
        CubeInputReader.Instance.OnSecondaryDelta    -= HandleOrbit;
        CubeInputReader.Instance.OnSecondaryReleased -= HandleSecondaryReleased;
        snapTween?.Kill();
    }

    // ── Orbit (free 3D continuous rotation) ───────────────────────────────────

    /// <summary>
    /// Directly applies input delta to freely rotate the cube around any and all axes.
    /// Kills any running snap tween so dragging feels instantaneous and 1:1 responsive.
    /// </summary>
    private void HandleOrbit(Vector2 scaledDelta)
    {
        snapTween?.Kill();

        // Rotate in world space relative to camera view
        Quaternion rot = Quaternion.Euler(scaledDelta.y, -scaledDelta.x, 0);
        transform.rotation = rot * transform.rotation;

        if (target != null)
        {
            target.transform.rotation = transform.rotation;
        }
    }

    // ── Release / Snap ────────────────────────────────────────────────────────

    /// <summary>
    /// Called when the player releases right-click or finishes a swipe gesture.
    /// Snaps smoothly to the nearest valid 90° orthogonal cube orientation
    /// right where the player left it, supporting multi-turn and multi-axis spins.
    /// </summary>
    private void HandleSecondaryReleased(Vector2 swipeDir)
    {
        SnapToNearestOrientation();
    }

    /// <summary>
    /// Finds the closest 90° orthogonal orientation out of the 24 possible cube orientations
    /// and smoothly animates the cube into alignment.
    /// </summary>
    public void SnapToNearestOrientation()
    {
        Quaternion snappedRot = GetNearestOrthogonalRotation(transform.rotation);

        if (target != null)
        {
            target.transform.rotation = snappedRot;
        }

        snapTween?.Kill();
        snapTween = transform
            .DORotateQuaternion(snappedRot, SnapDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                snapTween = null;
                // Update 2D net dynamically to match the newly faced orientation!
                CubeMap cm = FindAnyObjectByType<CubeMap>();
                if (cm != null) cm.set();
            });
    }

    /// <summary>
    /// Calculates the closest 90-degree orthogonal cube rotation to the given orientation.
    /// </summary>
    public static Quaternion GetNearestOrthogonalRotation(Quaternion currentRot)
    {
        Vector3 currentForward = currentRot * Vector3.forward;
        Vector3 snappedForward = SnapToCardinal(currentForward);

        Vector3 currentUp = currentRot * Vector3.up;
        Vector3 snappedUp = SnapToCardinalOrtho(currentUp, snappedForward);

        return Quaternion.LookRotation(snappedForward, snappedUp);
    }

    private static Vector3 SnapToCardinal(Vector3 v)
    {
        Vector3 best = Vector3.forward;
        float maxDot = -2f;
        foreach (Vector3 c in Cardinals)
        {
            float d = Vector3.Dot(v, c);
            if (d > maxDot)
            {
                maxDot = d;
                best = c;
            }
        }
        return best;
    }

    private static Vector3 SnapToCardinalOrtho(Vector3 v, Vector3 normal)
    {
        Vector3 best = Vector3.up;
        float maxDot = -2f;
        foreach (Vector3 c in Cardinals)
        {
            // Must be strictly perpendicular to normal
            if (Mathf.Abs(Vector3.Dot(c, normal)) > 0.1f) continue;

            float d = Vector3.Dot(v, c);
            if (d > maxDot)
            {
                maxDot = d;
                best = c;
            }
        }
        return best;
    }
}
