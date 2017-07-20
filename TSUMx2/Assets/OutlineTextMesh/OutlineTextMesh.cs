using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (MeshRenderer))]
[RequireComponent(typeof (TextMesh))]
[ExecuteInEditMode]
public class OutlineTextMesh : MonoBehaviour
{
	public enum ShaderType {
		Normal,
		Outline,
	}

	public ShaderType shaderType = ShaderType.Normal;
	public bool duplicateOutlineMesh = true;
	public Color outlineColor = Color.black;
	public float outerThickness = 1.0f;
	public float innerThickness = 1.0f;
	[HideInInspector]
	[SerializeField]
	public OutlineMesh[] outlineMeshes;
	[HideInInspector]
	public OutlineMaterial outlineMaterial;

	[System.Serializable]
	public class OutlineMesh
	{
		public GameObject	gameObject;
		public Transform	transform;
		public TextMesh		textMesh;
		public MeshRenderer	meshRenderer;
	}

	const string ShaderName_Normal = "GUI/Text Shader";
	const string ShaderName_Outline = "GUI/Outline Text Shader";
	string _shaderName {
		get {
			switch( this.shaderType ) {
			case ShaderType.Normal:		return ShaderName_Normal;
			case ShaderType.Outline:	return ShaderName_Outline;
			}
			return "";
		}
	}

	MeshRenderer _meshRenderer;
	Material _material;
	TextMesh _textMesh;
	ShaderType _cache_shaderType;
	bool _cache_duplicateOutlineMesh;
	Color _cached_shader_outlineColor = new Color( -1.0f, -1.0f, -1.0f, -1.0f );
	float _cached_shader_outerThickness = -1.0f;
	Vector4 _cached_shader_innerThickness = new Vector4( -1.0f, -1.0f, -1.0f, -1.0f );
	float _cached_mesh_outerThickness = -1.0f;

	[System.Serializable]
	public class OutlineMaterial
	{
		public Font		font;
		public Material	material;
		public int		refCount;

		public void Release()
		{
			if( --refCount <= 0 ) {
				if( !Application.isPlaying ) {
					Material.DestroyImmediate( this.material );
				} else {
					Material.Destroy( this.material );
				}
				this.font = null;
				this.material = null;
			}
		}
	}

	static Dictionary<OutlineTextMesh, OutlineMaterial> _outlineMaterials = new Dictionary<OutlineTextMesh, OutlineMaterial>();

	OutlineMaterial _CreateOutlineMaterial( Font font )
	{
		foreach( var m in _outlineMaterials ) {
			if( m.Value.font == font ) {
				OutlineMaterial outlineMaterial = m.Value;
				++outlineMaterial.refCount;
				_outlineMaterials.Add( this, outlineMaterial );
				return outlineMaterial;
			}
		}

		{
			OutlineMaterial outlineMaterial = new OutlineMaterial();
			outlineMaterial.font = font;
			outlineMaterial.material = new Material( font.material );
			outlineMaterial.material.renderQueue = outlineMaterial.material.renderQueue - 1;
			outlineMaterial.refCount = 1;
			_outlineMaterials.Add( this, outlineMaterial );
			return outlineMaterial;
		}
	}

	void Awake()
	{
		if( this.outlineMaterial != null ) {
			_outlineMaterials.Add( this, this.outlineMaterial );
		}
	}

	void OnDestroy()
	{
		_outlineMaterials.Remove( this );
	}

	public void ForceUpdate()
	{
		// Property value fix.
		if( _cache_shaderType != this.shaderType ) {
			_cache_shaderType = this.shaderType;
			if( _cache_shaderType != ShaderType.Normal ) {
				if( this.duplicateOutlineMesh ) {
					this.duplicateOutlineMesh = false;
					_cache_duplicateOutlineMesh = this.duplicateOutlineMesh;
				}
			}
		}
		if( _cache_duplicateOutlineMesh != this.duplicateOutlineMesh ) {
			_cache_duplicateOutlineMesh = this.duplicateOutlineMesh;
			if( _cache_duplicateOutlineMesh ) {
				if( this.shaderType != ShaderType.Normal ) {
					this.shaderType = ShaderType.Normal;
					_cache_shaderType = this.shaderType;
				}
			}
		}

		if( _meshRenderer == null ) {
			_material = null;
			_meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
		}
		if( _textMesh == null ) {
			_textMesh = this.gameObject.GetComponent<TextMesh>();
		}
		
		if( _meshRenderer != null ) {
			_material = _meshRenderer.sharedMaterial;
		}

		string shaderName = _shaderName;
		if( _material == null || _material.shader == null || _material.shader.name != shaderName ) {
			if( _textMesh != null && _textMesh.font != null ) {
				if( _material != null && _material.shader != null && _material.shader.name != ShaderName_Normal ) {
					if( !Application.isPlaying ) {
						Material.DestroyImmediate( _material );
					} else {
						Material.Destroy( _material );
					}
					_material = null;
				}

				if( this.shaderType == ShaderType.Normal ) {
					_material = _textMesh.font.material; // Use default material.
					_cached_shader_outlineColor = new Color( -1.0f, -1.0f, -1.0f, -1.0f );
					_cached_shader_outerThickness = -1.0f;
					_cached_shader_innerThickness = new Vector4( -1.0f, -1.0f, -1.0f, -1.0f );
				} else {
					_material = new Material( _textMesh.font.material ); // Clone material & override font shader.
					_material.shader = Shader.Find( shaderName );
				}
				if( _meshRenderer != null ) {
					_meshRenderer.sharedMaterial = _material;
				}
			}
		}

		if( _material != null && _material.shader != null && _material.shader.name == ShaderName_Outline ) {
			Texture mainTexture = _material.mainTexture;
			if( mainTexture != null && _textMesh != null ) {
				Texture2D mainTexture2D = mainTexture as Texture2D;
				if( mainTexture2D != null ) {
					float fontSize = (float)_textMesh.fontSize;
					float width = Mathf.Max( (float)mainTexture2D.width, 1.0f );
					float height = Mathf.Max( (float)mainTexture2D.height, 1.0f );

					float shader_outerThickness = this.outerThickness * 0.005f;

					Vector4 shader_innerThickness = new Vector4(
						this.innerThickness * 0.005f * fontSize / width,
						this.innerThickness * 0.005f * fontSize / height,
						0.0f, 0.0f );

					if( _cached_shader_outlineColor != this.outlineColor ) {
						_cached_shader_outlineColor = this.outlineColor;
						if( _material.GetColor( "_OutlineColor" ) != this.outlineColor ) {
							_material.SetColor( "_OutlineColor", this.outlineColor );
						}
					}
					if( _cached_shader_outerThickness != shader_outerThickness ) {
						_cached_shader_outerThickness = shader_outerThickness;
						if( _material.GetFloat( "_OuterThickness" ) != shader_outerThickness ) {
							_material.SetFloat( "_OuterThickness", shader_outerThickness );
						}
					}
					if( _cached_shader_innerThickness != shader_innerThickness ) {
						_cached_shader_innerThickness = shader_innerThickness;
						if( _material.GetVector( "_InnerThickness" ) != shader_innerThickness ) {
							_material.SetVector( "_InnerThickness", shader_innerThickness );
						}
					}
				}
			}
		}

		// Support duplicateOutlineMesh
		if( this.duplicateOutlineMesh ) {
			if( this.outlineMeshes == null || this.outlineMeshes.Length == 0 ) {
				_cached_mesh_outerThickness = -1.0f;
				this.outlineMeshes = new OutlineMesh[8];
				for( int i = 0; i != this.outlineMeshes.Length; ++i ) {
					OutlineMesh outlineMesh = new OutlineMesh();
					this.outlineMeshes[i] = outlineMesh;

					outlineMesh.gameObject = new GameObject( this.gameObject.name + "(Outline)" );
					outlineMesh.transform = outlineMesh.gameObject.transform;
					outlineMesh.transform.parent = this.transform;
					outlineMesh.transform.localPosition = Vector3.zero;
					outlineMesh.transform.localRotation = Quaternion.identity;
					outlineMesh.transform.localScale = Vector3.one;

					outlineMesh.textMesh = outlineMesh.gameObject.AddComponent<TextMesh>();
					outlineMesh.meshRenderer = outlineMesh.gameObject.GetComponent<MeshRenderer>();
				}
			}
			if( _textMesh != null && _textMesh.font != null && _textMesh.font.material != null ) {
				if( this.outlineMaterial == null || this.outlineMaterial.font != _textMesh.font ) {
					if( this.outlineMaterial != null ) {
						_outlineMaterials.Remove( this );
						this.outlineMaterial.Release();
						this.outlineMaterial = null;
					}

					this.outlineMaterial = _CreateOutlineMaterial( _textMesh.font );
				}
			}
		} else {
			if( this.outlineMeshes != null ) {
				if( this.outlineMeshes.Length != 0 ) {
					for( int i = 0; i != this.outlineMeshes.Length; ++i ) {
						if( this.outlineMeshes[i].transform != null ) {
							if( !Application.isPlaying ) {
								GameObject.DestroyImmediate( this.outlineMeshes[i].transform.gameObject );
							} else {
								GameObject.Destroy( this.outlineMeshes[i].transform.gameObject );
							}
						}
					}
					this.outlineMeshes = new OutlineMesh[0];
				}
			}

			if( this.outlineMaterial != null ) {
				_outlineMaterials.Remove( this );
				this.outlineMaterial.Release();
				this.outlineMaterial = null;
			}
		}

		if( this.outlineMeshes != null && this.outlineMeshes.Length != 0 ) {
			bool isUpdatedThickness = (_cached_mesh_outerThickness != this.outerThickness);
			_cached_mesh_outerThickness = this.outerThickness;
			float t = _cached_mesh_outerThickness * 0.2f;
			for( int i = 0, x = -1, y = -1; i != this.outlineMeshes.Length; ++i, ++x ) {
				GameObject outlienGameObject = this.outlineMeshes[i].gameObject;
				TextMesh outlineTextMesh = this.outlineMeshes[i].textMesh;
				MeshRenderer outlineMeshRenderer = this.outlineMeshes[i].meshRenderer;
				if( isUpdatedThickness ) {
					Transform outlineTransform = this.outlineMeshes[i].transform;
					if( outlineTextMesh != null && outlineTransform != null ) {
						if( x == 2 ) {
							x = -1;
							++y;
						} else if( x == 0 && y == 0 ) {
							++x;
						}

						Vector3 localPosition = new Vector3( (float)x * t, (float)y * t, 0.0f );
						if( outlineTransform.localPosition != localPosition ) {
							outlineTransform.localPosition = localPosition;
						}
					}
				}
				if( outlienGameObject != null ) {
					if( !outlienGameObject.name.StartsWith( this.gameObject.name ) ||
					     outlienGameObject.name.Length - 9 != this.gameObject.name.Length ) {
						outlienGameObject.name = this.gameObject.name + "(Outline)";
					}
				}
				if( outlineTextMesh != null && _textMesh != null ) {
					if( outlineTextMesh.text != _textMesh.text ) {
						outlineTextMesh.text = _textMesh.text;
					}
					if( outlineTextMesh.offsetZ != _textMesh.offsetZ ) {
						outlineTextMesh.offsetZ = _textMesh.offsetZ;
					}
					if( outlineTextMesh.characterSize != _textMesh.characterSize ) {
						outlineTextMesh.characterSize = _textMesh.characterSize;
					}
					if( outlineTextMesh.lineSpacing != _textMesh.lineSpacing ) {
						outlineTextMesh.lineSpacing = _textMesh.lineSpacing;
					}
					if( outlineTextMesh.anchor != _textMesh.anchor ) {
						outlineTextMesh.anchor = _textMesh.anchor;
					}
					if( outlineTextMesh.alignment != _textMesh.alignment ) {
						outlineTextMesh.alignment = _textMesh.alignment;
					}
					if( outlineTextMesh.tabSize != _textMesh.tabSize ) {
						outlineTextMesh.tabSize = _textMesh.tabSize;
					}
					if( outlineTextMesh.fontSize != _textMesh.fontSize ) {
						outlineTextMesh.fontSize = _textMesh.fontSize;
					}
					if( outlineTextMesh.fontStyle != _textMesh.fontStyle ) {
						outlineTextMesh.fontStyle = _textMesh.fontStyle;
					}
					if( outlineTextMesh.richText != _textMesh.richText ) {
						outlineTextMesh.richText = _textMesh.richText;
					}
					if( outlineTextMesh.font != _textMesh.font ) {
						outlineTextMesh.font = _textMesh.font;
					}
					if( outlineTextMesh.color != this.outlineColor ) {
						outlineTextMesh.color = this.outlineColor;
					}
				}
				if( outlineMeshRenderer != null && this.outlineMaterial != null && this.outlineMaterial.material != null ) {
					if( outlineMeshRenderer.sharedMaterial != this.outlineMaterial.material ) {
						outlineMeshRenderer.sharedMaterial = this.outlineMaterial.material;
					}
				}
			}
		}
	}

	void LateUpdate()
	{
		ForceUpdate();
	}
}
