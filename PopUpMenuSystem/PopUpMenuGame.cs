using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenu
{
    public class PopUpMenuGame : Game
    {
        private const int Patch = 8;
        private static Sprite Gfx { get; set; }
        private static MenuObject MainMenu { get; set; }
        private static string Name { get; set; }
        private static bool HasDrawnOnce { get; set; }

        public PopUpMenuGame()
        {
            Name = "Default";
        }

        public PopUpMenuGame(string name)
        {
            Name = name;
        }

        private static void Main(string[] args)
        {
            PopUpMenuGame game = new PopUpMenuGame("Not Default!");
            HasDrawnOnce = false;
            MainMenu = new MenuObject("main");
            game.Construct(400, 200, 4, 4);
            game.Start();
        }

        public override void OnCreate()
        {
            Gfx = Sprite.Load("./RetroMenu.png");

            DrawText(new Point(1, 1), "Testing", Pixel.Presets.Apricot);

            MainMenu.SetTable(1, 5);
            MainMenu["Attack"].SetId(101);
            MainMenu["Magic"].SetId(102);
            MainMenu["Defend"].SetId(103);
            MainMenu["Items"].SetId(104);
            MainMenu["Escape"].SetId(105);

            MainMenu.Build();
            MainMenu.DrawSelf(this, Gfx, new Vector2Int(10, 10));

            base.OnCreate();
        }

        public override void OnUpdate(float delta)
        {
            Clear(Pixel.Presets.Black);
            MainMenu.DrawSelf(this, Gfx, new Vector2Int(10, 10));
            HasDrawnOnce = true;

            base.OnUpdate(delta);
        }
    }
}