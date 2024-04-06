using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcTrailControl : MonoBehaviour
{
    TrailRenderer trailRenderer;
    public float NoiseScale, OffsetScale;
    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i <= trailRenderer.positionCount - 1; i++)
        {
            var pointPosition = trailRenderer.GetPosition(i);

            var sTime = Time.timeSinceLevelLoad * 0.2f;//防止重复播放时noise重复

            float xCoord = pointPosition.x * NoiseScale + sTime;
            float yCoord = pointPosition.y * NoiseScale + sTime;
            float zCoord = pointPosition.z * NoiseScale + sTime;

            //越接近尾端的point的位移会越大* (_lineRenderer.positionCount - 1 - i)，来近似烟雾飘散的感觉
            pointPosition.x += (Mathf.PerlinNoise(yCoord, zCoord) - 0.5f) * OffsetScale;// * (trailRenderer.positionCount - 1 - i);
            pointPosition.y += (Mathf.PerlinNoise(xCoord, zCoord) - 0.5f) * OffsetScale;// * (trailRenderer.positionCount - 1 - i);
            pointPosition.z += (Mathf.PerlinNoise(xCoord, yCoord) - 0.5f) * OffsetScale;// * (trailRenderer.positionCount - 1 - i);

            trailRenderer.SetPosition(i, pointPosition);
        }
    }
}
