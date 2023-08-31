using System.Collections.Generic;

namespace DrawLine {
    public interface IControllerEvent : GameEngine.IControllerEvent<Level, IControllerEvent> {
        void OnDrawTile(DrawLineController controller, Tile tile, ColorIndex color);
        void OnEraseTile(DrawLineController controller, Tile tile, ColorIndex originColor);
        void OnClearColor(List<Tile> tilesInLine);
    }
}