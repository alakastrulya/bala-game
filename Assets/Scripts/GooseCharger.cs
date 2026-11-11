using UnityEngine;

public class GooseCharger : EnemyBase
{
    enum State { Idle, WindUp, Dash, Stunned }

    [SerializeField] float windUp = 0.7f;
    [SerializeField] float dashSpeed = 9f;
    [SerializeField] float dashTime = 0.8f;
    [SerializeField] float stunTime = 1f;
    [SerializeField] int damage = 1;
    [SerializeField] AudioClip hissClip;

    State state;
    int dashDir;

    void Start()
    {
        SetState(State.Idle);
    }

    protected override void Think()
    {
        switch (state)
        {
            case State.Idle:
                MoveX(0f);
                if (DistanceToPlayer() < viewRange) Face(PlayerDX());
                if (stateTimer > 0.5f && seesPlayer && IsOnScreen()) SetState(State.WindUp);
                break;

            case State.WindUp:
                MoveX(0f);
                if (stateTimer >= windUp) SetState(State.Dash);
                break;

            case State.Dash:
                MoveX(dashDir * dashSpeed);
                if (stateTimer >= dashTime || !CanWalk(dashDir)) SetState(State.Stunned);
                break;

            case State.Stunned:
                MoveX(0f);
                if (stateTimer >= stunTime) SetState(State.Idle);
                break;
        }
    }

    void SetState(State next)
    {
        state = next;
        EnterState(next.ToString(), next == State.WindUp);

        if (next == State.WindUp)
        {
            dashDir = facing;
            PlaySound(hissClip);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (state == State.Dash && other.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(damage);
            SetState(State.Stunned);
        }
    }
}