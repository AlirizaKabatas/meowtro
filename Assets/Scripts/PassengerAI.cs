using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class PassengerAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Queue<Transform> waypoints;
    private Queue<bool> isOptionalQueue;
    private Transform currentWaypoint;

    private Renderer _meshRenderer;
    private Color _originalColor;
    private float _colorTimer = 0f;

    [Header("Queue Settings")]
    [Tooltip("Ne kadar uzağa bakıp sıradaki kişiyi algılayacak?")]
    public float checkDistance = 1.0f;
    
    [Tooltip("Önündeki kişiyi algılamak için atılacak SphereCast'in yarıçapı. (Takılmaları önlemek için küçük tutun)")]
    public float passengerRadius = 0.15f;
    
    [Tooltip("Sadece Passenger layer'ına sahip objeleri algılasın.")]
    public LayerMask passengerLayer;

    [Header("Path Settings")]
    [Tooltip("Hedefe bu kadar yaklaştığında (kalabalıktan tam üstüne basamasa bile) ulaştı sayılır ve sıradaki hedefe geçer.")]
    public float waypointTolerance = 1.5f;

    // Sıkışma (Deadlock) çözücü değişkenler
    private float stuckTimer = 0f;
    private float ignoreQueueTimer = 0f;
    private Vector3 lastPos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPos = transform.position;
        _meshRenderer = GetComponentInChildren<Renderer>();
        if (_meshRenderer != null)
        {
            _originalColor = _meshRenderer.material.color;
        }
    }

    public void InitializeRoute(List<Transform> route, List<bool> optionalList = null)
    {
        waypoints = new Queue<Transform>(route);
        if (optionalList != null) isOptionalQueue = new Queue<bool>(optionalList);
        SetNextWaypoint();
    }

    private void SetNextWaypoint()
    {
        if (waypoints != null && waypoints.Count > 0)
        {
            currentWaypoint = waypoints.Dequeue();
            
            bool isOptional = false;
            if (isOptionalQueue != null && isOptionalQueue.Count > 0)
            {
                isOptional = isOptionalQueue.Dequeue();
            }

            if (isOptional && _meshRenderer != null)
            {
                _meshRenderer.material.color = Color.green;
                _colorTimer = 1.5f;
            }

            agent.SetDestination(currentWaypoint.position);
            agent.isStopped = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_colorTimer > 0f)
        {
            _colorTimer -= Time.deltaTime;
            if (_colorTimer <= 0f && _meshRenderer != null)
            {
                _meshRenderer.material.color = _originalColor;
            }
        }

        if (currentWaypoint == null) return;

        HandleQueueing();
        CheckIfStuck();

        // Ulaşılamaz (NavMesh'in koptuğu veya havada kalan) bir hedefe gitmeye çalışıyorsa direkt atla
        if (!agent.pathPending && (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial))
        {
            SetNextWaypoint();
            return;
        }

        // Hedefe ulaştık mı kontrolü (NavMesh veya Direkt Mesafe)
        bool reachedByNavMesh = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f && (!agent.hasPath || agent.velocity.sqrMagnitude < 0.1f);
        
        // Mesafe olarak yeterince yaklaştıysak (teğet geçtiysek vs.) kalabalığı delmemesi için direkt ulaştı kabul et
        bool reachedByDistance = Vector3.Distance(transform.position, currentWaypoint.position) <= waypointTolerance;

        if (reachedByNavMesh || reachedByDistance)
        {
            SetNextWaypoint();
        }
    }

    private void CheckIfStuck()
    {
        // Eğer ajan çok yavaş hareket ediyorsa (veya duruyorsa)
        if (agent.velocity.magnitude < 0.1f)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        // 4 saniye boyunca hiçbir yere kıpırdayamadıysa, sıkışıklığı açmak için direkt bir sonraki hedefe geç!
        // Ve 1.5 saniye boyunca önündekine takılma (içinden geçerek düğümü çöz)
        if (stuckTimer > 4f)
        {
            ignoreQueueTimer = 1.5f;
            SetNextWaypoint();
            stuckTimer = 0f;
        }
    }

    private void HandleQueueing()
    {
        if (ignoreQueueTimer > 0f)
        {
            ignoreQueueTimer -= Time.deltaTime;
            agent.isStopped = false;
            return;
        }

        Vector3 rayOrigin = transform.position + Vector3.up * 1f; 
        
        bool isSomeoneInFront = Physics.SphereCast(rayOrigin, passengerRadius, transform.forward, out RaycastHit hit, checkDistance, passengerLayer);

        if (isSomeoneInFront)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
        }
        else
        {
            agent.isStopped = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        Gizmos.DrawWireSphere(rayOrigin + transform.forward * checkDistance, passengerRadius);
    }
}
