using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class RendererControl : Singleton<RendererControl>
{

    Renderer CurrentRenderer = new Renderer();
    MaterialPropertyBlock CurrentPropertyBlock = new MaterialPropertyBlock();
    Color litColor = new Color();
    public void SetColToChild(int teamId, Transform targetTransform)
    {
        switch (teamId)
        {
            case 0:
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team0Color[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity),
                        ParaDefine.GetInstance().Team0Color[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity),
                        ParaDefine.GetInstance().Team0Color[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                CurrentRenderer = targetTransform.Find("mask2").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team0Color[1].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity),
                        ParaDefine.GetInstance().Team0Color[1].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity),
                        ParaDefine.GetInstance().Team0Color[1].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                return;
            case 1:
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team1Color[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity),
                        ParaDefine.GetInstance().Team1Color[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity),
                        ParaDefine.GetInstance().Team1Color[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                CurrentRenderer = targetTransform.Find("mask2").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team1Color[1].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity),
                        ParaDefine.GetInstance().Team1Color[1].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity),
                        ParaDefine.GetInstance().Team1Color[1].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                return;
            default: return;
        }
    }
    public void SetColToChild(PlaceType placeType, Transform targetTransform)
    {
        switch (placeType)
        {
            case PlaceType.Resource:
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().ResourceColor[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        ParaDefine.GetInstance().ResourceColor[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        ParaDefine.GetInstance().ResourceColor[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                CurrentRenderer = targetTransform.Find("mask2").GetComponent<Renderer>();
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().ResourceColor[1].color.r * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[1].idensity),
                        ParaDefine.GetInstance().ResourceColor[1].color.g * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[1].idensity),
                        ParaDefine.GetInstance().ResourceColor[1].color.b * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[1].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                return;
            default: return;
        }
    }
}
