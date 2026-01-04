using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 3;

    private Vector2 input_move_vec = Vector2.zero;
    private Rigidbody rigid;

    private GameObject head;
    private GameObject interactionObject;

    private bool isInteraction = false;
    private bool isCrawing = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        head = this.gameObject.GetNamedChild("FowardObject/Head");
    }

    private void Start()
    {
        EventManager.Instance.AddEventListner<Vec2Args>("Move", SetMoveVec);
        EventManager.Instance.AddEventListner("Interaction", Interaction);
    }
    private void FixedUpdate()
    {
        if (CheckInteraction())
        {
            isInteraction = true;
        }
        else
        {
            isInteraction = false;
        }

        if(isCrawing) return;

        Move();
    }

    private void SetMoveVec(object sender, Vec2Args look)
    {
        Vec2Args vec2Args = look;
        Vector2 vec2 = vec2Args.vec;
        input_move_vec = vec2;
    }

    private void Move()
    {
        if (!head) return;
        Vector3 foward_vec = head.transform.forward;
        Vector3 foward_direction = foward_vec.normalized;

        Vector3 right_vec = head.transform.right;
        Vector3 right_direction = right_vec.normalized;

        Vector3 dir = (foward_direction * input_move_vec.y) + (right_direction * input_move_vec.x);
        Vector3 move_velocity = dir.normalized * (speed * Time.deltaTime);

        rigid.MovePosition(this.transform.position + move_velocity);
    }

    private void Interaction(object sender, EventArgs args)
    {
        if (isInteraction)
        {
            isCrawing = true;
        }
        else
        {
            isCrawing = false;
        }
    }

    private bool CheckInteraction()
    {
        Vector3 foward_vec = head.transform.forward;
        Vector3 foward_direction = foward_vec.normalized;
        RaycastHit layer_hit = new RaycastHit();
        if ( Physics.Raycast(this.transform.position, foward_direction, out layer_hit, 1 , LayerMask.GetMask("Interaction") ) )
        {
            interactionObject = layer_hit.collider.gameObject;
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Vector3 foward_vec = Vector3.forward;
        if (head)
        {
            foward_vec = head.transform.forward;
        }
        else
        {
            head = this.gameObject.GetNamedChild("FowardObject/Head");
            foward_vec = head.transform.forward;
        }

        Vector3 foward_direction = foward_vec.normalized;


        Gizmos.DrawLine(this.transform.position, this.transform.position + foward_direction * 1);
    }
}

