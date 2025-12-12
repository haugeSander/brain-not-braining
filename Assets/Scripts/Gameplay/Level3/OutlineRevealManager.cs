using UnityEngine;
using System.Collections.Generic;
using BrainNotBraining.Gameplay;
using BrainNotBraining.Core;

/// <summary>
/// Manages visual outline reveals for echolocation and proximity detection.
/// Creates temporary line renderers at hit points that fade out over time.
/// Handles both point-based reveals (echolocation) and object-based reveals (proximity).
/// </summary>
public class OutlineRevealManager : MonoBehaviour
{
    [Header("Outline Visual Configuration")]
    [Tooltip("Material for outline rendering (use an unlit/emissive material)")]
    public Material outlineMaterial;

    [Tooltip("Default width of outline lines")]
    public float defaultLineWidth = 0.02f;

    [Tooltip("Default color for outlines (monochrome white)")]
    public Color outlineColor = new Color(1f, 1f, 1f, 1f);

    [Tooltip("Number of segments for circular outline around hit point")]
    public int circleSegments = 8;

    [Tooltip("Radius of outline circle at hit point")]
    public float outlineRadius = 0.3f;

    [Header("Performance")]
    [Tooltip("Maximum number of active reveals at once")]
    public int maxActiveReveals = 100;

    [Tooltip("Use object pooling for line renderers")]
    public bool usePooling = true;

    [Header("Echo Particles (BLIND VR Style)")]
    [Tooltip("Particle prefab to spawn at pulse hit points")]
    public GameObject echoParticlePrefab;

    [Tooltip("Spawn particles on pulse hits (BLIND VR style)")]
    public bool spawnEchoParticles = true;

    [Tooltip("Use LineRenderer outlines (legacy system)")]
    public bool useLineRendererOutlines = false;

    // Pooling
    private Queue<LineRenderer> lineRendererPool = new Queue<LineRenderer>();
    private List<RevealInstance> activeReveals = new List<RevealInstance>();

    // Proximity reveals (persistent while in range)
    private Dictionary<GameObject, ObjectReveal> proximityReveals = new Dictionary<GameObject, ObjectReveal>();

    private void Start()
    {
        // Pre-populate pool if using pooling
        if (usePooling)
        {
            for (int i = 0; i < 20; i++)
            {
                LineRenderer lr = CreateLineRenderer();
                lr.gameObject.SetActive(false);
                lineRendererPool.Enqueue(lr);
            }
        }
    }

    private void Update()
    {
        // Update active reveals (fade and cleanup)
        for (int i = activeReveals.Count - 1; i >= 0; i--)
        {
            RevealInstance reveal = activeReveals[i];

            if (Time.time >= reveal.expirationTime)
            {
                // Remove expired reveal
                ReturnLineRenderer(reveal.lineRenderer);
                activeReveals.RemoveAt(i);
            }
            else if (Time.time >= reveal.fadeStartTime)
            {
                // Fade out
                float fadeProgress = (Time.time - reveal.fadeStartTime) / reveal.fadeDuration;
                float alpha = Mathf.Lerp(1f, 0f, fadeProgress);

                Color fadedColor = outlineColor;
                fadedColor.a = alpha;

                reveal.lineRenderer.startColor = fadedColor;
                reveal.lineRenderer.endColor = fadedColor;
            }
        }
    }

    /// <summary>
    /// Reveals an outline at a specific point (used by echolocation pulses).
    /// </summary>
    public void RevealOutlineAt(Vector3 position, Vector3 normal, float duration, float fadeDuration)
    {
        // Spawn echo particles (BLIND VR style)
        if (spawnEchoParticles && echoParticlePrefab != null)
        {
            Instantiate(echoParticlePrefab, position, Quaternion.LookRotation(normal));
        }

        // Legacy LineRenderer outlines (optional)
        if (!useLineRendererOutlines)
        {
            return; // Skip LineRenderer if disabled
        }

        // Check capacity
        if (activeReveals.Count >= maxActiveReveals)
        {
            return; // Skip if at capacity
        }

        // Get line renderer
        LineRenderer lr = GetLineRenderer();
        if (lr == null)
        {
            return;
        }

        // Create circular outline in plane perpendicular to normal
        Vector3[] circlePoints = GenerateCirclePoints(position, normal, outlineRadius, circleSegments);

        // Configure line renderer
        lr.positionCount = circlePoints.Length;
        lr.SetPositions(circlePoints);
        lr.startWidth = defaultLineWidth;
        lr.endWidth = defaultLineWidth;
        lr.startColor = outlineColor;
        lr.endColor = outlineColor;
        lr.loop = true;
        lr.gameObject.SetActive(true);

        // Add to active reveals
        RevealInstance reveal = new RevealInstance
        {
            lineRenderer = lr,
            fadeStartTime = Time.time + (duration - fadeDuration),
            expirationTime = Time.time + duration,
            fadeDuration = fadeDuration
        };

        activeReveals.Add(reveal);
    }

    /// <summary>
    /// Reveals an object's edges based on proximity (used by proximity detection).
    /// </summary>
    public void RevealObjectProximity(GameObject obj, float intensity, float lineWidth)
    {
        if (obj == null)
        {
            return;
        }

        // Check if already revealing this object
        if (proximityReveals.ContainsKey(obj))
        {
            // Update intensity
            UpdateProximityReveal(obj, intensity, lineWidth);
        }
        else
        {
            // Create new proximity reveal
            CreateProximityReveal(obj, intensity, lineWidth);
        }
    }

    /// <summary>
    /// Hides proximity reveal for an object.
    /// </summary>
    public void HideObjectProximity(GameObject obj)
    {
        if (obj == null || !proximityReveals.ContainsKey(obj))
        {
            return;
        }

        ObjectReveal reveal = proximityReveals[obj];

        // Return line renderer to pool
        if (reveal.lineRenderer != null)
        {
            ReturnLineRenderer(reveal.lineRenderer);
        }

        proximityReveals.Remove(obj);
    }

    /// <summary>
    /// Creates a new proximity reveal for an object.
    /// </summary>
    private void CreateProximityReveal(GameObject obj, float intensity, float lineWidth)
    {
        // Get or create line renderer
        LineRenderer lr = GetLineRenderer();
        if (lr == null)
        {
            return;
        }

        // Get object bounds
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null)
        {
            // No renderer, try to get bounds from collider
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
            {
                ReturnLineRenderer(lr);
                return;
            }

            // Create outline from collider bounds
            CreateBoundsOutline(lr, col.bounds, intensity, lineWidth);
        }
        else
        {
            // Create outline from renderer bounds
            CreateBoundsOutline(lr, renderer.bounds, intensity, lineWidth);
        }

        lr.gameObject.SetActive(true);

        // Store proximity reveal
        ObjectReveal reveal = new ObjectReveal
        {
            obj = obj,
            lineRenderer = lr,
            intensity = intensity
        };

        proximityReveals[obj] = reveal;
    }

    /// <summary>
    /// Updates an existing proximity reveal.
    /// </summary>
    private void UpdateProximityReveal(GameObject obj, float intensity, float lineWidth)
    {
        ObjectReveal reveal = proximityReveals[obj];
        reveal.intensity = intensity;

        // Update line renderer color based on intensity
        Color fadedColor = outlineColor;
        fadedColor.a = intensity;

        reveal.lineRenderer.startColor = fadedColor;
        reveal.lineRenderer.endColor = fadedColor;
        reveal.lineRenderer.startWidth = lineWidth;
        reveal.lineRenderer.endWidth = lineWidth;
    }

    /// <summary>
    /// Creates an outline around object bounds.
    /// </summary>
    private void CreateBoundsOutline(LineRenderer lr, Bounds bounds, float intensity, float lineWidth)
    {
        // Create box outline from bounds
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
        corners[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[4] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[5] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[6] = center + new Vector3(extents.x, extents.y, extents.z);
        corners[7] = center + new Vector3(-extents.x, extents.y, extents.z);

        // Create edge lines (bottom square, top square, vertical edges)
        Vector3[] edgePoints = new Vector3[16];
        // Bottom square
        edgePoints[0] = corners[0];
        edgePoints[1] = corners[1];
        edgePoints[2] = corners[2];
        edgePoints[3] = corners[3];
        // Top square
        edgePoints[4] = corners[4];
        edgePoints[5] = corners[5];
        edgePoints[6] = corners[6];
        edgePoints[7] = corners[7];
        // Vertical edges
        edgePoints[8] = corners[0];
        edgePoints[9] = corners[4];
        edgePoints[10] = corners[1];
        edgePoints[11] = corners[5];
        edgePoints[12] = corners[2];
        edgePoints[13] = corners[6];
        edgePoints[14] = corners[3];
        edgePoints[15] = corners[7];

        lr.positionCount = edgePoints.Length;
        lr.SetPositions(edgePoints);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        Color fadedColor = outlineColor;
        fadedColor.a = intensity;

        lr.startColor = fadedColor;
        lr.endColor = fadedColor;
        lr.loop = false;
    }

    /// <summary>
    /// Generates circle points in a plane perpendicular to normal.
    /// </summary>
    private Vector3[] GenerateCirclePoints(Vector3 center, Vector3 normal, float radius, int segments)
    {
        Vector3[] points = new Vector3[segments];

        // Find perpendicular vectors
        Vector3 right = Vector3.Cross(normal, Vector3.up);
        if (right.sqrMagnitude < 0.001f)
        {
            right = Vector3.Cross(normal, Vector3.forward);
        }
        right.Normalize();

        Vector3 forward = Vector3.Cross(right, normal).normalized;

        // Generate circle points
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = (right * Mathf.Cos(angle) + forward * Mathf.Sin(angle)) * radius;
            points[i] = center + offset;
        }

        return points;
    }

    /// <summary>
    /// Gets a line renderer from pool or creates new one.
    /// </summary>
    private LineRenderer GetLineRenderer()
    {
        if (usePooling && lineRendererPool.Count > 0)
        {
            return lineRendererPool.Dequeue();
        }

        return CreateLineRenderer();
    }

    /// <summary>
    /// Returns a line renderer to the pool.
    /// </summary>
    private void ReturnLineRenderer(LineRenderer lr)
    {
        if (lr == null)
        {
            return;
        }

        lr.gameObject.SetActive(false);

        if (usePooling)
        {
            lineRendererPool.Enqueue(lr);
        }
        else
        {
            Destroy(lr.gameObject);
        }
    }

    /// <summary>
    /// Creates a new line renderer GameObject.
    /// </summary>
    private LineRenderer CreateLineRenderer()
    {
        GameObject lineObj = new GameObject("OutlineReveal");
        lineObj.transform.SetParent(transform);

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = outlineMaterial;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lr.receiveShadows = false;
        lr.useWorldSpace = true;

        return lr;
    }

    /// <summary>
    /// Clears all active reveals.
    /// </summary>
    public void ClearAllReveals()
    {
        // Clear pulse reveals
        foreach (RevealInstance reveal in activeReveals)
        {
            ReturnLineRenderer(reveal.lineRenderer);
        }
        activeReveals.Clear();

        // Clear proximity reveals
        foreach (ObjectReveal reveal in proximityReveals.Values)
        {
            ReturnLineRenderer(reveal.lineRenderer);
        }
        proximityReveals.Clear();
    }

    private class RevealInstance
    {
        public LineRenderer lineRenderer;
        public float fadeStartTime;
        public float expirationTime;
        public float fadeDuration;
    }

    private class ObjectReveal
    {
        public GameObject obj;
        public LineRenderer lineRenderer;
        public float intensity;
    }
}
