using System.Collections;
using UnityEngine;
using Pathfinding; // we can access pathfinder classes and engine

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]
public class EnemyAI : MonoBehaviour
{
    // what to chase
    public Transform target;

    // how many times each seond we will update our path
    public float updateRate = 2f;

    // caching
    private Seeker seeker;
    private Rigidbody2D rb;

    // store a calculated path
    public Path path;

    // The AI's speed per second not by frame
    public float speed = 300f;
    // controls how force is applied to ridgebody - controls behavior
    public ForceMode2D forceMode;

    [HideInInspector]
    // wont show up in the inspector but is a public variable
    public bool pathIsEnded = false;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3f;

    // the waypoint we are currently moving towards
    private int currentWayPoint = 0;

    private bool searchingForPlayer = false;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        // Start a new path to the target position and return the result to the on path complete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());

    }

    IEnumerator SearchForPlayer()
    {
        GameObject searchResult = GameObject.FindGameObjectWithTag("Player");
        if (searchResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            searchingForPlayer = false;
            target = searchResult.transform;
            StartCoroutine(UpdatePath());
            yield return false;
        }

    }

     IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }

        // Start a new path to the target position and return the result to the on path complete method
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);

        StartCoroutine(UpdatePath());

    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a path. Did it have an error? " + p.error);

        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        if (path == null)
        {
            return;
        }

        if (currentWayPoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
            {
                return;
            }
            Debug.Log("End of path reached.");
            pathIsEnded = true;
            return;
        }

        pathIsEnded = false;

        // direction to the next waypoint
        Vector3 direction = (path.vectorPath[currentWayPoint] - transform.position).normalized;
        direction *= speed * Time.fixedDeltaTime;

        // move the AI 
        rb.AddForce(direction, forceMode);

        float distance = Vector3.Distance(transform.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
        {
            currentWayPoint++;
            return;
        }
    }
}
