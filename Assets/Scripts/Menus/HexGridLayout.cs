using UnityEngine;
using UnityEngine.UI;

public class HexGridLayout : MonoBehaviour, ILayoutGroup
{
	public RectTransform rect;
	public Vector2 cellSize = new Vector2(100, 100);
	public Vector2 spacing = new Vector2(10, 10);
	public Vector2 padding = Vector2.zero;

	void Start()
	{
		LayoutChildren();
	}

	void OnEnable() {
		MarkDirty();
	}

	void OnTransformChildrenChanged() {
		MarkDirty();
	}

	void OnRectTransformDimensionsChange() {
		MarkDirty();
	}

	public void SetLayoutVertical() {
		MarkDirty();
	}

	public void SetLayoutHorizontal() {
		MarkDirty();
	}

	void MarkDirty() {
		if (!isActiveAndEnabled) return;
		if (rect == null) rect = GetComponent<RectTransform>();
		LayoutRebuilder.MarkLayoutForRebuild(rect);
	}

	void LayoutChildren() {
		RectTransform parent = GetComponent<RectTransform>();
		int childCount = parent.childCount;

		float totalWidth = parent.rect.width + spacing.x+padding.x;
		int columns = (int)Mathf.Floor(totalWidth/(cellSize.x + spacing.x + padding.x));

		if(columns == 0 ) columns = 1;

		for (int i = 0; i < childCount; i++) {
			RectTransform child = rect.GetChild(i) as RectTransform;
			if (child == null || !child.gameObject.activeSelf) continue;

			int row = i / columns;
			int col = i % columns;

			// Top-left anchored grid
			child.anchorMin = new Vector2(0, 1);
			child.anchorMax = new Vector2(0, 1);
			child.pivot = new Vector2(0, 1);

			Vector2 pos = new Vector2(
				cellSize.x / 2 + padding.x + col * (cellSize.x + spacing.x),
				- cellSize.y / 2 - (padding.y + row * (cellSize.y + spacing.y))
			);

			if(col%2 == 0)
				pos.y += cellSize.y / 2 + padding.y/2;

			child.anchoredPosition = pos;
			child.sizeDelta = cellSize;
		}
	}
}