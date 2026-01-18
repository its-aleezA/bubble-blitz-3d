using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public float minY = -5f;
    public float maxY = 10f;
    
    private Transform target;
    private Vector3 desiredPosition;
    
    void Start()
    {
        // Find shooter as target
        GameObject shooter = GameObject.FindGameObjectWithTag("Shooter");
        if (shooter != null)
        {
            target = shooter.transform;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        desiredPosition = target.position + offset;
        
        // Clamp Y position
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        
        // Smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    
    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.1f)
    {
        StartCoroutine(Shake(duration, magnitude));
    }
    
    System.Collections.IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPos;
    }
}