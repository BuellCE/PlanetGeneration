using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;
    public float sprintMultiplier = 1.5f;
    public float jetPackForce;

    public Rigidbody rb;
    public Camera camera;
    public Gravity gravity;

    public float raycastDistance = 1f;

    public float height = 0.8f;

    void FixedUpdate() {

        Vector3 toMove = Vector3.zero;
        float moveSpeed = speed;
        float jpforce = jetPackForce;

        if (gravity.IsNearPlanet()) {

            Vector3 planetNormal = gravity.GetPlanetNormal().normalized;

            if (Input.GetKey(KeyCode.W)) {
                Vector3 cross = Vector3.Cross(planetNormal, camera.transform.right.normalized);
                toMove += cross.normalized;
            }
            if (Input.GetKey(KeyCode.S)) {
                Vector3 cross = Vector3.Cross(planetNormal, -camera.transform.right.normalized);
                toMove += cross.normalized;
            }
            if (Input.GetKey(KeyCode.A)) {
                Vector3 cross = Vector3.Cross(planetNormal, camera.transform.forward.normalized);
                toMove += cross.normalized;
            }
            if (Input.GetKey(KeyCode.D)) {
                Vector3 cross = Vector3.Cross(planetNormal, -camera.transform.forward.normalized);
                toMove += cross.normalized;
            }

            if (Input.GetKey(KeyCode.LeftControl)) {
                jpforce = jpforce * 5f;
                moveSpeed = moveSpeed * 5f;
            }

            if (toMove.sqrMagnitude > 0.1f) {

                if (Input.GetKey(KeyCode.LeftShift)) {
                    moveSpeed = moveSpeed * sprintMultiplier;
                }

                toMove = rb.position + toMove.normalized * moveSpeed * Time.fixedDeltaTime;

                if (!Input.GetKey(KeyCode.Space)) {
                    RaycastHit hit;
                    if (Physics.Raycast(toMove.normalized, planetNormal, out hit, raycastDistance)) {
                        toMove = hit.point - (planetNormal * height);
                    }
                }
                rb.MovePosition(toMove);
            }

            if (Input.GetKey(KeyCode.Space)) {
                rb.AddForce(-planetNormal * jpforce * Time.fixedDeltaTime);
            }
        } else {
            Vector3 moveTo = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) 
                moveTo += camera.transform.forward.normalized;
            if (Input.GetKey(KeyCode.S)) 
                moveTo -= camera.transform.forward.normalized;
            if (Input.GetKey(KeyCode.A)) 
                moveTo -= camera.transform.right.normalized;
            if (Input.GetKey(KeyCode.D)) 
                moveTo += camera.transform.right.normalized;

            if (Input.GetKey(KeyCode.LeftControl)) {
                jpforce = jpforce * 10f;
            }
            rb.AddForce(moveTo * jpforce * Time.fixedDeltaTime);

        }

    }
}
