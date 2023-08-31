using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace GameEngine {
    public enum GameResult { None, Clear, Fail, Stop }
    public enum GameStopReason { None, Replay, Leave }
    public abstract class Controller<TLevel, TEvent>
            where TLevel : class 
            where TEvent : IControllerEvent<TLevel, TEvent> {
        
        public Controller() { }
        public Controller(params TEvent[] listeners) {
            Listeners = listeners.ToList();
        }

        public TLevel CurrentLevel { get; private set; }
        public List<TEvent> Listeners { get; } = new List<TEvent>();
        
        protected UniTaskCompletionSource<GameResult> gameCompletionSource;
        public bool OnGoingGame => gameCompletionSource?.Task.Status.IsCompleted() == false;

        public virtual void StartGame(TLevel level) {
            if (OnGoingGame) StopGame(GameStopReason.None);
            CurrentLevel = level;

            StartGameInternal();
            
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();
            foreach (var listener in Listeners) listener.OnStartGame(this);
        }

        protected abstract void StartGameInternal();

        public virtual async UniTask<GameResult> WaitUntilGameEnd() {
            try {
                return await gameCompletionSource.Task;
            } catch (OperationCanceledException) {
                return GameResult.Stop;
            }
        }
        
        public virtual void ReplayGame() {
            StopGame(GameStopReason.Replay);
            StartGameInternal();
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();
            foreach (var listener in Listeners) listener.OnReplayGame(this);
        }

        public virtual void ClearGame() {
            gameCompletionSource.TrySetResult(GameResult.Clear);
            foreach (var listener in Listeners) listener.OnClearGame(this);
        }

        public virtual void FailGame() {
            gameCompletionSource.TrySetResult(GameResult.Fail);
            foreach (var listener in Listeners) listener.OnFailGame(this);
        }

        public virtual void StopGame(GameStopReason stopReason) {
            gameCompletionSource.TrySetResult(GameResult.Stop);
            foreach (var listener in Listeners) listener.OnStopGame(this, stopReason);
        }

        public virtual void OnContinueGame() {
            ContinueInternal();
            gameCompletionSource = new UniTaskCompletionSource<GameResult>();
            foreach (var listener in Listeners) listener.OnContinueGame(this);
        }

        protected virtual void ContinueInternal() { }
    }
}