using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour
{
    private Transform target;
    protected NavMeshAgent agent;
    private Animator animator;
    private float timer;
    private bool followPlayer = true;
   
    void Start()
    {
        agent = GetComponent<NavMeshAgent>(); 
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        timer = 30f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            followPlayer = false;
            if(GameObject.FindGameObjectWithTag("redbox") != null)
            {
            //target = GameObject.FindGameObjectWithTag("redbox").GetComponent<Transform>();
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }else{
                target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            }

        }
        agent.SetDestination(target.position);
        float minDist = 2;
        float dist = Vector3.Distance(target.position, transform.position);
        if (dist <= 5.5f)
            animator.Play("fight");
        else
            animator.Play("run");
    }

}
