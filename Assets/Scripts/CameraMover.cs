using UnityEngine;

public class CameraMover :  MonoBehaviour
{
	public Camera orthoCam;
	public enum CameraType {STATIC, FLOAT}
	public CameraType HorizontalType = CameraType.STATIC;
	public CameraType VerticalType = CameraType.STATIC;

	public Vector3 TargetLocation;
	public Vector3 StartLocation;
	float ScreenWidth;
	float ScreenHeight;

	public bool IsTargeting = false;
	public float LerpAmount;
	public float Speed = 1f;

	public float LeftOfLevel;
	public float RightOfLevel;
	public float TopOfLevel;
	public float BottomOfLevel;

	float LevelHeight {get{return TopOfLevel - BottomOfLevel;}}
	float LevelWidth {get{return RightOfLevel - LeftOfLevel;}}

	void RecalculateCameraType()
	{
		HorizontalType = (RightOfLevel - LeftOfLevel > ScreenWidth) ? 
			CameraType.FLOAT :
			CameraType.STATIC;

		VerticalType = (TopOfLevel - BottomOfLevel > ScreenHeight) ? 
			CameraType.FLOAT :
			CameraType.STATIC;

		if(HorizontalType == CameraType.STATIC && VerticalType == CameraType.STATIC)
		{
			RescaleCamera();
			transform.position = new Vector3(
				RightOfLevel / 2 + LeftOfLevel / 2,
				TopOfLevel / 2 + BottomOfLevel / 2,
				transform.position.z);
		}
		else
			SetTarget(
				Map.CoordToWorldPoint(
					GameManager.master.Player.Position));
	}

	void RescaleCamera()
	{
		orthoCam.orthographicSize = 
			Mathf.Max(
				RightOfLevel-LeftOfLevel-2,
				TopOfLevel-BottomOfLevel-2
			)/2-0.5f;
	}

	public void SetLevelRect(float _left, float _top, float _right, float _bottom)
	{
		LeftOfLevel = _left-1;
		TopOfLevel = _top + 1;
		RightOfLevel = _right+1;
		BottomOfLevel = _bottom-1;

		orthoCam.orthographicSize = 5;
		ScreenHeight = 2 * orthoCam.orthographicSize;
		ScreenWidth = orthoCam.aspect * ScreenHeight;
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
				LerpAmount*(2-LerpAmount));
		}
	}

	public void SetTarget(Vector2 _targetLocation)
	{
		if(HorizontalType == CameraType.STATIC &&
			VerticalType == CameraType.STATIC) return;

		float xTarget, yTarget;
		if(HorizontalType == CameraType.STATIC)
		{
			xTarget = RightOfLevel / 2 + LeftOfLevel / 2;
		}
		else
		{
			xTarget = LeftOfLevel + ScreenWidth/2 +
				(LevelWidth - ScreenWidth) * (_targetLocation.x - LeftOfLevel) / LevelWidth;
		}

		if(VerticalType == CameraType.STATIC)
		{
			yTarget = TopOfLevel / 2 + BottomOfLevel / 2;
		}
		else
		{
			yTarget = BottomOfLevel + ScreenHeight/2 +
				(LevelHeight - ScreenHeight) * (_targetLocation.y - BottomOfLevel) / LevelHeight;
		}

		StartLocation = transform.position;
		TargetLocation = new Vector3(
			xTarget,
			yTarget,
			transform.position.z);

		IsTargeting = true;
		LerpAmount = 0f;
	}
}