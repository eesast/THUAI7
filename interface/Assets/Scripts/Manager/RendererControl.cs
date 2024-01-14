using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererControl : Singleton<RendererControl>
{
    MaterialPropertyBlock CurrentPropertyBlock = new MaterialPropertyBlock();
    Renderer CurrentRenderer = new Renderer();
    Tuple<Color, Color> Team0Color = new Tuple<Color, Color>(new Color(6, 0.11f, 0, 0), new Color(6f, 0.85f, 0, 0));
    Tuple<Color, Color> Team1Color = new Tuple<Color, Color>(new Color(0.141f, 0, 6, 0), new Color(0, 0.45f, 6, 0));
    void Start()
    {
    }
    // public Tuple<MaterialPropertyBlock, MaterialPropertyBlock> GetColFromTeam(int teamKey, MaterialPropertyBlock a)
    // {
    //     if(Team1Renderer == null){
    //         Team1Renderer = new Tuple<MaterialPropertyBlock, MaterialPropertyBlock>(
    //             new MaterialPropertyBlock(),
    //             new MaterialPropertyBlock()
    //         );
    //     }
    //     Team1Renderer.Item1.SetColor("_GlowColor", new Color(4, 0, 0, 0));
    //     // Team1Renderer.Item1.SetTexture("_MainTex"z );
    //     Team1Renderer.Item2.SetColor("_GlowColor", new Color(4, 0, 0, 0));
    //     if(Team2Renderer == null){
    //         Team2Renderer = new Tuple<MaterialPropertyBlock, MaterialPropertyBlock>(
    //             new MaterialPropertyBlock(),
    //             new MaterialPropertyBlock()
    //         );
    //     }
    //     Team2Renderer.Item1.SetColor("_GlowColor", new Color(4, 0, 0, 2));
    //     Team2Renderer.Item2.SetColor("_GlowColor", new Color(4, 0, 0, 2));
    //     switch (teamKey)
    //     {
    //         case 0: return Team1Renderer;
    //         case 1: return Team2Renderer;
    //         default: return null;
    //     }
    // }
    public void SetColToChild(int teamKey, Transform targetTransform)
    {
        Debug.Log("step2");
        switch (teamKey)
        {
            case 0:
                Debug.Log("step2.5");
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                Debug.Log("step2.55:" + CurrentRenderer == null);
                CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                Debug.Log("step2.6");
                CurrentPropertyBlock.SetColor("_GlowColor", Team0Color.Item1);
                CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                CurrentRenderer = targetTransform.Find("mask2").GetComponent<Renderer>();
                CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                CurrentPropertyBlock.SetColor("_GlowColor", Team0Color.Item2);
                CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                return;
            case 1:
                CurrentRenderer = targetTransform.Find("mask1").GetComponent<Renderer>();
                CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                CurrentPropertyBlock.SetColor("_GlowColor", Team1Color.Item1);
                CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                CurrentRenderer = targetTransform.Find("mask2").GetComponent<Renderer>();
                CurrentRenderer.GetPropertyBlock(CurrentPropertyBlock);
                CurrentPropertyBlock.SetColor("_GlowColor", Team1Color.Item2);
                CurrentRenderer.SetPropertyBlock(CurrentPropertyBlock);
                return;
            default:
                return;
        }
    }
}
