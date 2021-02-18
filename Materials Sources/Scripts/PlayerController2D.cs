using Assets.Util;
using System;
using UnityEngine;

namespace Assets.Actor.Movement
{
	public class PlayerController2D : MonoBehaviour
	{
		private Vector2 moveInput;
		private float flyTime;

		private bool isGrounded;
		private bool isFacingRight;

		private bool toJump;
		private bool inJump;
		private bool inFall;
		private bool toContinueJump;

		private Animator animator;
		private Rigidbody2D body;

		private RaycastHit2D[] hitBuffer;
		private ContactFilter2D filter;

		float maxVelocityY = 0;

		[Header("CheckRadius")]
		public float groundDetectionLength;
		public float shellLength;
		public float wallMaxNormalY;
		[Header("MoveOptions")]
		public float maxSpeed;
		public float backwardSpeed;
		[Header("JumpOptions")]
		public float fallAnimationMinHeight;
		public float jumpInitialSpeed;
		public float jumpMaxSpeed;
		public float jumpDeceleration;
		public float flyAnimationDelay;

		public bool IsFlipLocked { get; set; }

		private void Start()
		{
			hitBuffer = new RaycastHit2D[16];
			filter = new ContactFilter2D()
			{
				layerMask = ~(1 << 8)
			};

			isGrounded = false;
			isFacingRight = true;
			toJump = false;

			body = GetComponent<Rigidbody2D>();
			body.freezeRotation = true;
			animator = GetComponent<Animator>();

			IsFlipLocked = false;
		}

        private void Update()
		{
			moveInput = InputUtil.GetMove();
			toJump = isGrounded && InputUtil.GetJump();
			toContinueJump = InputUtil.GetContinuousJump();

			CalculateGrounding();
			CalculateMove();
			SetAnimationState();
		}

		private void CalculateGrounding()
		{
			isGrounded = false;
			int hitCount = body.Cast(Vector2.down, filter, hitBuffer, groundDetectionLength);

			if (hitCount != 0) 
			{
				isGrounded = true;
				inJump = false;
				flyTime = 0;
			}
			else
			{
				flyTime += Time.deltaTime;
			}

			if (flyTime > flyAnimationDelay)
            {
				hitCount = body.Cast(Vector2.down, filter, hitBuffer, fallAnimationMinHeight);
				inFall = inFall || hitCount == 0;
			}
			else
            {
				inFall = false;
            }
		}

		private void CalculateMove()
		{
			Vector2 targetVelocity = new Vector2(moveInput.x * maxSpeed, body.velocity.y);

			if (!isGrounded)
            {
				targetVelocity.y -= 10 * Time.deltaTime;
            }
			
			if (targetVelocity.y > jumpMaxSpeed)
            {
				targetVelocity.y = jumpMaxSpeed;
			}

			if (toJump)
			{
				isGrounded = false;
				inJump = true;
				targetVelocity.y = jumpInitialSpeed;
			}

			if (inJump && targetVelocity.y > 0 && !toContinueJump)
            {
				targetVelocity.y -= jumpDeceleration;
				targetVelocity.y = (targetVelocity.y < 0) ? 0 : targetVelocity.y;
            }

			if ((moveInput.x > 0 && !isFacingRight) || (moveInput.x < 0 && isFacingRight))
			{
				Flip();
			}

			targetVelocity = ReduceVelocityToWall(targetVelocity, Vector2.right);
			targetVelocity = ReduceVelocityToWall(targetVelocity, Vector2.left);

			body.velocity = targetVelocity;
		}

		private Vector2 ReduceVelocityToWall(Vector2 velocity, Vector2 wallDirection)
        {
			int hitCount = body.Cast(wallDirection, filter, hitBuffer, shellLength);

			for (int i = 0; i < hitCount; i++)
			{
				if (hitBuffer[i].normal.y < 0.65 && velocity.x * wallDirection.x > 0)
                {
					velocity.x = 0;
					return velocity;
                }
			}

			return velocity;
		}

		private void Flip()
		{
			if (!IsFlipLocked)
			{
				isFacingRight = !isFacingRight;
				transform.forward *= -1;
			}
		}

		private void SetAnimationState()
		{
			float velocityScale = Math.Abs(body.velocity.x) / maxSpeed;
			if (IsFlipLocked)
            {
				if ((isFacingRight && body.velocity.x < 0) || (!isFacingRight && body.velocity.x > 0))
                {
					velocityScale *= -1;
					Vector2 velocity = body.velocity;
					velocity.x = (velocity.x > backwardSpeed) ? backwardSpeed : velocity.x;
					velocity.x = (velocity.x < -backwardSpeed) ? -backwardSpeed : velocity.x;
					body.velocity = velocity;
				}
            }

			animator.SetFloat("velocityScaleX", velocityScale);
			animator.SetFloat("velocityY", body.velocity.y);
			animator.SetBool("inFall", inFall);
			animator.SetBool("inJump", inJump);

			
			if (Math.Abs(body.velocity.y) > maxVelocityY)
				maxVelocityY = Math.Abs(body.velocity.y);
			Debug.Log(maxVelocityY);
			Debug.Log(Math.Abs(body.velocity.y));
		}
	}
}