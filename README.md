# Gam340

This project is an artefact for my Gam340 module and is talked about in greater detail [here](https://www.joshgames.co.uk) on my website

---
In brief, this project is developed with the intention of making a strong, modular AI system which can easily be adapted to create interesting behaviours with little work. It uses sensors for both *sight* and *hearing* to decide what the agent wants to do.

 
| Sensor Type | Description | Summary |
| ----------- | ----------- | ----------- |
| Sight | This generates a vision cone where the agent is facing and is affected by the 'scene' lighting. | When the agent sees an object, it will be checked against its behaviours. |
| Hearing | This will check to see if this agent is within the radius of a given noise. If it is, a curiosity value will be calculated based on a couple factors.  | When the agent hears something, it will be checked against its behaviours based on a "curiosity" value. |

---

Art and Music developed by [Esme Jones](https://linktr.ee/MimiNovalight)