# Week 10 - Three Types of Enemies
2026-11-10  Asked an agent to help me resolve issues with `EnemyBase`.
The agent created a "doctor-shooter" character; I replaced it with a field mouse carrying seeds, fitting the steppe theme.
2026-11-11  The fox would chase the player but wouldn't bite—the attack range of 1.2 was smaller than the size of the colliders. I increased the attack range to 2 and bumped the fox's mass from 1 to 20 so the player couldn't push it around.
2026-11-11  The mouse couldn't see me when I was to its left; in its "Idle" state, it was always facing right. I added a rotation mechanic that triggers every 2 seconds (`lookTime`).
2026-11-12  The fox was running in place, and an error kept appearing: the ground-check rays were configured for the old sprite, and the collider was getting stuck at the seams between ground segments. I updated the rays to match the collider's dimensions, switched from a `Box Collider` to a `Capsule Collider`, and applied a frictionless physics material.
# Playtest
