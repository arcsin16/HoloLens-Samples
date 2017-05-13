using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;
using HoloToolkit.Unity.SpatialMapping;

public class SpatialMappingMeshTest : MonoBehaviour
{
    public Material SelectedMeshMaterial;
    private Text debugText;

    void OnEnable()
    {
        InteractionManager.SourcePressed += OnSourcePressed;
    }

    void OnDisable()
    {
        InteractionManager.SourcePressed -= OnSourcePressed;
    }

    // Use this for initialization
    void Start()
    {
        this.debugText = FindObjectOfType<Text>();
    }


    void OnSourcePressed(InteractionSourceState state)
    {
        var cp = Camera.main.transform.position;
        var cf = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(cp, cf, out hitInfo))
        {
            var meshes = SpatialMappingManager.Instance.GetSurfaceObjects();
            for (int i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].Filter.mesh != null && meshes[i].Filter.mesh.bounds.Contains(hitInfo.point))
                {
                    meshes[i].Renderer.sharedMaterial = this.SelectedMeshMaterial;
                }
                else if (meshes[i].Renderer.sharedMaterial == this.SelectedMeshMaterial)
                {
                    meshes[i].Renderer.sharedMaterial = SpatialMappingManager.Instance.SurfaceMaterial;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var meshes = SpatialMappingManager.Instance.GetMeshes();
        if (this.debugText != null)
        {
            if (meshes != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MeshCount: " + meshes.Count);
                for (int i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    sb.AppendFormat(" [{0}] CENTER={1}, SIAZE={2}", i, mesh.bounds.center, mesh.bounds.size);

                    {
                        sb.AppendFormat("UpdateUVs");
                        Vector3[] vertices = mesh.vertices;
                        Vector2[] uvs = new Vector2[vertices.Length];

                        float minu = float.MaxValue;
                        float minv = float.MaxValue;
                        float maxu = float.MinValue;
                        float maxv = float.MinValue;

                        for (int j = 0; j < uvs.Length; j++)
                        {
                            var uv = new Vector2(vertices[j].x, vertices[j].z);
                            uvs[j] = uv;
                            minu = Mathf.Min(minu, uv.x);
                            maxu = Mathf.Max(maxu, uv.x);
                            minv = Mathf.Min(minv, uv.y);
                            maxv = Mathf.Max(maxv, uv.y);
                        }
                        float usize = maxu - minu;
                        float vsize = maxv - minv;
                        for (int j = 0; j < uvs.Length; j++)
                        {
                            uvs[j].x = (uvs[j].x - minu) / usize;
                            uvs[j].y = (uvs[j].y - minv) / vsize;
                        }
                        mesh.uv = uvs;
                    }
                    sb.AppendLine();
                }
                this.debugText.text = sb.ToString();
            }
            else
            {
                this.debugText.text = "Mesh Count: null";
            }
        }
    }
}
