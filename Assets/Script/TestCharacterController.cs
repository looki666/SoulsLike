using System;
using UnityEngine;
 
[RequireComponent(typeof(SphereCollider))]
public class TestCharacterController : MonoBehaviour
{
    public float mouseSensitivity = 10;
    public float gravity = 9f;
    public float speed = 1f;
    public bool moveEvenIfFailedCollision;

    Vector3 velocity;
    int maxCollisionAttemps = 50;

    float surfaceOffset = .0001f;
    float backstepOffset = .001f;
    float minVelocityBreak = .001f;

    float radius { get { return sphereCollider.radius; } }

    SphereCollider sphereCollider;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.enabled = false;
    }

    void FixedUpdate()
    {
        Rotate();
        Move();
    }

    void Rotate()
    {
        transform.Rotate(0, Input.GetAxisRaw("Mouse X") * mouseSensitivity, 0);
    }

    void Move()
    {
        Vector3 movement = GetMoveDirection() * speed;
        movement += ((Vector3.down * gravity) * Time.deltaTime);
        velocity += movement;

        Debug.Log(velocity);
        velocity = MoveCollision(velocity * Time.deltaTime);

        velocity /= Time.deltaTime;
    }

    Vector3 MoveCollision(Vector3 targetVelocity)
    {
        Vector3 originalVelocity = targetVelocity;
        Vector3 origin = transform.position;

        int attempts = 0;
        for (attempts = 0; attempts < maxCollisionAttemps; attempts++)
        {
            Vector3 prevOrigin = origin;
            Vector3 hitNormal = Vector3.zero;
            bool hasHit = false;

            float castDistance = targetVelocity.magnitude + backstepOffset;
            Vector3 castDirection = targetVelocity.normalized;
            Vector3 castStartBackOffset = origin - (castDirection * backstepOffset);

            RaycastHit hitInfo;
            if (Physics.SphereCast(castStartBackOffset, radius, castDirection, out hitInfo, castDistance))
            {
                origin = CastCenterOnCollision(castStartBackOffset, castDirection, hitInfo.distance);
                origin += (hitInfo.normal * surfaceOffset);

                hitNormal = hitInfo.normal;
                hasHit = true;
            }
            else
            {
                origin += targetVelocity;
            }

            if (hasHit)
            {
                //This might not be a accurate conversion, but is fine for testing.
                //There might be an issue with this since when I move almost parallel against a wall, I seem to slow down a lot as if getting stuck.
                float remainingDistance = Mathf.Max(0f, targetVelocity.magnitude - Vector3.Distance(prevOrigin, origin));
                Vector3 remainingVelocity = targetVelocity.normalized * remainingDistance;
                targetVelocity = Vector3.ProjectOnPlane(remainingVelocity, hitNormal);

                if (targetVelocity.magnitude <= minVelocityBreak) break;

            }
            else
            {
                break;
            }
        }

        bool failedCollision = attempts >= maxCollisionAttemps;
        if (failedCollision) Debug.LogWarning("Failed collision handling");

        if (!moveEvenIfFailedCollision && failedCollision)
        {
            Debug.LogWarning("Aborting movement");
            return Vector3.zero;
        }
        else
        {
            transform.position = origin;
            return targetVelocity.normalized * MagnitudeInDirection(targetVelocity, originalVelocity);
        }
    }

    Vector3 CastCenterOnCollision(Vector3 origin, Vector3 directionCast, float hitInfoDistance)
    {
        return origin + (directionCast.normalized * hitInfoDistance);
    }

    float MagnitudeInDirection(Vector3 vector, Vector3 direction)
    {
        return Vector3.Dot(vector, direction.normalized);
    }

    Vector3 GetMoveDirection()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        return transform.TransformDirection(inputDirection);
    }
}

/*
 * - For corners, I made an algorithm that detects if we are casting against a "blocking corner" based on all the previous iterations made so far, and it stops the iterations accordingly.
- For floating point errors, I always keep my collider 0.0001 units away from the hit surfaces, along the normals. That way, subsequent casts will never fail.
- Additionally, all of my casts have a 0.001 "backstep distance", which means they start some tiny distance backwards from their intended startingpoint/direction
- A small "MinimalVelocityClamp" value insures that there will be no jittering in certain corners
 * 
 * 
 * 
 Still on the topic of handling slightly parallel corridors with CCD, I think I have found a failure-proof solution.

In short, at each step of the sweep test iterations (the for loop at line 58 of your example above), whenever you decide to move the character position (or "origin", in your example), you must first make an overlap test at that position. If there is no overlap, just keep moving the position as you would normally. However, if there ARE overlaps:
project the movement against all overlap normals (which we find with ComputePenetration)
do not move the position. just stay where you were at the start of the iteration
proceed to the next step of the sweep iterations, with the new projected movement direction
This method guarantees that you can never move the character in a position where it's overlapping with something. I'll need more testing, but it seems to work like a charm so far.
     
     */
