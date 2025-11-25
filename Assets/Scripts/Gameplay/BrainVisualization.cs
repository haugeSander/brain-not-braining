using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using BrainNotBraining.Core;

namespace BrainNotBraining.Gameplay
{
    /// <summary>
    /// Displays a minimalist, constellation-style brain visualization.
    /// Shows which brain regions are lit up/unlocked.
    /// </summary>
    public class BrainVisualization : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Transform nodesContainer; // Parent for node sprites

        [Header("Node Prefab")]
        [SerializeField] private GameObject nodePrefab; // Simple UI Image (circle sprite)

        [Header("Visual Settings")]
        [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        [SerializeField] private Color activeColor = new Color(0, 1, 1, 1f); // Cyan glow
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmount = 0.2f;

        // Brain structure data
        private Dictionary<BrainRegion, List<BrainNode>> brainRegions = new Dictionary<BrainRegion, List<BrainNode>>();
        private List<BrainNode> allNodes = new List<BrainNode>();
        private BrainRegion currentlyLitRegion = BrainRegion.None;
        private float animationProgress = 0f;
        private bool isAnimating = false;

        private class BrainNode
        {
            public GameObject gameObject;
            public Image image;
            public Vector2 position;
            public BrainRegion region;
            public float baseScale = 1f;
        }

        private void Awake()
        {
            // Start hidden
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Initialize the brain structure with all nodes
        /// </summary>
        public void Initialize()
        {
            if (nodesContainer == null || nodePrefab == null)
            {
                Debug.LogError("BrainVisualization: Missing references!");
                return;
            }

            // Create brain structure
            CreateBrainStructure();

            Debug.Log($"BrainVisualization: Initialized with {allNodes.Count} nodes");
        }

        /// <summary>
        /// Creates the minimalist brain node structure
        /// </summary>
        private void CreateBrainStructure()
        {
            // Define brain region positions (relative to canvas)
            // Brainstem: Bottom center
            // MotorCortex: Middle-left
            // Somatosensory: Middle-center
            // VisualCortex: Middle-back
            // Prefrontal: Top-front

            // Brainstem nodes (bottom, 5 nodes in vertical line)
            CreateRegion(BrainRegion.Brainstem, new Vector2[]
            {
                new Vector2(0, -150),
                new Vector2(0, -120),
                new Vector2(0, -90),
                new Vector2(-20, -70),
                new Vector2(20, -70)
            });

            // Motor Cortex nodes (middle-left, 6 nodes)
            CreateRegion(BrainRegion.MotorCortex, new Vector2[]
            {
                new Vector2(-80, -20),
                new Vector2(-100, 10),
                new Vector2(-90, 40),
                new Vector2(-70, 70),
                new Vector2(-50, 90),
                new Vector2(-60, 50)
            });

            // Somatosensory Cortex nodes (middle-center, 7 nodes)
            CreateRegion(BrainRegion.Somatosensory, new Vector2[]
            {
                new Vector2(-30, 0),
                new Vector2(-10, 30),
                new Vector2(10, 30),
                new Vector2(30, 0),
                new Vector2(0, 60),
                new Vector2(-20, 60),
                new Vector2(20, 60)
            });

            // Visual Cortex nodes (back, 6 nodes)
            CreateRegion(BrainRegion.VisualCortex, new Vector2[]
            {
                new Vector2(50, -10),
                new Vector2(70, 20),
                new Vector2(80, 50),
                new Vector2(90, 80),
                new Vector2(60, 70),
                new Vector2(70, 40)
            });

            // Prefrontal Cortex nodes (top, 8 nodes)
            CreateRegion(BrainRegion.Prefrontal, new Vector2[]
            {
                new Vector2(-40, 100),
                new Vector2(-20, 120),
                new Vector2(0, 130),
                new Vector2(20, 120),
                new Vector2(40, 100),
                new Vector2(-10, 110),
                new Vector2(10, 110),
                new Vector2(0, 100)
            });

            // Set all nodes to inactive initially
            foreach (var node in allNodes)
            {
                node.image.color = inactiveColor;
            }
        }

        /// <summary>
        /// Creates nodes for a specific brain region
        /// </summary>
        private void CreateRegion(BrainRegion region, Vector2[] positions)
        {
            List<BrainNode> regionNodes = new List<BrainNode>();

            foreach (var pos in positions)
            {
                GameObject nodeObj = Instantiate(nodePrefab, nodesContainer);
                RectTransform rect = nodeObj.GetComponent<RectTransform>();
                rect.anchoredPosition = pos;
                rect.localScale = Vector3.one;

                Image img = nodeObj.GetComponent<Image>();
                if (img == null)
                {
                    img = nodeObj.AddComponent<Image>();
                }

                BrainNode node = new BrainNode
                {
                    gameObject = nodeObj,
                    image = img,
                    position = pos,
                    region = region,
                    baseScale = 1f
                };

                regionNodes.Add(node);
                allNodes.Add(node);
            }

            brainRegions[region] = regionNodes;
        }

        /// <summary>
        /// Show the brain visualization and light up a specific region
        /// </summary>
        public void Show(BrainRegion regionToLight, string title, string description)
        {
            gameObject.SetActive(true);
            currentlyLitRegion = regionToLight;
            animationProgress = 0f;
            isAnimating = true;

            // Set text
            if (titleText != null)
                titleText.text = title;

            if (descriptionText != null)
                descriptionText.text = description;

            // Fade in canvas
            if (canvasGroup != null)
            {
                StartCoroutine(FadeInCanvasGroup());
            }

            // Start lighting animation
            StartCoroutine(LightUpRegionCoroutine(regionToLight));

            Debug.Log($"BrainVisualization: Showing region {regionToLight}");
        }

        /// <summary>
        /// Hide the brain visualization
        /// </summary>
        public void Hide()
        {
            if (canvasGroup != null)
            {
                StartCoroutine(FadeOutCanvasGroup());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator FadeInCanvasGroup()
        {
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                }
                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private System.Collections.IEnumerator FadeOutCanvasGroup()
        {
            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                }
                yield return null;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Animates lighting up a brain region
        /// </summary>
        private System.Collections.IEnumerator LightUpRegionCoroutine(BrainRegion region)
        {
            if (!brainRegions.ContainsKey(region))
            {
                Debug.LogWarning($"BrainVisualization: Region {region} not found!");
                yield break;
            }

            List<BrainNode> nodes = brainRegions[region];
            float delayBetweenNodes = 0.1f;

            // Light up each node in sequence
            for (int i = 0; i < nodes.Count; i++)
            {
                BrainNode node = nodes[i];
                StartCoroutine(LightUpNode(node));
                yield return new WaitForSecondsRealtime(delayBetweenNodes);
            }

            isAnimating = false;
            Debug.Log($"BrainVisualization: Region {region} fully lit");
        }

        /// <summary>
        /// Animates a single node lighting up
        /// </summary>
        private System.Collections.IEnumerator LightUpNode(BrainNode node)
        {
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Fade from inactive to active color
                node.image.color = Color.Lerp(inactiveColor, activeColor, t);

                // Scale up slightly
                float scale = Mathf.Lerp(0.5f, 1f, t);
                node.gameObject.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            node.image.color = activeColor;
            node.gameObject.transform.localScale = Vector3.one;
        }

        private void Update()
        {
            // Pulse effect for active nodes
            if (currentlyLitRegion != BrainRegion.None && brainRegions.ContainsKey(currentlyLitRegion))
            {
                float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;

                foreach (var node in brainRegions[currentlyLitRegion])
                {
                    float scale = 1f + pulse;
                    node.gameObject.transform.localScale = Vector3.one * scale;
                }
            }
        }

        /// <summary>
        /// Get explanation text for a brain region
        /// </summary>
        public static string GetRegionDescription(BrainRegion region)
        {
            switch (region)
            {
                case BrainRegion.Brainstem:
                    return "Autonomic functions online:\nBreathing • Heartbeat • Reflexes";
                case BrainRegion.MotorCortex:
                    return "Motor control activated:\nMovement • Coordination • Balance";
                case BrainRegion.Somatosensory:
                    return "Sensory processing enabled:\nTouch • Spatial awareness • Proprioception";
                case BrainRegion.VisualCortex:
                    return "Visual cortex awakened:\nSight • Depth perception • Color recognition";
                case BrainRegion.Prefrontal:
                    return "Higher cognition unlocked:\nReasoning • Memory • Problem solving";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get title text for a brain region
        /// </summary>
        public static string GetRegionTitle(BrainRegion region)
        {
            switch (region)
            {
                case BrainRegion.Brainstem:
                    return "The Brainstem Awakens";
                case BrainRegion.MotorCortex:
                    return "Motor Cortex Online";
                case BrainRegion.Somatosensory:
                    return "Somatosensory Cortex Activated";
                case BrainRegion.VisualCortex:
                    return "Visual Cortex Awakened";
                case BrainRegion.Prefrontal:
                    return "Prefrontal Cortex Unlocked";
                default:
                    return "Brain Region Unlocked";
            }
        }
    }
}
