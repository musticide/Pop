using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[ExecuteInEditMode]
public class RichImage : MaskableGraphic
{
    private static Shader m_RichImageShader;

    /* [SerializeField]
    private Vector2
        blUV,
        tlUV,
        trUV,
        brUV; */

    [SerializeField] private Mesh m_Mesh;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (m_Mesh == null) return;

        var vertices = m_Mesh.vertices;
        var uv = m_Mesh.uv;
        var tris = m_Mesh.triangles;

        Bounds meshBounds = m_Mesh.bounds;
        Vector3 meshSize = meshBounds.size;
        Vector3 meshCenter = meshBounds.center;

        Vector2 rectSize = rectTransform.rect.size;

        float scale = Mathf.Min(rectSize.x/meshSize.x, rectSize.y/meshSize.y);


        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = vertices[i];

            v -= meshCenter;
            v *= scale;
            v += meshCenter;

            // v.x += rectSize.x * 0.5f;
            // v.y += rectSize.y * 0.5f;

            UIVertex vert = UIVertex.simpleVert;
            vert.position = v;
            vert.uv0 = (uv != null && uv.Length > i) ? uv[i] : Vector2.zero;
            vert.color = color;
            vh.AddVert(vert);
        }

        for (int i = 0; i < tris.Length; i+=3)
        {
            vh.AddTriangle(tris[i], tris[i + 1], tris[i + 2]);
    }
}

/* protected override void OnPopulateMesh(VertexHelper vh)
{
    Vector2 corner1 = Vector2.zero;
    Vector2 corner2 = Vector2.zero;

    corner1.x = 0f;
    corner1.y = 0f;
    corner2.x = 1f;
    corner2.y = 1f;

    corner1.x -= rectTransform.pivot.x;
    corner1.y -= rectTransform.pivot.y;
    corner2.x -= rectTransform.pivot.x;
    corner2.y -= rectTransform.pivot.y;

    corner1.x *= rectTransform.rect.width;
    corner1.y *= rectTransform.rect.height;
    corner2.x *= rectTransform.rect.width;
    corner2.y *= rectTransform.rect.height;

    vh.Clear();

    UIVertex vert = UIVertex.simpleVert;

    //bottomLeft
    vert.position = new Vector2(corner1.x, corner1.y);
    vert.color = color;
    vert.uv0.x += blUV.x;
    vert.uv0.y += blUV.y;
    vh.AddVert(vert);

    //topLeft
    vert.position = new Vector2(corner1.x, corner2.y);
    vert.color = color;
    vert.uv0.x += tlUV.x;
    vert.uv0.y += tlUV.y;
    vh.AddVert(vert);

    //topRight
    vert.position = new Vector2(corner2.x, corner2.y);
    vert.color = color;
    vert.uv0.x += trUV.x;
    vert.uv0.y += trUV.y;
    vh.AddVert(vert);

    //bottomRight
    vert.position = new Vector2(corner2.x, corner1.y);
    vert.color = color;
    vert.uv0.x += brUV.x;
    vert.uv0.y += brUV.y;
    vh.AddVert(vert);

    vh.AddTriangle(0, 1, 2);
    vh.AddTriangle(2, 3, 0);
} */

protected override void OnValidate()
{
    base.OnValidate();
    EnsureRichImageShader();

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
