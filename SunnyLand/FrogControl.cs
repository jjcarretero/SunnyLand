using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogControl : MonoBehaviour
{
	public GameObject Waypoint1, Waypoint2;
	public GameObject Point1, Point2;

	private Rigidbody2D rb;
	public LayerMask Ground;
	public Animator Anim;

	public float jumpLenght = 3f;
	public float jumpHeight = 4f;

	public bool b_LookRight = false;
	public bool b_IsGrounded = false;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		Anim = GetComponent<Animator>();
	}


	private void Update()
	{
		b_IsGrounded = Physics2D.OverlapArea(Point1.transform.position, Point2.transform.position, Ground);

		if (Anim.GetBool("Jump"))
		{
			if (rb.velocity.y < .1)
			{
				Anim.SetBool("Fall", true);
				Anim.SetBool("Jump", false);
			}
		}
		if (b_IsGrounded && Anim.GetBool("Fall"))
		{
			Anim.SetBool("Fall", false);
		}
	}


	private void Move()
	{
		if (b_LookRight)
		{
			if (transform.position.x < Waypoint2.transform.position.x)
			{
				if (transform.localScale.x != -3.5f)
					transform.localScale = new Vector3(-3.5f, 3.5f);
				if (b_IsGrounded)
				{
					rb.velocity = new Vector2(jumpLenght, jumpHeight);
					Anim.SetBool("Jump", true);
				}
			}
			else
				b_LookRight = false;
		}
		else
		{
			if (transform.position.x > Waypoint1.transform.position.x)
			{
				if (transform.localScale.x != 3.5f)
					transform.localScale = new Vector3(3.5f, 3.5f);
				if (b_IsGrounded)
				{
					rb.velocity = new Vector2(-jumpLenght, jumpHeight);
					Anim.SetBool("Jump", true);
				}
			}
			else
			{
				b_LookRight = true;
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D col)
	{

		if (col.gameObject.tag == "Enemy")
		{
			Physics2D.IgnoreCollision(col.gameObject.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
		}
	}
}
