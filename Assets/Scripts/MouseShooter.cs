using UnityEngine;

public class MouseShooter : EnemyBase
{
    enum State { Idle, KeepDistance, Aim, Cooldown }

    [SerializeField] float lookTime = 2f;
    [SerializeField] float preferredDistance = 5f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float aimTime = 0.6f;
    [SerializeField] float shotSpeed = 6f;
    [SerializeField] float cooldown = 1.5f;
    [SerializeField] Seed seedPrefab;
    [SerializeField] LineRenderer aimLine;
    [SerializeField] AudioClip aimClip;

    State state;
    Vector2 aimDir;

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
                if (stateTimer > lookTime)
                {
                    Face(-facing);
                    stateTimer = 0f;
                }
                if (seesPlayer) SetState(State.KeepDistance);
                break;

            case State.KeepDistance:
                KeepDistance();
                if (stateTimer > 1f && seesPlayer && IsOnScreen()) SetState(State.Aim);
                else if (stateTimer > 3f) SetState(State.Idle);
                break;

            case State.Aim:
                MoveX(0f);
                aimDir = (player.position - transform.position).normalized;
                Face(aimDir.x);
                if (aimLine != null)
                {
                    aimLine.SetPosition(0, transform.position);
                    aimLine.SetPosition(1, (Vector2)transform.position + aimDir * 8f);
                }

                if (stateTimer >= aimTime)
                {
                    Throw();
                    SetState(State.Cooldown);
                }
                break;

            case State.Cooldown:
                MoveX(0f);
                if (stateTimer >= cooldown) SetState(State.KeepDistance);
                break;
        }
    }

    void SetState(State next)
    {
        state = next;
        EnterState(next.ToString(), (int)next, next == State.Aim);
        if (aimLine != null) aimLine.enabled = next == State.Aim;
        if (next == State.Aim) PlaySound(aimClip);
    }

    void KeepDistance()
    {
        float dx = PlayerDX();
        Face(dx);

        int dir = 0;
        if (Mathf.Abs(dx) < preferredDistance - 1f) dir = -facing;
        if (Mathf.Abs(dx) > preferredDistance + 1f) dir = facing;

        if (dir != 0 && CanWalk(dir)) MoveX(dir * moveSpeed);
        else MoveX(0f);
    }

    void Throw()
    {
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        Vector2 spawnPos = (Vector2)transform.position + aimDir * 0.8f;

        Seed s = Instantiate(seedPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        s.speed = shotSpeed;
    }
}
