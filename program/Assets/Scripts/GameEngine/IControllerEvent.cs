namespace GameEngine {
    public interface IControllerEvent<TCtrl, TLevel> 
            where TCtrl : Controller<TLevel, IControllerEvent> 
            where TLevel : class  {
        void OnStartGame(TCtrl controller);
        void OnReplayGame(TCtrl controller);
        void OnClearGame(TCtrl controller);
        void OnFailGame(TCtrl controller);
        void OnContinueGame(TCtrl controller);
        void OnStopGame(TCtrl controller, GameStopReason gameStopReason);
    }
}