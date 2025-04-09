# Developer Guide 👨‍💻

## CG4002 Computer Engineering Capstone Project

This guide outlines the architecture and logic of the AR Laser Tag Visualiser Unity app. The visualiser displays player actions, effects, and status in AR using real-time data from a wearable system via MQTT. It’s designed to help developers understand the system flow and contribute effectively.

## 🧠 Logic Architecture

The logic in the **AR Laser Tag Visualiser** is centered around 2 primary entities: the **Player** and the **Opponent**, both of which interact with each other and respond to incoming MQTT messages.

### 🔁 Entity Setup
<table>
  <tr>
    <th>Player</th>
    <th>Opponent</th>
  </tr>
  <tr>
    <td>Attached to the <b>ARCamera</b> object.</td>
    <td>Instantiated and attached to a <b>Vuforia image target</b>.</td>
  </tr>
  <tr>
    <td>Actions originate from the mobile device and move with the player in space.</td>
    <td>Requires the real opponent to wear the printed image target on their body.
</td>
  </tr>
  <tr>
    <td>Maintains a reference to the Opponent to apply effects or deal damage.</td>
    <td>Maintains a reference to the Player as well.
</td>
  </tr>
</table>

### 📡 MQTT Data Flow
The `MqttManager` class is responsible for:

- Establishing the MQTT connection with the broker (hosted on Ultra96).
- Subscribing to relevant topics.
- Forwarding messages to the correct player class based on the player number.

Each player must tap a `Connect` button in the UI to initiate the MQTT connection.

#### 📨 Subscribed Topics
| Topic	| Purpose | Format (hex) |
| ----- | ------- | ------------ |
| `viz/trigger`	| Triggers an action execution | `<PLAYER_NO><ACTION_NO>` | 
| `viz/visibility`	|  Informs visibility state |  `<PLAYER_NO><OPPONENT_VISIBILITY>` | 
| `backend/state`	| Overall state updates from backend | 
| `backend/ready`	| Signals readiness of backend or players | *empty payload*

#### 🎯 Action Dispatch
When a message is received on the `viz/trigger` topic:

1. `MqttManager` determines the `PLAYER_NO`.
2. Sends the corresponding `ACTION_NO` to the `Player` or `Opponent`.

This ensures each player sees the actions performed by their opponent, in real-time.

### 🧩 Action Controllers
All 9 in-game actions are managed by dedicated controller classes:

| Controller        | Handles | Description |
| ----------------- | ------- | ----------- |
| GunController	    | Shoot, Reload     | Gun-based actions | 
| ShieldController  | Shield Activation | Auxiliary action |
| BombController    | Snow Bomb | Auxiliary action |
| ActionController	| Boxing, Fencing, Badminton, Golf | Sports-based actions |  

> Logout not implemented (Only used by backend)

### 🗂️ Project Hierarchy
Below is the simplified Unity scene hierarchy that outlines how major GameObjects are structured in the project:
```
📌 ARCamera
 └── 🧍‍♂️ Player
      ├── PlayerController (Script)
      ├── GunController (Script)
      ├── ShieldController (Script)
      ├── BombController (Script)
      └── ActionController (Script)

📌 OpponentImageTarget (Vuforia Image Target)
 └── 🧍 Opponent
      ├── OpponentController (Script)
      ├── GunController (Script)
      ├── ShieldController (Script)
      ├── BombController (Script)
      └── ActionController (Script)

📌 MqttManager (Singleton/Scene Object)
 ├── Handles MQTT connection
 └── Routes messages to Player/Opponent
```

Across the different scripts, we have the follow class diagram to show their relationship with one another:

![](images/VisualiserClassDiagram.png)

### 🧩 Sequence Diagram Explanation – Player

The sequence diagram illustrates how the Player processes actions in response to MQTT messages. 
1. When an action trigger is received from the broker, the MqttManager verifies the topic and player number before forwarding it to the Player. 
2. The Player then calls ProcessAction, which delegates the request to the appropriate controller (e.g., gun, shield, bomb). 
3. Each controller checks if the action is valid (e.g., enough ammo, shield not active) before executing it and calculating damage.
4. The player publishes their visibility status after an action.

- When the player enters a snow zone (OpponentSnow), they take 5 damage, increment their snow stack counter, and publish a message to notify the broker.
- Upon exiting the zone, the snow stack is decremented, and another message is sent to update the player's snow status.

![](images/PlayerSequenceDiagram.png)

### 🧩 Sequence Diagram Explanation – Opponent

This diagram shows how the Opponent processes MQTT messages received from the broker via the MqttManager.

1. When a `viz/trigger` message is received, the opponent stores the action to be displayed later.
2. Upon receiving a `viz/visibility` message, the opponent checks if the player is visible, 
3. Process the previously stored action, adds any snow damage, and applies the total damage to the player if visible.
4. After applying the action, the stored action is reset to avoid repeat execution.

![](images/OpponentSequenceDiagram.png)

> Note that snow state is only sent via the Player object as each device can determine if the camera has entered the opponent's snow effect.

### ⚙️ `ProcessAction` Function Overview

The `ProcessAction(int action, bool isLookingAtOpponent, Vector3 opponentPos)` function in both PlayerController and OpponentController is responsible for orchestrating the execution of a game action. Here's how it works:

1. Action Routing:

Based on the action number, the request is delegated to the corresponding logic controller:

- `GunController` for shooting or reloading
- `ShieldController` for activating shields
- `BombController` for launching snow bombs
- `ActionController` for sports actions (boxing, fencing, badminton & golf)

2. Validity Checks:

Each controller is responsible for verifying that the action can be performed before executing it. Examples include:

- `GunController` checks if bullets are available before shooting.
- `ShieldController` checks if the player has shield units available and no shield is currently active before allowing shield activation.
- `BombController` checks bomb inventory before launching.
- `ActionController` doesn't check anything 

3. Action Execution & Damage Calculation

If the action is valid and `isLookingAtOpponent` is true, the action is performed with projectiles landing at the opponent's position. For snow bomb action, the snow effect will be instantiated at the opponent's position.

4. Return Value:

The function returns the amount of damage dealt to the opponent. 

### 🔄 Backend State Synchronization
The state synchronization logic is triggered when the player receives an MQTT message on the backend/state topic. The message contains the latest values for the player's and opponent's state, and the `SyncPlayerInfo()` and `SyncOpponentInfo()` function  are responsible for updating the in-game UI and logic accordingly.

```csharp
public void SyncPlayerInfo(int health, int bullets, int bomb, int shieldHealth, int deaths, int shieldCount)
```

#### 🎯 Breakdown:
- Health and Shield health: 
Updated via `PlayerInfo.SyncHealthAndShield(health, shieldHealth)`.
- Shield Inventory: 
Passed to `ShieldController.SyncShield(shieldCount, shieldHealth)` to update both shield health and remaining uses.
- Bullets: Sent to `GunController.SyncBullets(bullets)` to reflect ammo count.
- Bomb Count: 
Updated via `BombController.SyncBomb(bomb)`.
- Death Counter: 
Refreshed through `KillDeathSection.UpdateDeathCount(deaths)`.

This function ensures that all player stats are instantly synchronized with backend data, if a wrong action is played. 
