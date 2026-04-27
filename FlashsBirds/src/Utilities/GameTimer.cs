namespace FlashsBirds.Utilities;

public static class GameTimer {
	private struct TimedAction(Action action, float delayMs, bool isRepeating = false, float repeatIntervalMs = 0f, int id = 0) {
		public readonly Action Action = action;
		public float DelayMs = delayMs;
		public float ElapsedMs = 0f;
		public readonly bool IsRepeating = isRepeating;
		public readonly float RepeatIntervalMs = repeatIntervalMs;
		public readonly int Id = id;
	}
	
	private static readonly List<TimedAction> _timedActions = [];
	private static readonly List<int> _actionsToRemove = [];
	private static int _nextId = 1;
	public static float ElapsedTime = 0f;
	
	public static int AddDelayedAction(Action action, float delayMs) {
		TimedAction timedAction = new(action, delayMs, false, 0f, _nextId);
		_timedActions.Add(timedAction);
		return _nextId++;
	}
	
	public static int AddRepeatingAction(Action action, float initialDelayMs, float repeatIntervalMs) {
		TimedAction timedAction = new(action, initialDelayMs, true, repeatIntervalMs, _nextId);
		_timedActions.Add(timedAction);
		return _nextId++;
	}
	
	public static bool CancelAction(int actionId) {
		for (int i = 0; i < _timedActions.Count; i++) {
			if (_timedActions[i].Id == actionId) {
				_actionsToRemove.Add(i);
				return true;
			}
		}

		return false;
	}
	
	public static void Update(float deltaTimeMs) {
		ElapsedTime += deltaTimeMs;
		
		for (int i = 0; i < _timedActions.Count; i++) {
			TimedAction action = _timedActions[i];
			action.ElapsedMs += deltaTimeMs;
			
			if (action.ElapsedMs >= action.DelayMs) {
				try {
					action.Action?.Invoke();
				} catch (Exception ex) {
					Console.WriteLine($"Timer action exception: {ex.Message}");
				}

				if (action.IsRepeating) {
					action.ElapsedMs = 0f;
					action.DelayMs = action.RepeatIntervalMs;
					_timedActions[i] = action;
				} else {
					_actionsToRemove.Add(i);
				}
			} else {
				_timedActions[i] = action;
			}
		}
		
		for (int i = _actionsToRemove.Count - 1; i >= 0; i--) {
			_timedActions.RemoveAt(_actionsToRemove[i]);
		}

		_actionsToRemove.Clear();
	}
	
	public static void Clear() {
		_timedActions.Clear();
		_actionsToRemove.Clear();
	}
}