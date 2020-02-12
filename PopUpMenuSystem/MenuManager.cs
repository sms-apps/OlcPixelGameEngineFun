using System.Collections.Generic;
using System.Linq;
using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenuSystem
{
    public class MenuManager
    {
        public bool IsOpen => Panels.Any();

        private Queue<MenuObject> Panels { get; set; } = new Queue<MenuObject>();

        public MenuManager()
        {
        }

        public void Close() => Panels.Clear();

        public MenuObject OnConfirm()
        {
            if (!IsOpen) return null;
            var next = Panels.Last().OnConfirm();

            if (next == Panels.Last())
            {
                if (Panels.Last().GetSelectedItem().Enabled)
                {
                    return Panels.Last().GetSelectedItem();
                }
            }
            else
            {
                if (next.Enabled)
                {
                    Panels.Enqueue(next);
                }
            }

            return null;
        }

        public void Draw(Game game, Sprite sprite, Vector2Int screenOffset)
        {
            if (!Panels.Any()) return;

            // Draw visible menu system.
            foreach (var panel in Panels)
            {
                panel.DrawSelf(game, sprite, screenOffset);
                screenOffset = new Vector2Int(screenOffset.x + 10, screenOffset.y + 10);
            }

            // Draw the cursor.
            var currentPixelMode = game.PixelMode;
            game.PixelMode = Pixel.Mode.Alpha;
            game.DrawPartialSprite(Panels.Last().CursorPos, sprite, new Vector2Int(4, 0) * MenuObject.Patch, MenuObject.Patch * 2, MenuObject.Patch * 2);
            game.PixelMode = currentPixelMode;
        }

        public void OnUp()
        {
            if (Panels.Any()) Panels.Last().OnUp();
        }

        public void OnDown()
        {
            if (Panels.Any()) Panels.Last().OnDown();
        }

        public void OnLeft()
        {
            if (Panels.Any()) Panels.Last().OnLeft();
        }

        public void OnRight()
        {
            if (Panels.Any()) Panels.Last().OnRight();
        }

        public void OnBack()
        {
            if (Panels.Any()) Panels.Dequeue();
        }

        public void Open(MenuObject menuObject)
        {
            Close();
            Panels.Enqueue(menuObject);
        }
    }
}