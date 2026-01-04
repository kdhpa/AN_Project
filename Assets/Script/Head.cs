using System;
using UnityEngine;


public class Head : MonoBehaviour
{
    private Vector2 input_look_vec = Vector2.zero;
    private float xRotation = 0f;

    private Camera camera;

    [SerializeField]
    private float horiViewSpeed;
    [SerializeField]
    private float verViewSpeed;

    private void Awake()
    {
        camera = Camera.main;
    }
    private void Start()
    {
        EventManager.Instance.AddEventListner<Vec2Args>("Look", SetLookVec);
    }

    private void SetLookVec(object sender, EventArgs look)
    {
        Vec2Args vec2Args = look as Vec2Args;
        Vector2 vec2 = vec2Args.vec;
        input_look_vec = vec2;
    }

    private void Update()
    {
        Rotation(); 
    }

    private void Rotation()
    {
        Vector3 input = new Vector3(
            input_look_vec.y * horiViewSpeed * Time.deltaTime,
            input_look_vec.x * verViewSpeed * Time.deltaTime,
            0f
        );

        xRotation -= input.x;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        this.transform.Rotate(Vector3.up, input.y, Space.Self);

        Vector3 currentEuler = transform.eulerAngles;
        camera.transform.rotation = Quaternion.Euler(xRotation, currentEuler.y, 0f);
    }
}
