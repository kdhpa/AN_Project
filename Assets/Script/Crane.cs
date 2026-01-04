using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField]
    private float maxValueX = 0.0035f;

    [SerializeField]
    private float minValueX = -0.0035f;

    [SerializeField]
    private float speed = 1f;

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
        if ( input_move_vec.x != 0)
        {
            Vector3 pos = this.transform.localPosition;
            float input_x = input_move_vec.x;

            float next_y = pos.y + ( input_x * speed * Time.deltaTime);
            float y = Mathf.Clamp( next_y, minValueX, maxValueX );
            this.transform.localPosition = new Vector3(pos.x, y, pos.z);
        }
    }
}
