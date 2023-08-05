using UnityEngine;

//This script is used by both movement and jump to detect when the character is touching the ground

public class PlayerOnGround : MonoBehaviour
{
        public bool isOnGround { get; private set; }
       
        [Header("Collider Settings")]
        [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.95f;
        [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;

        [Header("Layer Masks")]
        [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;
 

        private void Update()
        {
            //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
            bool rc1 = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer);
            bool rc2 = Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
            isOnGround = (rc1||rc2);
        }

        private void OnDrawGizmos()
        {
            //Draw the ground colliders on screen for debug purposes
            if (isOnGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
            Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
            Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
        }

        
}