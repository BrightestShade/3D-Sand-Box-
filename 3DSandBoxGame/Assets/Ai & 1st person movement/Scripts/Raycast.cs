using UnityEngine;

public class Raycast : MonoBehaviour
{

    public LayerMask raycastMask;
    public float maxDistance;
    public GameObject Player;

    public float destroyDistance;
    RaycastHit objectJustPinged;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        RaycastHit objectJustPinged;
     
        if (Physics.Raycast(this.transform.position, Vector3.forward, out objectJustPinged, 10f, raycastMask)) //100f can also be Mathf.infinity it can be any number 
        {
    
            Debug.Log("Hit, !!! : " + objectJustPinged.collider.gameObject.name);
            
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward); // direction 

        /*if (Physics.Raycast(transform.position, forward, 6)) // distance 
        {
             Debug.Log("Hit!!! 5 metres");
        }*/


    }
    private void Update()
    {
        
        
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 80;
            Debug.DrawRay(transform.position, transform.forward, Color.green);

            if (Player != null)
            {
                Vector3 directionPlayer = Player.transform.position - transform.position;

                if (Physics.Raycast(transform.position, directionPlayer, out objectJustPinged, maxDistance, raycastMask))
                {
                    if (objectJustPinged.collider.gameObject == Player && objectJustPinged.distance <= destroyDistance)
                    {
                        Debug.Log("YOU died");
                    }
                    else
                    {
                        Destroy(Player);
                        Debug.Log("Hit " + objectJustPinged.collider.gameObject.name);
                        bool isAlive = false;
                    }

                }
            }
        

    }

}