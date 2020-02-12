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

            DrawText(new Point(1, 1), "Testing", Pixel.Presets.Apricot);

            Menu.SetTable(3, 3);
            Menu["Attack"].SetId(101);
            Menu["Magic"].SetId(102).SetTable(2, 4);
            Menu["Magic"]["Fire"].SetId(10201);
            Menu["Magic"]["Force"].SetId(10202);
            Menu["Magic"]["Ice"].SetId(10203);
            Menu["Magic"]["Necrotic"].SetId(10204);
            Menu["Magic"]["Force"].SetId(10205);
            Menu["Magic"]["Smeg"].SetId(10206);
            Menu["Magic"]["Ma"].SetId(10207);

            Menu["Defend"].SetId(103);
            Menu["Items"].SetId(104);
            Menu["Escape"].SetId(105);

            Menu["Dummy1"].SetId(151);
            Menu["Dummy2"].SetId(152);
            Menu["Dummy3"].SetId(153);
            Menu["Dummy4"].SetId(154).Enable(false);
            Menu["Dummy5"].SetId(155).Enable(false);
            Menu["Dummy6"].SetId(156).Enable(false);
            Menu["Dummy7"].SetId(157);
            Menu["Dummy8"].SetId(158);
            Menu["Dummy9"].SetId(159);

            Menu.Build();
            Menu.DrawSelf(this, MainSprite, new Vector2Int(10, 10));

            base.OnCreate();
        }

        public override void OnUpdate(float delta)
        {
            Clear(Pixel.Presets.Black);

            if (GetKey(Key.M).Pressed)
            {
                if (MenuManager.IsOpen) MenuManager.Close();
                else MenuManager.Open(Menu);
            }

            if (GetKey(Key.Up).Pressed) MenuManager.OnUp();
            if (GetKey(Key.Down).Pressed) MenuManager.OnDown();
            if (GetKey(Key.Left).Pressed) MenuManager.OnLeft();
            if (GetKey(Key.Right).Pressed) MenuManager.OnRight();

            MenuObject command = null;
            if (GetKey(Key.Space).Pressed) command = MenuManager.OnConfirm();

            if (command != null)
            {
                LastAction = $"Selected: {command.Name} ID: {command.Id}";
                MenuManager.Close();
            }

            MenuManager.Draw(this, MainSprite, new Vector2Int(30, 30));
            DrawText(new Point(10, 200), LastAction, Pixel.Presets.White);

            base.OnUpdate(delta);
        }
    }
}