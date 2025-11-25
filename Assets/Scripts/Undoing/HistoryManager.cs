using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HistoryManager : MonoBehaviour
{
	public static HistoryManager master;

	void Awake()
	{
		if(master == null) master = this;
		else if(master != this) Destroy(gameObject);
	}

	public GameState CaptureState()
	{
		var state = new GameState();
		state.playerPos = GameManager.master.Player.Position;
		state.playerRecharge = GameManager.master.Player.RechargeAmount;

		state.boxPositions = new List<Vector2Int>();
		foreach (Box box in GameManager.master.CurrentLevel.Boxes.Values)
			state.boxPositions.Add(box.Position);

		state.ghosts = new List<GhostState>();
		foreach (Ghost ghost in GameManager.master.CurrentLevel.Ghosts.Values)
		{
			state.ghosts.Add(new GhostState {
				position = ghost.Position,
				ticksUntilNextWave = ghost.currentSteps
			});
		}
		

		state.doors = new List<DoorState>();
		foreach (Door door in GameManager.master.CurrentLevel.Doors.Values)
		{
			state.doors.Add(new DoorState {
				position = door.Position,
				isOpen = door.Open
			});
		}

		state.emitters = new List<EmitterState>();
		foreach (Emitter emitter in GameManager.master.CurrentLevel.Emitters.Values)
		{
			state.emitters.Add(new EmitterState {
				position = emitter.Position,
				ticksUntilNextWave = emitter.currentSteps,
				isActive = emitter.IsActive
			});
		}

		state.buttons = new List<ButtonState>();
		foreach (InGameButton button in GameManager.master.CurrentLevel.Buttons.Values)
		{
			state.buttons.Add(new ButtonState {
				position = button.Position,
				isActive = button.IsActivated
			});
		}

		state.receivers = new List<ReceiverState>();
		foreach (Receiver receiver in GameManager.master.CurrentLevel.Receivers.Values)
		{
			state.receivers.Add(new ReceiverState {
				position = receiver.Position,
				isActive = receiver.IsActivated
			});
		}

		state.waves = new List<WaveState>();
		foreach (Wave wave in GameManager.master.CurrentLevel.Waves)
		{
			WaveState waveState = new WaveState();
			waveState.center = wave.Center;
			waveState.elements = new List<WaveElementState>();

			foreach (WaveElement elem in wave.Elements.Values)
			{
				WaveElementState newElement = new WaveElementState
				{
					position = elem.Position,
					strength = elem.Strength
				};
				newElement.flowDirections = new List<MapDirection>();
				foreach(MapDirection direction in elem.DirectionsToFlow)
					newElement.flowDirections.Add(direction);
			
				waveState.elements.Add(newElement);
			}

			state.waves.Add(waveState);
		}

		return state;
	}

	public void RestoreState(GameState state)
	{
		if (state == null) return;
		GameManager.master.CurrentLevel.ClearMost();

		GameManager.master.Player.Position = state.playerPos;
		GameManager.master.Player.RechargeAmount = state.playerRecharge;

		for (int i = 0; i < state.boxPositions.Count; i++)
		{
			Box newBox = new Box();
			newBox.Position = state.boxPositions[i];
			GameManager.master.CurrentLevel.Boxes.Add(
				state.boxPositions[i], 
				newBox);
		}

		for (int i = 0; i < state.ghosts.Count; i++)
		{
			Ghost newGhost = new Ghost(
				state.ghosts[i].position, 
				state.ghosts[i].ticksUntilNextWave);
			GameManager.master.CurrentLevel.Ghosts.Add(
				state.ghosts[i].position,
				newGhost);
		}

		for (int i = 0; i < state.doors.Count; i++)
		{
			Door newDoor = GameManager.master.CurrentLevel.Doors[state.doors[i].position];
			newDoor.Open = state.doors[i].isOpen;
		}

		for (int i = 0; i < state.emitters.Count; i++)
		{
			Emitter newEmitter = GameManager.master.CurrentLevel.Emitters[state.emitters[i].position];
			newEmitter.IsActive = state.emitters[i].isActive;
			newEmitter.currentSteps = state.emitters[i].ticksUntilNextWave;
		}

		for (int i = 0; i < state.receivers.Count; i++)
		{
			Receiver newReceiver = GameManager.master.CurrentLevel.Receivers[state.receivers[i].position];
			newReceiver.IsActivated = state.receivers[i].isActive;
		}

		for (int i = 0; i < state.buttons.Count; i++)
		{
			InGameButton newButton = GameManager.master.CurrentLevel.Buttons[state.buttons[i].position];
			newButton.IsActivated = state.buttons[i].isActive;
		}

		/* Waves */
		foreach (WaveState waveState in state.waves)
		{
			Wave newWave = new Wave(waveState.center);

			newWave.Elements = new Dictionary<Vector2Int,WaveElement>();
			foreach (WaveElementState elemState in waveState.elements)
			{
				WaveElement newElement = new WaveElement(elemState.position,  elemState.strength);
				newElement.DirectionsToFlow = new HashSet<MapDirection>();
				foreach(MapDirection direction in elemState.flowDirections)
					newElement.DirectionsToFlow.Add(direction);
				newWave.Elements.Add(elemState.position,newElement);
			}
			GameManager.master.CurrentLevel.Waves.Add(newWave);
		}
		GameManager.master.CurrentLevel.RecalculateScores();
		GameManager.master.Map.Display();
		GameManager.master.CurrentLevel.Refresh();
	}
}