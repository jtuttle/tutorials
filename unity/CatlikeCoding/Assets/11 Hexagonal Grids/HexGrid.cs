using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {
    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;
    public Text cellLabelPrefab;

    private Canvas _gridCanvas;
    private HexMesh _hexMesh;
    private HexCell[] _cells;

    private void Awake() {
        _gridCanvas = GetComponentInChildren<Canvas>();
        _hexMesh = GetComponentInChildren<HexMesh>();

        _cells = new HexCell[width * height];

        for(int z = 0, i = 0; z < height; z++) {
            for(int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    private void Start() {
        _hexMesh.TriangulateCells(_cells);
    }

    private void Update() {
        if(Input.GetMouseButton(0)) {
            HandleInput();
        }
    }

    private void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(inputRay, out hit) ) {
            TouchCell(hit.point);
        }
    }

    private void TouchCell(Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        Debug.Log("touched at " + coordinates.ToString());
    }

    private void CreateCell(int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = _cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(_gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }
}
