using UnityEngine;

public class Block : MonoBehaviour
{
    public float blockSize = 30f; 
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        float halfSize = blockSize / 2f;
        Vector3 center = transform.position;
        Gizmos.DrawWireCube(center, new Vector3(blockSize, blockSize, 0));
    }
}