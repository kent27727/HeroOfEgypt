using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float sens;
	private Transform parent;

	private void Start()
	{
		parent = transform.parent;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		Rotate();
	}

	private void Rotate()
	{
		float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
		parent.Rotate(Vector3.up, mouseX);
	}
}
