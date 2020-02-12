using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenuSystem
{
    public class PopUpMenuGame : Game
    {
        private const int Patch = 8;
        private static Sprite Gfx { get; set; }
        private static MenuObject MainMenu { get; set; }
        private static MenuManager MenuManager { get; set; }
        private static string Name { get; set; }

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
            MainMenu = new MenuObject("main");
            MenuManager = new MenuManager();
            game.Construct(400, 200, 4, 4);
            game.Start();
        }

        public override void OnCreate()
        {
            Gfx = Sprite.Load("./RetroMenu.png");

            DrawText(new Point(1, 1), "Testing", Pixel.Presets.Apricot);

            MainMenu.SetTable(3, 3);
            MainMenu["Attack"].SetId(101);
            MainMenu["Magic"].SetId(102);
            MainMenu["Defend"].SetId(103);
            MainMenu["Items"].SetId(104);
            MainMenu["Escape"].SetId(105);

            MainMenu["Dummy1"].SetId(201);
            MainMenu["Dummy2"].SetId(202);
            MainMenu["Dummy3"].SetId(203);
            MainMenu["Dummy4"].SetId(204);
            MainMenu["Dummy5"].SetId(205);
            MainMenu["Dummy6"].SetId(206);
            MainMenu["Dummy7"].SetId(207);
            MainMenu["Dummy8"].SetId(208);
            MainMenu["Dummy9"].SetId(209);

            MainMenu.Build();
            MainMenu.DrawSelf(this, Gfx, new Vector2Int(10, 10));

            base.OnCreate();
        }

        public override void OnUpdate(float delta)
        {
            Clear(Pixel.Presets.Black);

            if (GetKey(Key.M).Pressed) MenuManager.Open(MainMenu);
            if (GetKey(Key.Up).Pressed) MenuManager.OnUp();
            if (GetKey(Key.Down).Pressed) MenuManager.OnDown();
            if (GetKey(Key.Left).Pressed) MenuManager.OnLeft();
            if (GetKey(Key.Right).Pressed) MenuManager.OnRight();

            MenuManager.Draw(this, Gfx, new Vector2Int(30, 30));
            //MainMenu.DrawSelf(this, Gfx, new Vector2Int(10, 10));

            base.OnUpdate(delta);
        }
    }
}