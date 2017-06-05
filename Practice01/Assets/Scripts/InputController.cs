using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Input;


public class InputController : MonoBehaviour {
    public GameObject prefab;

    private Color[] colors = new Color[] {
        Color.red, Color.blue, Color.cyan, Color.green, Color.white, Color.gray
    };

    void OnEnable()
    {
        // AirTapイベントを登録
        InteractionManager.SourcePressed += OnSourcePressed;
    }

    void OnDisable()
    {
        // AirTapイベントを解除
        InteractionManager.SourcePressed -= OnSourcePressed;
    }

    void Update()
    {
        // Unity Player用にキーボードイベント処理
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateCube();
        }
    }

    void OnSourcePressed(InteractionSourceState state)
    {
        CreateCube();
    }

    // Prefabからオブジェクト生成し、カーソルの位置に生成する。
    private void CreateCube()
    {
        // カメラの向きにあるオブジェクトを取得する
        var from = Camera.main.transform.position;
        var to = Camera.main.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(from, to, out hit, 10f))
        {
            // SpatialMappingのメッシュをタップした時だけに限定したい場合
            //if (hit.collider.gameObject.layer == 31)
            {
                // 位置、回転を指定して、オブジェクトを生成する
                var obj = Instantiate(prefab, hit.point, Quaternion.identity);

                // 色をランダムに切り替える
                var renderer = obj.GetComponent<Renderer>();
                renderer.material.color = colors[Random.Range(0, colors.Length - 1)];

                // 2秒後に破棄する
                Destroy(obj, 2f);
            }
        }
    }

}
