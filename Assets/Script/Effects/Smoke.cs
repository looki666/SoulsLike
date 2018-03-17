using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Smoke : MonoBehaviour {

    private LineRenderer line;
    private Material lineMaterial;

    [SerializeField]
    private int numberOfPoints = 10;
    [SerializeField]
    [ReadOnly]
    private int currentNumberOfPoints = 2;

    private float lineSegment;

    private Vector3[] positions;
    private Vector3[] directions;

    public float updateSpeed  = 0.25f;
    public float riseSpeed = 0.25f;
    public float spread = 0.2f;

    private Vector3 tempVec;

    private float timeSinceUpdate;
    private bool allPointsAdded;

    // Use this for initialization
    void Start () {
        allPointsAdded = false;
        timeSinceUpdate = 0f;
        line = GetComponent<LineRenderer>();
        lineMaterial = line.material;

        lineSegment = 1.0f / numberOfPoints;

        positions = new Vector3[numberOfPoints];
        directions = new Vector3[numberOfPoints];

        line.positionCount = currentNumberOfPoints;

        for (int i = 0; i < currentNumberOfPoints; i++)
        {
            tempVec = GetSmokeVec();
            directions[i] = tempVec;
            positions[i] = transform.position;
            line.SetPosition(i, positions[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {
        timeSinceUpdate += Time.deltaTime; // Update time.

        // If it's time to update the line...
        if (timeSinceUpdate > updateSpeed)
        {
            timeSinceUpdate -= updateSpeed;

            // Add points until the target number is reached.
            if (!allPointsAdded)
            {
                currentNumberOfPoints++;
                line.positionCount = currentNumberOfPoints;
                tempVec = GetSmokeVec();
                directions[0] = tempVec;
                positions[0] = transform.position;
                line.SetPosition(0, positions[0]);
            }

            if (!allPointsAdded && (currentNumberOfPoints == numberOfPoints))
            {
                allPointsAdded = true;
            }

            // Make each point in the line take the position and direction of the one before it (effectively removing the last point from the line and adding a new one at transform position).
            for (int i = currentNumberOfPoints - 1; i > 0; i--)
            {
                tempVec = positions[i - 1];
                positions[i] = tempVec;
                tempVec = directions[i - 1];
                directions[i] = tempVec;
            }
            tempVec = GetSmokeVec();
            directions[0] = tempVec; // Remember and give 0th point a direction for when it gets pulled up the chain in the next line update.
        }

        // Update the line...
        for (int i = 1; i < currentNumberOfPoints; i++)
        {
            positions[i] += directions[i] * Time.deltaTime;

            line.SetPosition(i, positions[i]);
        }
        positions[0] = transform.position; // 0th point is a special case, always follows the transform directly.
        line.SetPosition(0, transform.position);

        // If we're at the maximum number of points, tweak the offset so that the last line segment is "invisible" (i.e. off the top of the texture) when it disappears.
        // Makes the change less jarring and ensures the texture doesn't jump.
        if (allPointsAdded)
        {
            lineMaterial.mainTextureOffset = new Vector2(lineMaterial.mainTextureOffset.x, lineMaterial.mainTextureOffset.y + lineSegment * (timeSinceUpdate / updateSpeed));
        }
    }

    // Give a random upwards vector.
    private Vector3 GetSmokeVec(){
        float x = Random.Range(-1.0f, 1.0f);
        float y = Random.Range(0.0f, 1.0f);
        float z = Random.Range(-1.0f, 1.0f);

        Vector3 smokeVec = new Vector3(x, y, z);
		smokeVec.Normalize();
		smokeVec *= spread;
		smokeVec.y += riseSpeed;
		return smokeVec;
    }


}