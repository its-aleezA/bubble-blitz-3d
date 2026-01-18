using UnityEngine;
using System.Collections;

public class ShooterController : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float rotationSpeed = 150f;
    public float shootPower = 25f;
    public float maxAngle = 60f;
    public Transform firePoint;
    public GameObject bubblePrefab;
    public LineRenderer aimLine;
    
    [Header("Visual Settings")]
    public float aimLineLength = 8f;
    
    private float currentAngle = 0f;
    private GameObject loadedBubble;
    private bool canShoot = true;
    private Vector3 shootDirection;
    
    void Start()
    {
        // Make sure firePoint exists
        if (firePoint == null)
        {
            firePoint = transform;
        }
        
        LoadBubble();
        SetupAimLine();
    }
    
    void Update()
    {
        // 1. ROTATION
        float rotate = Input.GetAxis("Horizontal");
        currentAngle += rotate * rotationSpeed * Time.deltaTime;
        currentAngle = Mathf.Clamp(currentAngle, -maxAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        
        // 2. UPDATE AIM DIRECTION
        shootDirection = transform.up;
        
        // 3. UPDATE AIM LINE
        UpdateAimLine();
        
        // 4. SHOOTING - FIXED INPUT CHECK
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && canShoot && loadedBubble != null)
        {
            ShootBubble();
        }
    }
    
    void SetupAimLine()
    {
        if (aimLine != null)
        {
            aimLine.positionCount = 20; // More points for curved line
            aimLine.startWidth = 0.1f;
            aimLine.endWidth = 0.05f;
            aimLine.material = new Material(Shader.Find("Sprites/Default"));
            aimLine.startColor = Color.cyan;
            aimLine.endColor = Color.blue;
        }
    }
    
    void UpdateAimLine()
    {
        if (aimLine == null) return;
        
        // Create curved trajectory prediction
        Vector3 startPos = firePoint.position;
        Vector3 velocity = shootDirection * shootPower;
        Vector3 gravity = Physics.gravity;
        
        for (int i = 0; i < aimLine.positionCount; i++)
        {
            float t = i / (float)(aimLine.positionCount - 1);
            float time = t * 2f; // Predict 2 seconds ahead
            
            // Physics trajectory: position = start + velocity*time + 0.5*gravity*time²
            Vector3 point = startPos + (velocity * time) + (0.5f * gravity * time * time);
            aimLine.SetPosition(i, point);
            
            // Stop if hitting something
            RaycastHit hit;
            if (Physics.Raycast(startPos, (point - startPos).normalized, out hit, Vector3.Distance(startPos, point)))
            {
                // Change line color based on what we hit
                Bubble bubble = hit.collider.GetComponent<Bubble>();
                if (bubble != null)
                {
                    aimLine.startColor = Color.green;
                    aimLine.endColor = Color.green;
                }
                else
                {
                    aimLine.startColor = Color.red;
                    aimLine.endColor = Color.red;
                }
                break;
            }
        }
    }
    
    void LoadBubble()
    {
        if (firePoint == null || bubblePrefab == null || GameManager.Instance == null) 
        {
            Debug.LogError("Missing references in ShooterController!");
            return;
        }
        
        // Create bubble at fire point
        loadedBubble = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        
        // Get color from GameManager
        int colorIndex = GameManager.Instance.GetNextColorIndex();
        Color color = GameManager.Instance.GetColor(colorIndex);
        
        // Set bubble color
        Bubble bubbleScript = loadedBubble.GetComponent<Bubble>();
        if (bubbleScript != null)
        {
            bubbleScript.SetColor(colorIndex, color);
        }
        
        // Disable physics until shot
        Rigidbody rb = loadedBubble.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        // Add small bobbing animation
        StartCoroutine(BubbleBobAnimation());
    }
    
    IEnumerator BubbleBobAnimation()
    {
        Vector3 startPos = loadedBubble.transform.localPosition;
        float elapsed = 0f;
        
        while (loadedBubble != null && loadedBubble.GetComponent<Rigidbody>().isKinematic)
        {
            float bobY = Mathf.Sin(elapsed * 3f) * 0.1f;
            loadedBubble.transform.localPosition = startPos + new Vector3(0, bobY, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    void ShootBubble()
    {
        if (!canShoot || loadedBubble == null) return;
        
        canShoot = false;
        
        // Remove kinematic and enable physics
        Rigidbody rb = loadedBubble.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better collision
            rb.AddForce(shootDirection * shootPower, ForceMode.VelocityChange);
        }
        
        // Add trail effect
        TrailRenderer trail = loadedBubble.AddComponent<TrailRenderer>();
        trail.time = 0.3f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        
        // Get bubble's color for trail
        Bubble bubbleScript = loadedBubble.GetComponent<Bubble>();
        if (bubbleScript != null && GameManager.Instance != null)
        {
            Color bubbleColor = GameManager.Instance.GetColor(bubbleScript.colorIndex);
            trail.startColor = bubbleColor;
            trail.endColor = new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, 0f);
        }
        
        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BallShot();
        }
        
        // Clear reference
        loadedBubble = null;
        
        // Reload after delay
        StartCoroutine(ReloadAfterDelay());
    }
    
    IEnumerator ReloadAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        LoadBubble();
        canShoot = true;
    }
    
    void OnDrawGizmos()
    {
        // Visualize shoot direction in editor
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, firePoint.position + (transform.up * 3f));
            Gizmos.DrawSphere(firePoint.position + (transform.up * 3f), 0.2f);
        }
    }
}