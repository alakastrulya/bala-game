using UnityEngine;

public class MouseShooter : EnemyBase
{
    enum State { Idle, KeepDistance, Aim, Cooldown }

    const float AimLineLength = 8f;
    const float ThrowOffset = 0.8f;

    [SerializeField] MouseStats stats;
    [SerializeField] Seed seedPrefab;
    [SerializeField] LineRenderer aimLine;

    State state;
    Vector2 aimDir;

    public override EnemyStats Stats => stats;

    void Start()
    {
        if (aimLine != null) aimLine.positionCount = 2;
        SetState(State.Idle);
    }

    protected override void Think()
    {
        switch (state)
        {
            case State.Idle:
                MoveX(0f);
                if (stateTimer > stats.lookTime)
                {
                    Face(-facing);
                    stateTimer = 0f;
                }
                if (seesPlayer) SetState(State.KeepDistance);
                break;

            case State.KeepDistance:
                KeepDistance();
                if (stateTimer > stats.noticeDelay && seesPlayer && IsOnScreen()) SetState(State.Aim);
                else if (stateTimer > stats.giveUpTime) SetState(State.Idle);
                break;

            case State.Aim:
                MoveX(0f);
                aimDir = (player.position - transform.position).normalized;
                Face(aimDir.x);
                if (aimLine != null)
                {
                    aimLine.SetPosition(0, transform.position);
                    aimLine.SetPosition(1, (Vector2)transform.position + aimDir * AimLineLength);
                }

                if (stateTimer >= stats.windUp)
                {
                    Throw();
                    SetState(State.Cooldown);
                }
                break;

            case State.Cooldown:
                MoveX(0f);
                if (stateTimer >= stats.cooldown) SetState(State.KeepDistance);
                break;
        }
    }

    void SetState(State next)
    {
        state = next;
        EnterState(next.ToString(), (int)next, next == State.Aim);
        if (aimLine != null) aimLine.enabled = next == State.Aim;
        if (next == State.Aim) PlaySound(Sfx.MouseWindUp);
    }

    void KeepDistance()
    {
        float dx = PlayerDX();
        Face(dx);

        int dir = 0;
        if (Mathf.Abs(dx) < stats.preferredDistance - 1f) dir = -facing;
        if (Mathf.Abs(dx) > stats.preferredDistance + 1f) dir = facing;

        if (dir != 0 && CanWalk(dir)) MoveX(dir * stats.moveSpeed);
        else MoveX(0f);
    }

    void Throw()
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Vector2 spawnPos = (Vector2)transform.position + aimDir * ThrowOffset;

        Seed s = Instantiate(seedPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        s.speed = stats.shotSpeed;
        s.damage = stats.damage;
    }
}
