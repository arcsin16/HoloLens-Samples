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
    private int frameCount;
    private float prevTime;
    private float fps;
    private bool calcUVs;

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
        this.frameCount = 0;
        this.prevTime = 0;
        this.calcUVs = false;

        this.debugText = FindObjectOfType<Text>();
    }


    void OnSourcePressed(InteractionSourceState state)
    {
        var cp = Camera.main.transform.position;
        var cf = Camera.main.transform.forward;

        //RaycastHit hitInfo;
        //if (Physics.Raycast(cp, cf, out hitInfo))
        //{
        //    var meshes = SpatialMappingManager.Instance.GetSurfaceObjects();
        //    for (int i = 0; i < meshes.Count; i++)
        //    {
        //        if (meshes[i].Filter.mesh != null && meshes[i].Filter.mesh.bounds.Contains(hitInfo.point))
        //        {
        //            meshes[i].Renderer.sharedMaterial = this.SelectedMeshMaterial;
        //        }
        //        else if (meshes[i].Renderer.sharedMaterial == this.SelectedMeshMaterial)
        //        {
        //            meshes[i].Renderer.sharedMaterial = SpatialMappingManager.Instance.SurfaceMaterial;
        //        }
        //    }
        //}
        calcUVs = !calcUVs;
    }

    // Update is called once per frame
    void Update()
    {
        ++frameCount;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            this.fps = frameCount / time;
        }

        var meshes = SpatialMappingManager.Instance.GetMeshes();
        if (this.debugText != null)
        {
            StringBuilder sb = new StringBuilder();
            if (meshes != null)
            {
                sb.AppendLine("FPS: " + fps);
                sb.AppendLine("MeshCount: " + meshes.Count);
                for (int i = 0; i < meshes.Count; i++)
                {
                    var mesh = meshes[i];
                    sb.AppendFormat(" [{0}] CENTER={1}, SIZE={2}, VERTICES={3}", i, mesh.bounds.center, mesh.bounds.size, mesh.vertices.Length);

                    if(calcUVs)
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
            }
            else
            {
                sb.AppendLine("FPS: " + fps);
                sb.AppendLine("Mesh Count: null");
            }

            this.debugText.text = sb.ToString();
        }
    }
}
