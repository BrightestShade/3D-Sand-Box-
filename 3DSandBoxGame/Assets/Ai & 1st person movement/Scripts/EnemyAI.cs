using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] GameObject target;
    
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            this.GetComponent<NavMeshAgent>().destination = target.transform.position;
        }
        
    }
   /*  private void OnTriggerEnter(Collider other)
    { 
        this.GetComponent<NavMeshAgent>().destination = target.transform.position;
    }
    private void OnTriggerStay(Collider other)
    {
        this.GetComponent<NavMeshAgent>().destination = target.transform.position;
    }*/
}

