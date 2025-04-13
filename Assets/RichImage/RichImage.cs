using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class RichImage : UnityEngine.UI.Image
{
    private static Shader m_RichImageShader;

    [SerializeField] private Mesh m_CustomMesh;

    private Mesh CustomMesh
    {
        get => m_CustomMesh;
        set
        {
            if (m_CustomMesh != value)
            {
                m_CustomMesh = value;
                OnCustomMeshChanged(value);
            }
        }
    }

    private void OnCustomMeshChanged(Mesh m_CustomMesh)
    {
    }

    [SerializeField]
    private Sprite
        m_MainSprite,
        m_SecondarySprite;

    public Sprite MainSprite
    {
        get => m_MainSprite;
        set
        {
            m_MainSprite = value;
            OnMainSpriteChanged(value);
        }

    }

    public Sprite SecondarySprite
    {
        get => m_SecondarySprite;
        set
        {
            m_SecondarySprite = value;
            OnSecondarySpriteChanged(value);
        }
    }

    private void OnMainSpriteChanged(Sprite a_MainSprite)
    {
        material.SetTexture("_MainTex", a_MainSprite.texture);
        Rect texRect = a_MainSprite.textureRect;
        Vector2 offset = new Vector2(texRect.x / a_MainSprite.texture.width, texRect.y / a_MainSprite.texture.height);
        Vector2 scale = new Vector2(texRect.width / a_MainSprite.texture.width, texRect.height / a_MainSprite.texture.height);

        material.SetTextureOffset("_MainTex", offset);
        material.SetTextureScale("_MainTex", scale);
    }

    private void OnSecondarySpriteChanged(Sprite a_SecondarySprite)
    {
        material.SetTexture("_SecTex", a_SecondarySprite.texture);
        Rect texRect = a_SecondarySprite.textureRect;
        Vector2 offset = new Vector2(texRect.x / a_SecondarySprite.texture.width, texRect.y / a_SecondarySprite.texture.height);
        Vector2 scale = new Vector2(texRect.width / a_SecondarySprite.texture.width, texRect.height / a_SecondarySprite.texture.height);

        material.SetTextureOffset("_SecTex", offset);
        material.SetTextureScale("_SecTex", scale);
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (m_CustomMesh == null)
        {
            base.OnPopulateMesh(vh);
        }
        else
        {
            var vertices = m_CustomMesh.vertices;
            var uv = m_CustomMesh.uv;
            var tris = m_CustomMesh.triangles;

            Bounds meshBounds = m_CustomMesh.bounds;
            Vector3 meshSize = meshBounds.size;
            Vector3 meshCenter = meshBounds.center;

            Vector2 rectSize = rectTransform.rect.size;

            float scale = Mathf.Min(rectSize.x / meshSize.x, rectSize.y / meshSize.y);


            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i];

                v -= meshCenter;
                v *= scale;
                v += meshCenter;

                UIVertex vert = UIVertex.simpleVert;
                vert.position = v;
                vert.uv0 = (uv != null && uv.Length > i) ? uv[i] : Vector2.zero;
                vert.color = color;
                vh.AddVert(vert);
            }

            for (int i = 0; i < tris.Length; i += 3)
            {
                vh.AddTriangle(tris[i], tris[i + 1], tris[i + 2]);
            }
        }
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        EnsureRichImageShader();

        // if (MainSprite != m_MainSprite)
            OnMainSpriteChanged(m_MainSprite);
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureRichImageShader();
    }
#endif

    public override Material material
    {
        get => base.material;
        set
        {
            if (value != null && value.shader == m_RichImageShader)
            {
                base.material = value;
            }
        }
    }

    public void SetToNativeSize()
    {
        if (MainSprite != null)
        {
            Vector2 size = MainSprite.rect.size;
            Vector2 pivot = MainSprite.pivot;

            rectTransform.sizeDelta = size;
            rectTransform.pivot = new Vector2(pivot.x / size.x, pivot.y / size.y);
        }
        else
        {
            Debug.LogWarning("MainSprite is null. Cannot set native size.");
        }
    }

    void EnsureRichImageShader()
    {
        if (m_RichImageShader == null)
        {
            m_RichImageShader = Shader.Find("Hidden/musticide/UI/RichImageShader");
            if (m_RichImageShader == null)
            {
                Debug.LogError("RichImage shader not found!");
                return;
            }
        }

        if (material == null || material.shader != m_RichImageShader)
        {
            material = new Material(m_RichImageShader);
        }
    }
}

