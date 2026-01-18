using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour
{
    public int colorIndex = 0;
    private Color bubbleColor;
    private bool isStuck = false;
    private Rigidbody rb;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Ensure bubble has proper components
        if (GetComponent<SphereCollider>() == null)
            gameObject.AddComponent<SphereCollider>().radius = 0.5f;
        
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
    }
    
    void Start()
    {
        // Apply material after a frame (lets GameManager initialize)
        StartCoroutine(DelayedMaterialApply());
    }
    
    IEnumerator DelayedMaterialApply()
    {
        yield return new WaitForEndOfFrame();
        ApplyMaterial();
    }
    
    public void SetColor(int index, Color color)
    {
        colorIndex = index;
        bubbleColor = color;
        ApplyMaterial();
    }
    
    void ApplyMaterial()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null)
        {
            rend = gameObject.AddComponent<MeshRenderer>();
        }
        
        // Create new material instance
        Material mat = new Material(Shader.Find("Standard"));
        
        // Set to transparent
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        
        // Set color with transparency
        Color finalColor = bubbleColor;
        if (finalColor == Color.clear && GameManager.Instance != null && 
            GameManager.Instance.colors.Length > colorIndex)
        {
            finalColor = GameManager.Instance.colors[colorIndex];
        }
        
        mat.color = new Color(finalColor.r, finalColor.g, finalColor.b, 0.85f);
        
        // Add emission for glow
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", finalColor * 1.5f);
        
        // Set specular
        mat.SetFloat("_Glossiness", 0.8f);
        mat.SetFloat("_Metallic", 0.1f);
        
        // Apply material
        rend.material = mat;
        
        // Update global illumination
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        
        Debug.Log($"Bubble color set: Index={colorIndex}, Color={finalColor}");
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (isStuck) return;
        
        Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();
        
        if (otherBubble != null)
        {
            Debug.Log($"Collision! This color: {colorIndex}, Other color: {otherBubble.colorIndex}");
            
            // Check color match
            if (colorIndex == otherBubble.colorIndex)
            {
                Debug.Log("Colors match! Looking for cluster...");
                // Find matching cluster
                StartCoroutine(CheckForMatchingCluster());
            }
            else
            {
                Debug.Log("Colors don't match - sticking");
                // Stick to other bubble
                StickToPosition();
            }
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ceiling"))
        {
            Debug.Log("Hit wall/ceiling - sticking");
            StickToPosition();
        }
    }
    
    void StickToPosition()
    {
        if (isStuck) return;
        
        isStuck = true;
        
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Bubble stuck at position: " + transform.position);
        
        // After sticking, check if this created a match
        Invoke("CheckStuckForMatches", 0.1f);
    }
    
    void CheckStuckForMatches()
    {
        // Find all bubbles of same color within radius
        Collider[] nearbyBubbles = Physics.OverlapSphere(transform.position, 1.5f);
        
        foreach (Collider col in nearbyBubbles)
        {
            Bubble bubble = col.GetComponent<Bubble>();
            if (bubble != null && bubble.colorIndex == colorIndex && bubble != this)
            {
                // Found a neighbor with same color - check for cluster
                CheckForMatchingCluster();
                return;
            }
        }
    }
    
    IEnumerator CheckForMatchingCluster()
    {
        yield return new WaitForSeconds(0.05f); // Small delay
        
        // Find all connected bubbles of same color
        System.Collections.Generic.List<Bubble> cluster = FindMatchingCluster();
        
        Debug.Log($"Found cluster of {cluster.Count} bubbles");
        
        if (cluster.Count >= 3)
        {
            // Pop all bubbles in cluster
            foreach (Bubble bubble in cluster)
            {
                if (bubble != null)
                {
                    bubble.PopBubble();
                    yield return new WaitForSeconds(0.05f); // Staggered popping
                }
            }
        }
        else
        {
            // Not enough matches, stick normally
            if (!isStuck)
            {
                StickToPosition();
            }
        }
    }
    
    System.Collections.Generic.List<Bubble> FindMatchingCluster()
    {
        System.Collections.Generic.List<Bubble> cluster = new System.Collections.Generic.List<Bubble>();
        System.Collections.Generic.HashSet<Bubble> visited = new System.Collections.Generic.HashSet<Bubble>();
        System.Collections.Generic.Queue<Bubble> queue = new System.Collections.Generic.Queue<Bubble>();
        
        queue.Enqueue(this);
        
        while (queue.Count > 0)
        {
            Bubble current = queue.Dequeue();
            
            if (visited.Contains(current)) continue;
            if (current.colorIndex != colorIndex) continue;
            
            visited.Add(current);
            cluster.Add(current);
            
            // Find adjacent bubbles
            Collider[] adjacent = Physics.OverlapSphere(current.transform.position, 1.2f);
            foreach (Collider col in adjacent)
            {
                Bubble neighbor = col.GetComponent<Bubble>();
                if (neighbor != null && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return cluster;
    }
    
    public void PopBubble()
    {
        // Visual effect before destruction
        StartCoroutine(PopAnimation());
        
        // Add score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(100);
            GameManager.Instance.BubbleDestroyed(gameObject);
        }
        
        // Destroy after animation
        Destroy(gameObject, 0.2f);
    }
    
    IEnumerator PopAnimation()
    {
        // Scale down animation
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;
        
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    void OnDestroy()
    {
        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BubbleDestroyed(gameObject);
        }
    }
}