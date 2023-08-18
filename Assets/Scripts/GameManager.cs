using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Colors {
    public List<Color> colors = new List<Color>();
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] CameraController cameraController;

    [Header("Projectile")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float initialVelocity = 35;

    [Header("misc")]
    [SerializeField] GameObject destroyerCube;

    [Header("Colors for shapes")]
    [SerializeField] Colors shapeColorsList;
    [SerializeField] TextAsset shapeColorsJson;

    [Header("Colors for ball")]
    [SerializeField] Colors ballColorsList;
    [SerializeField] TextAsset ballColorsJson;

    public const string BallProjectileName = "BallP";

    private void Awake() {
        instance = this;
    }

    private void Start() {
        LoadShapeColorsFromJson();
        LoadBallColorsFromJson();
        SpawnShape();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShootProjectile();
        }
    }

    [ContextMenu("PrintShapeColorsListJson")]
    public void PrintShapeColorsListJson() {
        string json = JsonUtility.ToJson(shapeColorsList);
        Debug.Log(json);
    }

    public void LoadShapeColorsFromJson() {
        shapeColorsList = JsonUtility.FromJson<Colors>(shapeColorsJson.text);
    }

    [ContextMenu("PrintBallColorsListJson")]
    public void PrintBallColorsListJson() {
        string json = JsonUtility.ToJson(ballColorsList);
        Debug.Log(json);
    }

    public void LoadBallColorsFromJson() {
        ballColorsList = JsonUtility.FromJson<Colors>(ballColorsJson.text);
    }

    public void ShootProjectile() {
        ///TBD - use object pooling for balls
        if (cameraController.AllowShootingProjectile == false) {
            return;
        }
        if (projectilePrefab == null) {
            return;
        }
        GameObject insP = Instantiate(projectilePrefab, cameraController.CamTransform.transform.position, Quaternion.identity);
        insP.transform.name = GameManager.BallProjectileName;

        Color ballColor = Color.red;
        if (ballColorsList != null) {
            if (ballColorsList.colors != null) {
                if (ballColorsList.colors.Count > 0) {
                    ballColor = ballColorsList.colors[Random.Range(0, ballColorsList.colors.Count)];
                }
            }
        }

        MeshRenderer mr = insP.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.color = ballColor;
        mr.material = mat;

        insP.GetComponent<Rigidbody>().velocity = initialVelocity * cameraController.CamTransform.transform.forward;
        insP.GetComponent<DelayDestroy>().Init(6f);
    }

    public void SpawnShape() {
        ///TBD - use object pooling for shapes;
        Vector3 pos = new Vector3(Random.Range(-10f, 10f), Random.Range(3, 9f), 5f);
        Color shapeColor = Color.yellow;
        if (shapeColorsList != null) {
            if (shapeColorsList.colors != null) {
                if (shapeColorsList.colors.Count > 0) {
                    shapeColor = shapeColorsList.colors[Random.Range(0, shapeColorsList.colors.Count)];
                }
            }
        }
        GameObject newShape = null;
        int rand = Random.Range(0, 4);
        switch (rand) {
            case 0:
                newShape = CreateDestroyableCircle(pos, shapeColor);
                break;

            case 1:
                newShape = CreateDestroyableRectangle(pos, shapeColor);
                break;

            case 2:
                newShape = CreateDestroyableSquare(pos, shapeColor);
                break;

            case 3:
                newShape = CreateDestroyablePyramid(pos, shapeColor);
                break;
        }

        if (newShape != null) {
            cameraController.RotateToLookAt(newShape);
            newShape.AddComponent<BobbingAnim>().Init();
        }
    }

    public GameObject CreateDestroyableCircle(Vector3 pos,Color c) {
        GameObject x = ShapeCreator.GetCircle(c,1.25f);
        GameObject parent = Instantiate(destroyerCube);
        parent.GetComponent<MeshRenderer>().material = x.GetComponent<MeshRenderer>().material;
        parent.transform.position = pos;
        x.transform.SetParent(parent.transform);
        x.transform.localPosition = Vector3.zero;
        return parent;
    }

    public GameObject CreateDestroyableRectangle(Vector3 pos, Color c) {
        GameObject x = ShapeCreator.GetRectangle(c);
        GameObject parent = Instantiate(destroyerCube);
        Vector3 scale = parent.transform.localScale;
        scale.x = 4f;
        scale.y = 2f;
        parent.transform.localScale = scale;
        parent.GetComponent<MeshRenderer>().material = x.GetComponent<MeshRenderer>().material;
        parent.transform.position = pos;
        x.transform.SetParent(parent.transform);
        x.transform.localPosition = Vector3.zero;
        return parent;
    }

    public GameObject CreateDestroyableSquare(Vector3 pos, Color c) {
        GameObject x = ShapeCreator.GetSquare(c);
        GameObject parent = Instantiate(destroyerCube);
        Vector3 scale = parent.transform.localScale;
        //scale.x = 4f;
        //scale.y = 2f;
        parent.transform.localScale = scale;
        parent.GetComponent<MeshRenderer>().material = x.GetComponent<MeshRenderer>().material;
        parent.transform.position = pos;
        x.transform.SetParent(parent.transform);
        x.transform.localPosition = Vector3.zero;
        return parent;
    }

    public GameObject CreateDestroyablePyramid(Vector3 pos, Color c) {
        GameObject x = ShapeCreator.GetPyramid(c,2f);
        GameObject parent = Instantiate(destroyerCube);
        Vector3 scale = parent.transform.localScale;
        scale.x = 0.9f;
        scale.y = 0.9f;
        parent.transform.localScale = scale;
        parent.GetComponent<MeshRenderer>().material = x.GetComponent<MeshRenderer>().material;
        parent.transform.position = pos;
        x.transform.SetParent(parent.transform);
        x.transform.localPosition = Vector3.down * 0.4f;
        return parent;
    }
}
