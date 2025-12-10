# The Internal  
*Tiny Doctor, Big Dreams*

In a world torn apart by an apocalypse, one exhausted doctor refuses to give up.  
When infected survivors arrive at his hospital, he does the unthinkable: shrinks himself down in a tiny medical mech suit and dives inside their bodies to fight the infection from within.

---

## Story & Premise

The apocalypse has been raging for years. The doctor has been saving whoever he can, but things are getting worse.

At the start of each run, you’re introduced to **three patients**:
- One with a **zombie infection**
- One with a **werewolf infection**
- One with a **vampire infection**

You listen to their stories, then choose who to treat.  
The doctor shrinks down, jumps into his medical pod, and **dives inside the patient** to battle the infection directly.

How many of them you save (0–3) affects the doctor’s **diary entry** and the **ending** you get:
- **3/3 saved** – hopeful, “we are turning things around”
- **2/3 saved** – bittersweet, still trying
- **1/3 saved** – losing hope, but continuing
- **0/3 saved** – the doctor can’t go on anymore…

---

## Gameplay Overview

The Internal is split into multiple stages and scenes:

### 🏥 Scene 1 – Main Menu
- Start game  
- Controls info  
- Quit

### 🧬 Scene 2 – Patient Selection
- Short introduction to the day’s situation  
- See **three patients** with different infections  
- Hover / select a patient to see more details and symptoms  
- Choose one patient to treat → continue to gameplay

### 🩸 Scene 3 – Obstacle Gameplay (Inside the Body)
- You are inside a **vein** in the patient’s body  
- The **InfectionController** spawns enemies (tiny infections)
- Enemies move towards the player along a “conveyor belt” style path
- You **dodge up and down** and **shoot medicine bullets**
- If enemies hit you, you lose health
- Survive the obstacle course to reach the **core infection** → boss fight

### 💀 Scene 4 – Boss Battle
Each infection type has its own boss concept:

- **Zombie Boss**
  - Shoots vine projectiles
  - Big bite attacks
  - Vine attacks from above
  
- **Werewolf Boss**
  - Shooting claw attacks
  - Boomerang attack
  - Fast, aggressive movement

- **Vampire Boss**
  - Summons bats (who attack)
  - Wind knowckback and damage
  
Mechanically:
- Boss has **health**
- When boss health ≤ 0 → patient is **saved**
- If player dies in boss fight → patient is **lost**


### 🧑‍⚕️ Scene 5 – Patient Result
- **Saved patient**
  - Patient thanks the doctor  
  - Sent to out of the hospital lobby  
  - Counted towards total saved patients  
- **Killed patient**
  - The doctor fails and must end the patient’s life  
  - Counted as not saved  


### 📓 Scene 6 – Ending Screen
Based on `savedPatientCount`, you see a different **Diary Entry 748**:

- **3/3 saved – Good Ending**  
  “We’re turning things around. I’m hopeful for the future.”

- **2/3 saved – Partial Good Ending**  
  “I wish I could’ve saved the third, but I’ll keep trying.”

- **1/3 saved – Struggling Ending**  
  “It’s hard to stay motivated, but I’ll continue.”

- **0/3 saved – Bad Ending**  
  “I couldn’t save anyone today. I can’t go on like this.”  
  *(The doctor’s story ends here.)*

After the ending, you’re taken to **Scene 7 Credits**.

---

## 🕹 Controls

| Input       | Action                      |
|------------|-----------------------------|
| `WASD`     | Move your medical pod       |
| Arrow Keys | Move your medical pod       |
| `Space`    | Shoot medicine bullets      |
