namespace LSNoir.Callouts.SA
{
/*
class SocialMediaDrawer
{
    private static string _modelName = "a_f_m_beach_01", _vicName, _susName, _birthDay, _relStatus = "In a Relationship", _interested = "Enoying life\nHaving fun with \nfriends :)",
        _polViews = "Avid feminist", _about = "I live life to the fullest\nI love my friends more\nthan anything\nI enjoy going for drives", _status1, _status2;

    private static Texture _bg = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\Generic_Profile.jpg");
    private static Texture _photo;

    private static System.Drawing.Size _screenRes = GetScreenSize();
    private static System.Drawing.Color _black = System.Drawing.Color.FromArgb(111, 95, 105);

    private static KeyboardState _state;

    public static void DrawSocialMediaPage(PedData victim, PedData suspect, Base sender)
    {
        FillData(victim, suspect);
        
        MainCode.HideForm = true;

        Game.RawFrameRender += OnRawFrameRender;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        GameFiber.StartNew(delegate
        {
            while (sw.Elapsed.Seconds < 10)
                GameFiber.Yield();

            Game.RawFrameRender -= OnRawFrameRender;

            MainCode.HideForm = false;
        });
    }

    private static System.Drawing.Size GetScreenSize()
    {
        return Game.Resolution;
    }

    private static void FillData(PedData vic, PedData sus)
    {
        _modelName = vic.Model;
        ("Victim model: " + _modelName).AddLog();
        _photo = Game.CreateTextureFromFile(@"Plugins\LSPDFR\LSNoir\Textures\Persons\" + _modelName + ".jpg");
        _vicName = vic.Name;
        _birthDay = vic.Dob.ToShortDateString();

        var bfgf = sus.Gender == ExtensionMethods.Gender.Male ? "boo" : "bae";
            
        _status1 = $"I can't wait to hang out with my {bfgf} and go see a movie!! :)";

        _susName = sus.Name;

        _status2 = $"The best 2 months of my life have been spent with {_susName}";
    }

    private static void OnRawFrameRender(object sender, GraphicsEventArgs e)
    { 
        e.Graphics.DrawTexture(_bg, 0f, 0f, Game.Resolution.Width, Game.Resolution.Height);
        e.Graphics.DrawTexture(_photo, 510f, 85f, 178f, 194f);
        e.Graphics.DrawTexture(_photo, 710f, 320f, 20f, 30f);

        e.Graphics.DrawText(_vicName, "Arial", 16f, new System.Drawing.PointF(700, 150), _black);
        e.Graphics.DrawText(_vicName, "Arial", 13f, new System.Drawing.PointF(750, 330), System.Drawing.Color.FromArgb(117, 62, 67));
        e.Graphics.DrawText(_birthDay, "Arial", 13f, new System.Drawing.PointF(550, 350), _black);
        e.Graphics.DrawText(_relStatus, "Arial", 13f, new System.Drawing.PointF(615, 365), _black);
        e.Graphics.DrawText(_interested, "Arial", 13f, new System.Drawing.PointF(575, 410), _black);
        e.Graphics.DrawText(_polViews, "Arial", 13f, new System.Drawing.PointF(585, 540), _black);
        e.Graphics.DrawText(_about, "Arial", 13f, new System.Drawing.PointF(540, 585), _black);
        e.Graphics.DrawText(_status1, "Arial", 13f, new System.Drawing.PointF(720, 370), _black);
        e.Graphics.DrawText(_status2, "Arial", 13f, new System.Drawing.PointF(720, 350), _black);
    }

    private enum ScreenSize { Hd1080, Hd2K, Hd4K }
}*/
}
