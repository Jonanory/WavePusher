using UnityEngine;

public class CameraMover :  MonoBehaviour
{
	public Camera orthoCam;
	public enum CameraType {STATIC, FLOAT}
	public CameraType Type = CameraType.STATIC;

	public Vector3 TargetLocation;
	public Vector3 StartLocation;

	public bool IsTargeting = false;
	public float LerpAmount;
	public float Speed = 1f;

	public float LeftOfLevel;
	public float RightOfLevel;
	public float TopOfLevel;
	public float BottomOfLevel;

	void RecalculateCameraType()
	{
		float screenAspect = (float) Screen.width / (float) Screen.height;
		float camHalfHeight = orthoCam.orthographicSize;
		float camHalfWidth = orthoCam.aspect * camHalfHeight;

		if(RightOfLevel - LeftOfLevel > 2.0f * camHalfWidth || TopOfLevel - BottomOfLevel > 2.0f * camHalfHeight)
		{
			Type = CameraType.FLOAT;
		}
		else
		{
			Type = CameraType.STATIC;
			transform.position = new Vector3(
				RightOfLevel / 2 +   LeftOfLevel / 2,
				  TopOfLevel / 2 + BottomOfLevel / 2,
				transform.position.z);
		}
	}

	public void SetLevelRect(float _left, float _top, float _right, float _bottom)
	{
		LeftOfLevel = _left;
		TopOfLevel = _top;
		RightOfLevel = _right;
		BottomOfLevel = _bottom;
		RecalculateCameraType();
	}

	void Awake()
	{
		orthoCam = GetComponent<Camera>();
	}

	void Update()
	{
		if(IsTargeting)
		{
			LerpAmount += Time.deltaTime * Speed;
			if(LerpAmount > 1)
			{
				LerpAmount = 1f;
				IsTargeting = false;
			}
			transform.position = Vector3.Lerp(
				StartLocation,
				TargetLocation,
				1-(1-LerpAmount)*(1-LerpAmount));
		}
	}

	public void SetTarget(Vector2 _targetLocation)
	{
		if(Type != CameraType.FLOAT) return;
		StartLocation = transform.position;
		TargetLocation = new Vector3(
			_targetLocation.x,
			_targetLocation.y,
			transform.position.z);
		IsTargeting = true;
		LerpAmount = 0f;
	}
}