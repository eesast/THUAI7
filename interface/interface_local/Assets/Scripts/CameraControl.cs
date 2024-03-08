using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector2 mousePos;
    public AnimationCurve cameraScaleCurve;
    public float currentScaleTime = 0.5f, basicCameraScale, currentScale;
    void Start()
    {
        currentScale = cameraScaleCurve.Evaluate(currentScaleTime) * basicCameraScale;
        Camera.main.orthographicSize = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * Time.deltaTime * currentScale * 1.5f);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * Time.deltaTime * currentScale * 1.5f);
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * Time.deltaTime * currentScale * 1.5f);
        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * Time.deltaTime * currentScale * 1.5f);
        if (Input.mouseScrollDelta.y < 0)
        {
            currentScaleTime = Mathf.Min(1f, currentScaleTime + 0.02f);

            currentScale = cameraScaleCurve.Evaluate(currentScaleTime) * basicCameraScale;
            Camera.main.transform.position = Camera.main.ScreenToWorldPoint(mousePos) +
                currentScale / Camera.main.orthographicSize * (Camera.main.transform.position - Camera.main.ScreenToWorldPoint(mousePos));
            Camera.main.orthographicSize = currentScale;
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            currentScaleTime = Mathf.Max(0f, currentScaleTime - 0.02f);
            currentScale = cameraScaleCurve.Evaluate(currentScaleTime) * basicCameraScale;
            Camera.main.transform.position = Camera.main.ScreenToWorldPoint(mousePos) +
                currentScale / Camera.main.orthographicSize * (Camera.main.transform.position - Camera.main.ScreenToWorldPoint(mousePos));
            Camera.main.orthographicSize = currentScale;
        }
    }
}
