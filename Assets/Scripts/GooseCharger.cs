using UnityEngine;

public class GooseCharger : EnemyBase
{
    enum State { Idle, WindUp, Dash, Stunned }

    [SerializeField] GooseStats stats;
    [SerializeField] AudioClip hissClip;

    State state;
    int dashDir;

    public override EnemyStats Stats => stats;

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
                if (DistanceToPlayer() < stats.viewRange) Face(PlayerDX());
                if (stateTimer > stats.noticeDelay && seesPlayer && IsOnScreen()) SetState(State.WindUp);
                break;

            case State.WindUp:
                MoveX(0f);
                if (stateTimer >= stats.windUp) SetState(State.Dash);
                break;

            case State.Dash:
                MoveX(dashDir * stats.dashSpeed);
                if (stateTimer >= stats.dashTime || !CanWalk(dashDir)) SetState(State.Stunned);
                break;

            case State.Stunned:
                MoveX(0f);
                if (stateTimer >= stats.cooldown) SetState(State.Idle);
                break;
        }
    }

    void SetState(State next)
    {
        state = next;
        EnterState(next.ToString(), (int)next, next == State.WindUp);

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
            playerHealth.TakeDamage(stats.damage);
            SetState(State.Stunned);
        }
    }
}
