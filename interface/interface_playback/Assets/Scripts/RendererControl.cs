using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;

public class RendererControl : Singleton<RendererControl>
{

    Renderer CurrentRenderer = new Renderer();
    MaterialPropertyBlock CurrentPropertyBlock = new MaterialPropertyBlock();
    Color litColor = new Color();
    public void SetColToChild(PlayerTeam playerTeam, Transform targetTransform, float idensityRevise = 1)
    {
        switch (playerTeam)
        {
            case PlayerTeam.Red:
                Debug.Log("paintred");
                if (targetTransform.Find("mask1").TryGetComponent<Renderer>(out CurrentRenderer))
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team0Color[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity * idensityRevise),
                        ParaDefine.GetInstance().Team0Color[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity * idensityRevise),
                        ParaDefine.GetInstance().Team0Color[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[0].idensity * idensityRevise),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                if (targetTransform.Find("mask2"))
                {
                    if (targetTransform.Find("mask2").TryGetComponent<Renderer>(out CurrentRenderer))
                    {
                        CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                        litColor = new Color(
                            ParaDefine.GetInstance().Team0Color[1].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity * idensityRevise),
                            ParaDefine.GetInstance().Team0Color[1].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity * idensityRevise),
                            ParaDefine.GetInstance().Team0Color[1].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team0Color[1].idensity * idensityRevise),
                            0);
                        CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                        CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                    }
                }
                return;
            case PlayerTeam.Blue:
                if (targetTransform.Find("mask1").TryGetComponent<Renderer>(out CurrentRenderer))
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().Team1Color[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity * idensityRevise),
                        ParaDefine.GetInstance().Team1Color[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity * idensityRevise),
                        ParaDefine.GetInstance().Team1Color[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[0].idensity * idensityRevise),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                if (targetTransform.Find("mask2"))
                {
                    if (targetTransform.Find("mask2").TryGetComponent<Renderer>(out CurrentRenderer))
                    {
                        CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                        litColor = new Color(
                            ParaDefine.GetInstance().Team1Color[1].color.r * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity * idensityRevise),
                            ParaDefine.GetInstance().Team1Color[1].color.g * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity * idensityRevise),
                            ParaDefine.GetInstance().Team1Color[1].color.b * Mathf.Pow(2, ParaDefine.GetInstance().Team1Color[1].idensity * idensityRevise),
                            0);
                        CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                        CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                    }
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
                        ParaDefine.GetInstance().ResourceColor[0].color.r * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        ParaDefine.GetInstance().ResourceColor[0].color.g * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        ParaDefine.GetInstance().ResourceColor[0].color.b * Mathf.Pow(2, ParaDefine.GetInstance().ResourceColor[0].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                return;
            default: return;
        }
    }
    public void SetColToChild(BulletType bulletType, PlayerTeam playerTeam, Transform targetTransform)
    {
        switch (bulletType)
        {
            case BulletType.Laser:
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                // Debug.Log("render bullet");
                if (CurrentRenderer)
                {
                    CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                    litColor = new Color(
                        ParaDefine.GetInstance().LaserColor[(int)playerTeam].color.r * Mathf.Pow(2, ParaDefine.GetInstance().LaserColor[(int)playerTeam].idensity),
                        ParaDefine.GetInstance().LaserColor[(int)playerTeam].color.g * Mathf.Pow(2, ParaDefine.GetInstance().LaserColor[(int)playerTeam].idensity),
                        ParaDefine.GetInstance().LaserColor[(int)playerTeam].color.b * Mathf.Pow(2, ParaDefine.GetInstance().LaserColor[(int)playerTeam].idensity),
                        0);
                    CurrentPropertyBlock.SetColor("_GlowColor", litColor);
                    CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                }
                return;
            default: return;
        }
    }
}
