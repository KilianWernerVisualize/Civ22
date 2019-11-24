using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PathNode
{
    int x();
    int y();
    float moveCost(Unit walker, int destinationNeighbor);
    PathNode neighborp(int i);
}

public class PathFinder : MonoBehaviour
{

    public static float manhattanDistance(PathNode start, PathNode finish)
    {
        // Read x and y values from start and finish each
        int xStart = start.x();
        int yStart = start.y();
        int xFinish = finish.x();
        int yFinish = finish.y();

        // Transform x coordinate to staggered grid
        xStart = 2 * xStart + yStart % 2;
        xFinish = 2 * xFinish + yFinish % 2;

        // Determine minimal and maximal difference
        int m = Mathf.Min(Mathf.Abs(xStart - xFinish), Mathf.Abs(yStart - yFinish));
        int M = Mathf.Max(Mathf.Abs(xStart - xFinish), Mathf.Abs(yStart - yFinish));

        // Compute and return result
        int result = m + ((M - m) / 2);
        return 1.0f*result;
    }

    public static float makePath(Unit walker, PathNode start, PathNode finish, out Vector2 next)
    {
        start.moveCost(walker, 1);
        next = new Vector2(0, 0);
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
