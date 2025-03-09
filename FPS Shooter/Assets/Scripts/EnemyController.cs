using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private List<Transform> PathNodes = new List<Transform>();
    [SerializeField] private float OrientationSpeed = 10f;
    [SerializeField] private float DetectionRadius = 10f;
    [SerializeField] private float AttackStopDistance = 5f;
    [Tooltip("Stop time, when enemy reached the node")] 
    [SerializeField] private float StopTime = 2f;

    private Health health;
    private NavMeshAgent agent;
    private WeaponController weapon;
    private Transform playerAimPoint;
    private Collider[] selfColliders;
    private Vector3 targetNodePosition;
    private int currentNodeIndex = 0;
    private float arrivalTimeThePoint;
    private bool isArrival;
    void Start()
    {
        playerAimPoint = FindFirstObjectByType<PlayerController>().AimPoint;
        selfColliders = GetComponentsInChildren<Collider>();
        weapon = GetComponentInChildren<WeaponController>();
        weapon.Owner = gameObject;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        health.OnDie += OnDie;

        SetNextNode();
    }

    void Update()
    {
        if (HandlePlayerDetection())
        {
            OrientTowards(playerAimPoint.position - transform.position);

            agent.destination = playerAimPoint.position;
            weapon.HandleShootInput(true, false);

            if(Vector3.Distance(transform.position, playerAimPoint.position) < AttackStopDistance)
                agent.velocity = Vector3.zero;
        }
        else
        {
            Patrol();
            if (agent.velocity != Vector3.zero)
                OrientTowards(agent.velocity);
        }
    }

    private void Patrol()
    {
        //TODO: fix this stupid code
        if (agent.destination != targetNodePosition)
            agent.destination = targetNodePosition;

        if ((targetNodePosition - transform.position).magnitude < 2f )
        {
            if(!isArrival)
            {
                arrivalTimeThePoint = Time.time;
                isArrival = true;
            }

            if(arrivalTimeThePoint + StopTime < Time.time)
                SetNextNode();
        }
    }

    private void OrientTowards(Vector3 lookPosition)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookPosition);
        transform.rotation =
            Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * OrientationSpeed);
    }

    private void SetNextNode()
    {
        targetNodePosition = PathNodes[currentNodeIndex].position;
        agent.destination = targetNodePosition;

        currentNodeIndex++;
        isArrival = false;

        if (currentNodeIndex >= PathNodes.Count)
            currentNodeIndex = 0;
    }

    private bool HandlePlayerDetection()
    {
        Vector3 directionToPlayer = (playerAimPoint.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, 
            DetectionRadius, -1, QueryTriggerInteraction.Ignore))
        {
            if (!selfColliders.Contains(hit.collider) && hit.collider.gameObject.GetComponent<PlayerController>())
                return true;
            else
                return false;
        }
        else
            return false;

    }

    private void OnDie()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < PathNodes.Count; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= PathNodes.Count)
                nextIndex = 0;

            Gizmos.DrawLine(PathNodes[i].position, PathNodes[nextIndex].position);
            Gizmos.DrawSphere(PathNodes[i].position, 0.1f);
        }

        if(Application.isPlaying)
        {
            Vector3 directionToPlayer = (playerAimPoint.position - transform.position).normalized;
            Debug.DrawRay(transform.position, directionToPlayer * DetectionRadius, Color.green);
        }
    }
}
