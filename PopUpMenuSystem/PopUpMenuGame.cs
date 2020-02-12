using PixelEngine;
using PixelEngine.Utilities;

namespace PopUpMenuSystem
{
    public class PopUpMenuGame : Game
    {
        private static Sprite MainSprite { get; set; }
        private static MenuObject Menu { get; set; }
        private static MenuManager MenuManager { get; set; }

        private static void Main(string[] args)
        {
            PopUpMenuGame game = new PopUpMenuGame();
            Menu = new MenuObject("main");
            MenuManager = new MenuManager();
            game.Construct(400, 200, 4, 4);
            game.Start();
        }

        public override void OnCreate()
        {
            MainSprite = Sprite.Load("./RetroMenu.png");

            DrawText(new Point(1, 1), "Testing", Pixel.Presets.Apricot);

            Menu.SetTable(3, 3);
            Menu["Attack"].SetId(101);
            Menu["Magic"].SetId(102);
            Menu["Defend"].SetId(103);
            Menu["Items"].SetId(104);
            Menu["Escape"].SetId(105);

            Menu["Dummy1"].SetId(201);
            Menu["Dummy2"].SetId(202);
            Menu["Dummy3"].SetId(203);
            Menu["Dummy4"].SetId(204);
            Menu["Dummy5"].SetId(205);
            Menu["Dummy6"].SetId(206);
            Menu["Dummy7"].SetId(207);
            Menu["Dummy8"].SetId(208);
            Menu["Dummy9"].SetId(209);

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

            MenuManager.Draw(this, MainSprite, new Vector2Int(30, 30));

            base.OnUpdate(delta);
        }
    }
}