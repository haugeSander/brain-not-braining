using UnityEngine;

public class ShockWave: MonoBehaviour
{
    public float speed = 0.2f; // Velocità di propagazione dell'onda
    private Vector3 direction;

    public void Initialize(Vector3 moveDirection)
    {
        direction = moveDirection;
        // Distrugge l'onda dopo un certo tempo per evitare oggetti persistenti
        Destroy(gameObject, 5f); 
    }

    void Update()
    {
        // Muove l'onda in avanti ogni frame
        if (speed == 0) speed = 0.2f;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    // Rileva quando l'onda entra in contatto con un Collider (grazie a Is Trigger)
    void OnParticleCollision(GameObject other)
    {
        

        if (other.gameObject.name == "Player"|| other.gameObject.name=="Floor" || other.gameObject.name=="ShockWave") 
            return;

        Debug.Log($"Hit: {other.gameObject.name} - Tag: {other.gameObject.tag}"); 
        // GetComponent<Collider>().enabled = false;
        
        
        Destroy(gameObject);
    }
}