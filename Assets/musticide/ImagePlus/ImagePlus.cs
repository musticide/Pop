using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;

namespace Musticide.UI
{
    [ExecuteInEditMode]
    public class ImagePlus : UnityEngine.UI.Image
    {
        private static Shader m_ImagePlusShader;

        [SerializeField] private Mesh m_CustomMesh;

        public Mesh CustomMesh
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

        [SerializeField] private Sprite m_MainSprite;

        public Sprite MainSprite
        {
            get => m_MainSprite;
            set
            {
                m_MainSprite = value;
                OnMainSpriteChanged(value);
            }

        }

        private void OnMainSpriteChanged(Sprite a_MainSprite)
        {
            if (a_MainSprite == null) return;
            material.SetTexture("_MainTex", a_MainSprite.texture);
            Rect texRect = a_MainSprite.textureRect;
            Vector2 offset = new Vector2(texRect.x / a_MainSprite.texture.width, texRect.y / a_MainSprite.texture.height);
            Vector2 scale = new Vector2(texRect.width / a_MainSprite.texture.width, texRect.height / a_MainSprite.texture.height);

            material.SetTextureOffset("_MainTex", offset);
            material.SetTextureScale("_MainTex", scale);
        }

        [SerializeField] private Sprite m_SecondarySprite;
        public Sprite SecondarySprite
        {
            get => m_SecondarySprite;
            set
            {
                m_SecondarySprite = value;
                OnSecondarySpriteChanged(value);
            }
        }

        private void OnSecondarySpriteChanged(Sprite a_SecondarySprite)
        {
            if (a_SecondarySprite == null) return;
            material.SetTexture("_SecTex", a_SecondarySprite.texture);

            Rect texRect = a_SecondarySprite.textureRect;

            Vector2 offset = new Vector2(texRect.x / a_SecondarySprite.texture.width, texRect.y / a_SecondarySprite.texture.height);

            Vector2 scale = new Vector2(texRect.width / a_SecondarySprite.texture.width, texRect.height / a_SecondarySprite.texture.height);

            material.SetTextureOffset("_SecTex", offset);
            material.SetTextureScale("_SecTex", scale);
        }

        [SerializeField] private Vector4 m_SecondarySpriteUserScaleOffset = new Vector4(1, 1, 0, 0);
        public Vector4 SecondarySpriteScaleOffset
        {
            get => m_SecondarySpriteUserScaleOffset;
            set
            {
                m_SecondarySpriteUserScaleOffset = value;
                OnSecondarySpriteUserScaleOffsetChanged(value);
            }
        }

        private void OnSecondarySpriteUserScaleOffsetChanged(Vector4 value)
        {
            material.SetVector("_SecTex_UserST", value);
        }

        [SerializeField] bool m_TileSecondarySprite = false;
        public bool TileSecondarySprite
        {
            get => m_TileSecondarySprite;
            set
            {
                m_TileSecondarySprite = value;
                OnTileSecondarySpriteChanged(value);
            }
        }

        void OnTileSecondarySpriteChanged(bool value)
        {
            if (value)
            {
                material.EnableKeyword("_TILE_SECTEX");
            }
            else
            {
                material.DisableKeyword("_TILE_SECTEX");
            }

        }

        [System.Serializable]
        public enum BlendMode
        {
            Alpha,
            Additive,
            Multiply,
            Overlay
        }

        [SerializeField] private BlendMode m_TexBlendMode = BlendMode.Alpha;

        public BlendMode TexBlendMode
        {
            get => m_TexBlendMode;
            set
            {
                m_TexBlendMode = value;
                OnTexBlendModeChanged(value);
            }
        }

        [SerializeField] private bool m_ClipSecTexToBase = false;
        public bool ClipSecTexToBase
        {
            get => m_ClipSecTexToBase;
            set
            {
                m_ClipSecTexToBase = value;
                OnClipSecTexToBaseChanged(value);
            }
        }

        private void OnClipSecTexToBaseChanged(bool value)
        {
            if (value)
            {
                material.EnableKeyword("_CLIP_SECTEX_TO_BASE");
            }
            else
            {
                material.DisableKeyword("_CLIP_SECTEX_TO_BASE");
            }
        }

        [SerializeField] bool m_IsGleam = false;
        public bool IsGleam
        {
            get => m_IsGleam;
            set
            {
                m_IsGleam = value;
                OnGleamActivated(value);
            }
        }

        private void OnGleamActivated(bool value)
        {
            if (value)
            {
                material.EnableKeyword("_GLEAM");
            }
            else
            {
                material.DisableKeyword("_GLEAM");
            }
        }

        [SerializeField] Vector4 m_Gleam = new Vector4(0.2f, 0.0f, 0.5f, 2.0f);
        public Vector4 Gleam
        {
            get => m_Gleam;
            set
            {
                m_Gleam = value;
                OnGleamChanged(value);
            }
        }

        private void OnGleamChanged(Vector4 value)
        {
            material.SetVector("_Gleam", value);
        }

        private void OnTexBlendModeChanged(BlendMode value)
        {
            switch (value)
            {
                case BlendMode.Alpha:
                    material.EnableKeyword("_TEXBLENDMODE_ALPHA");
                    material.DisableKeyword("_TEXBLENDMODE_ADD");
                    material.DisableKeyword("_TEXBLENDMODE_MULTIPLY");
                    material.DisableKeyword("_TEXBLENDMODE_OVERLAY");
                    break;
                case BlendMode.Additive:
                    material.DisableKeyword("_TEXBLENDMODE_ALPHA");
                    material.EnableKeyword("_TEXBLENDMODE_ADD");
                    material.DisableKeyword("_TEXBLENDMODE_MULTIPLY");
                    material.DisableKeyword("_TEXBLENDMODE_OVERLAY");
                    break;
                case BlendMode.Multiply:
                    material.DisableKeyword("_TEXBLENDMODE_ALPHA");
                    material.DisableKeyword("_TEXBLENDMODE_ADD");
                    material.EnableKeyword("_TEXBLENDMODE_MULTIPLY");
                    material.DisableKeyword("_TEXBLENDMODE_OVERLAY");
                    break;
                case BlendMode.Overlay:
                    material.DisableKeyword("_TEXBLENDMODE_ALPHA");
                    material.DisableKeyword("_TEXBLENDMODE_ADD");
                    material.DisableKeyword("_TEXBLENDMODE_MULTIPLY");
                    material.EnableKeyword("_TEXBLENDMODE_OVERLAY");
                    break;
            }
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

            OnMaterialChanged(m_Material);

            // if (MainSprite != m_MainSprite)
            OnMainSpriteChanged(m_MainSprite);
            OnSecondarySpriteChanged(m_SecondarySprite);
            OnTexBlendModeChanged(m_TexBlendMode);
            OnSecondarySpriteUserScaleOffsetChanged(m_SecondarySpriteUserScaleOffset);
            OnTileSecondarySpriteChanged(m_TileSecondarySprite);
            OnClipSecTexToBaseChanged(m_ClipSecTexToBase);

            OnGleamChanged(m_Gleam);
            OnGleamActivated(m_IsGleam);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            EnsureRichImageShader();
        }
#endif

        Material shaderMaterial;// = new Material(Shader.Find("Hidden/musticide/UI/RichImageShader"));
        [SerializeField] private bool shaderFeatures;

        public override Material material
        {
            get
            {
                if (m_Material != null)
                {
                    return m_Material;
                }
                else
                {
                    // return new Material(m_ImagePlusShader);
                    return shaderMaterial;
                }
            }
            set
            {
                // m_Material = value;
                // OnMaterialChanged(value);
                if (value != null)
                {
                    m_Material = value;
                }
                else
                {
                    m_Material = new Material(m_ImagePlusShader);
                }

                shaderFeatures = value.shader == m_ImagePlusShader;
            }
        }

        private void OnMaterialChanged(Material value)
        {
            if (value == null)
            {
                shaderFeatures = true;
                return;
            }
            shaderFeatures = value.shader == m_ImagePlusShader;
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

        public void PreserveAspectRatio()
        {
            if (MainSprite != null)
            {
                Vector2 size = MainSprite.rect.size;
                if (size.x >= size.y)
                {
                    rectTransform.localScale = Vector3.one * rectTransform.localScale.x;
                    // rectTransform.localScale.y = (1 ,size.x /size.y, 1);
                }
                else
                {
                    rectTransform.localScale = Vector3.one * rectTransform.localScale.x;
                    // rectTransform.localScale.y *= size.x /size.y;

                }
                // float aspectRatio = 
            }
        }

        void EnsureRichImageShader()
        {
            if (m_ImagePlusShader == null)
            {
                m_ImagePlusShader = Shader.Find("Hidden/musticide/UI/ImagePlus Shader");
                if (m_ImagePlusShader == null)
                {
                    Debug.LogError("ImagePlus shader not found!");
                    return;
                }
            }
            if (m_Material == null)
            {
                shaderMaterial = new Material(m_ImagePlusShader);
                // m_Material = shaderMaterial;
            }

            // if (material == null || material.shader != m_ImagePlusShader)
            // {
            //     material = new Material(m_ImagePlusShader);
            // }
        }
    }
}
