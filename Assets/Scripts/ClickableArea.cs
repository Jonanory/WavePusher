using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableArea: MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {

	const float TIME_BETWEEN_DRAG_MOVEMENT = 0.3f;

	public bool IsDragging = false;
	public float CountdownUntilDragMovement = 0f;
	Vector2 MousePosition;

	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("pionter begn");
		CountdownUntilDragMovement = 0;
		IsDragging = true;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		CountdownUntilDragMovement = 0;
		IsDragging = false;
	}

	void FixedUpdate()
	{
		if(!IsDragging) return;
		CountdownUntilDragMovement -= Time.fixedDeltaTime;
		if(CountdownUntilDragMovement > 0)
		{
			return;
		}
		CountdownUntilDragMovement = TIME_BETWEEN_DRAG_MOVEMENT;
		MapDirection? direction = Map.Vector2ToDirection(MousePosition - Map.CoordToWorldPoint(GameManager.master.Player.Position));
		if(direction != null)
		{
			GameManager.master.Player.TryMove(direction.Value);
		}
	}

	public void OnDrag (PointerEventData eventData)
	{
		MousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		IsDragging = true;
		MousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
		Vector2Int clickedCoord = Map.WorldPointToCoord(MousePosition);
		MapDirection? direction = Map.CoordsAreAdjacent(GameManager.master.Player.Position, clickedCoord);
		if(direction != null)
		{
			GameManager.master.Player.TryMove(direction.Value);
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		IsDragging = false;
	}
}