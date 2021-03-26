using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {

	public float sensitivityX = 13F;
	public float sensitivityY = 13F;

	public float minX = -360F;
	public float maxX = 360F;

	public Gravity gravity;

	private float minY = -85F;
	private float maxY = 85F;
	private float rotationX = 0F;
	private float rotationY = 0F;

	Quaternion originalRotation;

	void Start() {
		originalRotation = transform.localRotation;
	}

	void Update() {
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;

        rotationY = ClampAngle(rotationY, minY, maxY);
        rotationX = ClampAngle(rotationX, minX, maxX);

        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

        transform.localRotation = originalRotation * xQuaternion * yQuaternion;
    }

	public static float ClampAngle(float angle, float min, float max) {
		angle = angle % 360;
		if (angle >= -360F && angle <= 360F) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}
}