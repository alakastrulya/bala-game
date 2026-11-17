using UnityEngine;

public class FoxPatrol : EnemyBase
{
    enum State { Patrol, Chase, WindUp, Recover }

    [SerializeField] FoxStats stats;
    [SerializeField] Transform[] points;
    [SerializeField] AudioClip growlClip;

    State state;
    int pointIndex;
    float lostTimer;

    public override EnemyStats Stats => stats;

    void Start()
    {
        SetState(State.Patrol);
    }

    protected override void Think()
    {
        switch (state)
        {
            case State.Patrol:
                Patrol();
                if (seesPlayer && stateTimer > stats.noticeDelay) SetState(State.Chase);
                break;

            case State.Chase:
                bool moving = Chase();
                if (seesPlayer && moving) lostTimer = 0f;
                else lostTimer += Time.deltaTime;

                if (DistanceToPlayer() < stats.attackRange && IsOnScreen()) SetState(State.WindUp);
                else if (lostTimer > stats.loseTime) SetState(State.Patrol);
                break;

            case State.WindUp:
                MoveX(0f);
                if (stateTimer >= stats.windUp)
                {
                    Bite();
                    SetState(State.Recover);
                }
                break;

            case State.Recover:
                MoveX(0f);
                if (stateTimer >= stats.cooldown) SetState(State.Chase);
                break;
        }
    }

    void SetState(State next)
    {
        state = next;
        EnterState(next.ToString(), (int)next, next == State.WindUp);
        lostTimer = 0f;
        if (next == State.WindUp) PlaySound(growlClip);
    }

    void Patrol()
    {
        float dx = points[pointIndex].position.x - transform.position.x;

        if (Mathf.Abs(dx) < 0.1f)
        {
            pointIndex = (pointIndex + 1) % points.Length;
            return;
        }

        Face(dx);
        MoveX(facing * stats.patrolSpeed);
    }

    bool Chase()
    {
        Face(PlayerDX());

        if (CanWalk(facing))
        {
            MoveX(facing * stats.chaseSpeed);
            return true;
        }

        MoveX(0f);
        return DistanceToPlayer() < stats.attackRange;
    }

    void Bite()
    {
        if (DistanceToPlayer() <= stats.attackRange) playerHealth.TakeDamage(stats.damage);
    }
}