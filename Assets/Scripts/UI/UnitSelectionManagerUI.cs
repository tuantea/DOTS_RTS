using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour {



    [SerializeField] private RectTransform selectionAreaRectTransform;


    private void Start() {
        UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;


        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void Update() {
        if (selectionAreaRectTransform.gameObject.activeSelf) {
            UpdateVisual();
        }
    }

    private void UnitSelectionManager_OnSelectionAreaStart(object sender, System.EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(true);

        UpdateVisual();
    }

    private void UnitSelectionManager_OnSelectionAreaEnd(object sender, System.EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateVisual() {
        Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y);
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height);
    }

}