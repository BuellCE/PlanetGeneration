using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gravity : MonoBehaviour {

    public float forceAngleSpeed;
    public float forceMultiplier = 1f;

    private Rigidbody rb;
    private List<WorldGeneratorWithChunks> worlds;
    private WorldGeneratorWithChunks currentWorld;

    [System.NonSerialized]
    public bool isTouchingGround;

    [System.NonSerialized]
    public bool isInGravitationalPull;

    void Start() {
        rb = GetComponent<Rigidbody>();
        worlds = new List<WorldGeneratorWithChunks>();
        FindWorlds();
    }

    void FindWorlds() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Planet")){
            worlds.Add(go.GetComponent<WorldGeneratorWithChunks>());
        }
    }

    void FixedUpdate() {
        UpdateCurrentWorld();
        if (currentWorld != null) {
            ApplyGravity();
            if (forceAngleSpeed > 0f) {
                ForceAngle(forceAngleSpeed);
            }
        }
    }

    private void UpdateCurrentWorld() {
        WorldGeneratorWithChunks closest = GetClosestWorld();
        currentWorld = closest;
    }

    private void ApplyGravity() {
        Vector3 direction = currentWorld.transform.position - this.transform.position;

        float distance = direction.magnitude - currentWorld.radius;
        float strength = currentWorld.radius - distance;

        if (strength >= 0) {
            strength = (strength * currentWorld.radius) / 175f;
            rb.AddForce(direction.normalized * strength * Time.fixedDeltaTime);
        }
    }

    public Quaternion GetAngleOnPlanet() {
        WorldGeneratorWithChunks closest = GetClosestWorld();
        if (closest == null) {
            return Quaternion.identity;
        }
        Vector3 surfaceNormal = this.transform.position - closest.transform.position;
        if (closest != null) {
            float strength = closest.radius * 2;
            float magnitude = surfaceNormal.magnitude;
            if (magnitude < strength) {
                return Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            }
        }
        return Quaternion.identity;
    }

    private void ForceAngle(float forceAngleSpeed) {
        WorldGeneratorWithChunks closest = GetClosestWorld();
        if (closest != null) {
            Vector3 surfaceNormal = this.transform.position - closest.transform.position;
            float strength = closest.radius * 2;
            float magnitude = surfaceNormal.magnitude;
            if (magnitude < strength) {
                if (isTouchingGround) {

                    transform.rotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

                } else {
                    var targetRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, forceAngleSpeed * strength * Time.fixedDeltaTime);
                }
            }
        }
    }

    private WorldGeneratorWithChunks GetClosestWorld() {
        WorldGeneratorWithChunks closest = null;
        float distance = Mathf.Infinity;
        foreach (WorldGeneratorWithChunks world in worlds) {
            if (world == null || world.transform == null) {
                continue;
            }

            float maxDis = world.radius * 2;

            Vector3 direction = world.transform.position - this.transform.position;
            float distanceFrom = direction.magnitude;
            if (distanceFrom < distance && distanceFrom < maxDis) {
                distance = distanceFrom;
                closest = world;
            }
        }
        return closest;
    }

    public Vector3 GetPlanetNormal() {
        if (currentWorld != null) {
            return currentWorld.transform.position - this.transform.position;
        } else {
            return Vector3.zero;
        }
    }

    public bool IsNearPlanet() {
        if (currentWorld != null) {
            return true;
        } else {
            return false;
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.other.CompareTag("Planet")) {
            isTouchingGround = true;
        }
    }

    public void OnCollisionExit(Collision collision) {
        if (collision.other.CompareTag("Planet")) {
            isTouchingGround = false;
        }
    }

}
