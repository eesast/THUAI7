using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    Vector2 mousePos;
    public AnimationCurve cameraScaleCurve;
    public RectTransform sideBarRect;
    public float currentScaleTime = 0.5f, basicCameraScale, currentScale;
    public float cameraSpeedMax = 1.5f, cameraSpeed;
    void Start()
    {
        currentScale = cameraScaleCurve.Evaluate(currentScaleTime) * basicCameraScale;
        Camera.main.orthographicSize = currentScale;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        // Debug.Log(mousePos);
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            cameraSpeed = Mathf.Lerp(cameraSpeed, cameraSpeedMax, 0.1f);
            if (Input.GetKey(KeyCode.A) && transform.position.x >= -0.5f)
            {
                transform.Translate(Vector3.left * Time.deltaTime * currentScale * cameraSpeed);
            }
            if (Input.GetKey(KeyCode.D) && transform.position.x <= 49.5f)
            {
                transform.Translate(Vector3.right * Time.deltaTime * currentScale * cameraSpeed);
            }
            if (Input.GetKey(KeyCode.W) && transform.position.y <= 49.5f)
            {
                transform.Translate(Vector3.up * Time.deltaTime * currentScale * cameraSpeed);
            }
            if (Input.GetKey(KeyCode.S) && transform.position.y >= -0.5f)
            {
                transform.Translate(Vector3.down * Time.deltaTime * currentScale * cameraSpeed);
            }
        }
        else
        {
            cameraSpeed = Mathf.Lerp(cameraSpeed, 0, 0.1f);
        }
        if (mousePos.x > 0 && mousePos.x < Screen.width && mousePos.y > 0 && mousePos.y < Screen.height)
        {
            if (Camera.main.ScreenToWorldPoint(mousePos).x > -1 &&
            Camera.main.ScreenToWorldPoint(mousePos).x < 50 &&
            Camera.main.ScreenToWorldPoint(mousePos).y > -1 &&
            Camera.main.ScreenToWorldPoint(mousePos).y < 50)
            {
                if (!sideBarRect || (sideBarRect && !RectTransformUtility.RectangleContainsScreenPoint(sideBarRect, Input.mousePosition, Camera.main)))
                {
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
        }
        // Debug.Log(RectTransformUtility.RectangleContainsScreenPoint(sideBarRect, Input.mousePosition, Camera.main));
    }
}
