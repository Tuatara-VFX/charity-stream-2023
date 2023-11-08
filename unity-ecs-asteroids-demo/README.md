# Asteroids

## Description

This is an ECS Asteroids demo that shows how making a simple custom particle system can serve as a reusable core game system.
It relies mostly on `SystemBase`, `Entities.ForEach` and `EntityCommandBuffer` to avoid deeper topics during the live stream and to make it easier to understand.

## Systems

- Particle Spawner
- Initial Velocity
- Lifetime
- Asteroids Spaceship
- Destruction On Contact