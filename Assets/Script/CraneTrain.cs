using UnityEngine;
using System;
using Unity.Mathematics;

public class CraneTrain : MonoBehaviour
{
    [SerializeField]
    private float maxValueZ = 0.4f;

    [SerializeField]
    private float minValueZ = -0.4f;

    [SerializeField]
    private float speed = 5f;
    private Vector2 input_move_vec;

    private void Start()
    {
        EventManager.Instance.AddEventListner<Vec2Args>("MoveClaw", SetMoveVec);
    }

    private void SetMoveVec(object sender, Vec2Args look)
    {
        Vec2Args vec2Args = look;
        Vector2 vec2 = vec2Args.vec;
        input_move_vec = vec2;
    }

    private void Update()
    {
        if ( input_move_vec.y != 0)
        {
            Vector3 pos = this.transform.localPosition;
            float input_z = input_move_vec.y * -1;

            float next_z = pos.z + ( input_z * speed * Time.deltaTime);
            float z = Mathf.Clamp( next_z, minValueZ, maxValueZ );
            this.transform.localPosition = new Vector3(pos.x, pos.y, z);
        }
    }
}
