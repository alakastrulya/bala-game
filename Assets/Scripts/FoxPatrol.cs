using UnityEngine;

public class FoxPatrol : EnemyBase
{
    enum State { Patrol, Chase, WindUp, Recover }

    [SerializeField] Transform[] points;
    [SerializeField] float patrolSpeed = 1.7f;
    [SerializeField] float chaseSpeed = 4f;
    [SerializeField] float loseTime = 2f;
    [SerializeField] float attackRange = 1.2f;
    [SerializeField] float windUp = 0.5f;
    [SerializeField] float recoverTime = 0.6f;
    [SerializeField] int damage = 1;
    [SerializeField] AudioClip growlClip;

    State state;
    int pointIndex;
    float lostTimer;

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
                if (seesPlayer) SetState(State.Chase);
                break;

            case State.Chase:
                if (seesPlayer) lostTimer = 0f;
                else lostTimer += Time.deltaTime;

                Chase();

                if (DistanceToPlayer() < attackRange && IsOnScreen()) SetState(State.WindUp);
                else if (lostTimer > loseTime) SetState(State.Patrol);
                break;

            case State.WindUp:
                MoveX(0f);
                if (stateTimer >= windUp)
                {
                    Bite();
                    SetState(State.Recover);
                }
                break;

            case State.Recover:
                MoveX(0f);
                if (stateTimer >= recoverTime) SetState(State.Chase);
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
        MoveX(facing * patrolSpeed);
    }

    void Chase()
    {
        Face(PlayerDX());

        if (CanWalk(facing)) MoveX(facing * chaseSpeed);
        else MoveX(0f);
    }

    void Bite()
    {
        if (DistanceToPlayer() <= attackRange) playerHealth.TakeDamage(damage);
    }
}
