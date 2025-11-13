using UnityEngine;

public class CollisionSenes : CoreComponent {

    public Transform GroundCheck {
        get => GenericNotImplementedError<Transform>.TryGet(groundCheck, core.transform.parent.name);
        private set => groundCheck = value;
    }
    public Transform CeilingCheck {
        get => GenericNotImplementedError<Transform>.TryGet(ceilingCheck, core.transform.parent.name);
        private set => ceilingCheck = value;
    }
    public float GroundCheckRadius { get => groundCheckRadius; set => groundCheckRadius = value; }
    public LayerMask WhatIsGround { get => whatIsGround; set => whatIsGround = value; }

    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform ceilingCheck;

    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask whatIsGround;

    public bool Ceiling {
        get => Physics2D.OverlapCircle(CeilingCheck.position, groundCheckRadius, whatIsGround);
    }

    public bool Ground {
        get => Physics2D.OverlapCircle(GroundCheck.position, groundCheckRadius, whatIsGround);
    }

}
