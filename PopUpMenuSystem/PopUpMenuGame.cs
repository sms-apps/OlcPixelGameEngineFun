using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenuSystem
{
    public class PopUpMenuGame : Game
    {
        private static Sprite MainSprite { get; set; }
        private static MenuObject Menu { get; set; }
        private static MenuManager MenuManager { get; set; }
        private static string LastAction { get; set; }

        private static void Main(string[] args)
        {
            PopUpMenuGame game = new PopUpMenuGame();
            Menu = new MenuObject("main");
            MenuManager = new MenuManager();
            game.Construct(400, 240, 3, 3);
            game.Start();
        }

        public override void OnCreate()
        {
            MainSprite = Sprite.Load("./RetroMenu.png");

            Menu.SetTable(1, 4);

            Menu["Attack"].SetId(100);

            Menu["Magic"].SetTable(1, 2);
            Menu["Magic"]["White"].SetTable(3, 6);
            Menu["Magic"]["White"]["Cure"].SetId(401);
            Menu["Magic"]["White"]["Cura"].SetId(402);
            Menu["Magic"]["White"]["Curaga"].SetId(403);
            Menu["Magic"]["White"]["Esuna"].SetId(404);

            Menu["Magic"]["Black"].SetTable(3, 4);
            Menu["Magic"]["Black"]["Fire"].SetId(201);
            Menu["Magic"]["Black"]["Fira"].SetId(202);
            Menu["Magic"]["Black"]["Firaga"].SetId(203);
            Menu["Magic"]["Black"]["Blizzard"].SetId(204);
            Menu["Magic"]["Black"]["Blizzara"].SetId(205).Enable(true);
            Menu["Magic"]["Black"]["Blizzaga"].SetId(206).Enable(true);
            Menu["Magic"]["Black"]["Thunder"].SetId(207);
            Menu["Magic"]["Black"]["Thundara"].SetId(208);
            Menu["Magic"]["Black"]["Thundaga"].SetId(209);
            Menu["Magic"]["Black"]["Quake"].SetId(210);
            Menu["Magic"]["Black"]["Quake2"].SetId(211);
            Menu["Magic"]["Black"]["Quake3"].SetId(212);
            Menu["Magic"]["Black"]["Bio"].SetId(213);
            Menu["Magic"]["Black"]["Bio1"].SetId(214);
            Menu["Magic"]["Black"]["Bio2"].SetId(215);
            Menu["Magic"]["Black"]["Demi"].SetId(216);
            Menu["Magic"]["Black"]["Demi1"].SetId(217);
            Menu["Magic"]["Black"]["Demi2"].SetId(218);

            Menu["Defend"].SetId(102);

            Menu["Items"].SetTable(2, 4).Enable(true);
            Menu["Items"]["Potion"].SetId(301);
            Menu["Items"]["Ether"].SetId(302);
            Menu["Items"]["Elixir"].SetId(303);

            Menu["Escape"].SetId(103);

            Menu.Build();
            MenuManager.Open(Menu);

            base.OnCreate();
        }

        public override void OnUpdate(float delta)
        {
            MenuObject command = null;

            if (GetKey(Key.M).Pressed)
            {
                if (MenuManager.IsOpen) MenuManager.Close();
                else MenuManager.Open(Menu);
            }

            if (GetKey(Key.Up).Pressed) MenuManager.OnUp();
            if (GetKey(Key.Down).Pressed) MenuManager.OnDown();
            if (GetKey(Key.Left).Pressed) MenuManager.OnLeft();
            if (GetKey(Key.Right).Pressed) MenuManager.OnRight();
            if (GetKey(Key.Space).Pressed) command = MenuManager.OnConfirm();

            if (command != null)
            {
                LastAction = $"Selected: {command.Name} ID: {command.Id}";
                MenuManager.Close();
            }

            Clear(Pixel.Presets.Black);
            MenuManager.Draw(this, MainSprite, new Vector2Int(30, 30));
            DrawText(new Point(10, 200), LastAction, Pixel.Presets.White);
            base.OnUpdate(delta);
        }
    }
}