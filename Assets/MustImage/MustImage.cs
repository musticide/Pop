using UnityEngine;
using UnityEngine.UI;

// [ExecuteInEditMode]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class MustImage : Image
{
    private static Shader mustImageShader;
    private static readonly int SecondTexID = Shader.PropertyToID("_SecondTex");
    private static readonly int BlendID = Shader.PropertyToID("_Blend");

    private Vector2Int sizeCorrection = Vector2Int.one * 50;

    public Sprite secondSprite;
    // [Range(0, 1)][SerializeField] private float blendFactor = 0.5f;


    protected override void Awake()
    {
        base.Awake();
        EnsureCustomShader();
    }

#if UNITY_EDITOR
    protected override void Reset()
    {
        base.Reset();
        EnsureCustomShader();
    }
#endif

    private void EnsureCustomShader()
    {
        if (mustImageShader == null)
        {
            // Load the shader (Ensure it's included in the build)
            mustImageShader = Shader.Find("UI/MustImage");

            if (mustImageShader == null)
            {
                Debug.LogError("CustomUIImage: Shader not found! Ensure 'UI/CustomShader' exists.");
                return;
            }
        }

        // Assign material with the enforced shader
        if (material == null || material.shader != mustImageShader)
        {
            material = new Material(mustImageShader);
        }
    }

    // Ensure it works in edit mode
    protected override void OnValidate()
    {
        EnsureCustomShader();
        base.OnValidate();
    }

    public override void SetNativeSize()
    {
        // this.rectTransform.rect =  
        base.SetNativeSize();
        // this.rectTransform.sizeDelta += sizeCorrection;
        // TODO 

    }

    // Prevent material override
    public override Material material
    {
        get => base.material;
        set
        {
            if (value != null && value.shader == mustImageShader)
            {
                base.material = value;
            }
        }
    }
}
