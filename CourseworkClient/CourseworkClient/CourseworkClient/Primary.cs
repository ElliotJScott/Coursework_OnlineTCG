using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Security.Cryptography;
using System.Text;
using CourseworkClient.Gui;

namespace CourseworkClient
{
    public enum ScreenRatio
    {
        FourByThree,
        SixteenByNine,
        Other
    }
    public class Primary : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        HMACMD5 hasher = new HMACMD5();
        public KeyPressHandler keypresshandler = new KeyPressHandler();
        ScreenRatio ratio;
        Texture2D loginScreenBackground;
        public Texture2D textFieldTexture;
        public Texture2D title;
        public Texture2D buttonTexture;
        public SpriteFont mainFont;
        public static Primary game;
        public Form currentForm;


        static void Main(string[] args)
        {
            game = new Primary();
            game.Run();
        }
        public Primary()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Hearthclone";

        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            ratio = CalculateRatio();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            loginScreenBackground = LoadLoadingScreenBackground();
            textFieldTexture = Content.Load<Texture2D>("TextFieldBox");
            buttonTexture = Content.Load<Texture2D>("ButtonIcon");
            currentForm = new LoginScreenForm(loginScreenBackground);
            //currentForm.formItems.Add(new TextField(new Rectangle(100, 100, 300, 30), 20));
            //currentForm.formItems.Add(new TextField(new Rectangle(100, 200, 300, 30), 15, "", true));
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("Mainfont");
            title = Content.Load<Texture2D>("Title");
            
        }

        protected override void Update(GameTime gameTime)
        {
            currentForm.Update();
            base.Update(gameTime);
            keypresshandler.UpdateOldState();
            GuiItem.UpdateOldState();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGoldenrodYellow);
            spriteBatch.Begin();
            currentForm.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
            
        }
        public string ComputeHash(string s)
        {
            byte[] b = hasher.ComputeHash(Encoding.UTF8.GetBytes(s));
            string output = "";
            foreach (byte e in b)
            {
                output += e.ToString("x0");
            }
            return output;
        }
        public ScreenRatio CalculateRatio()
        {
            if ((3d * GraphicsDevice.Viewport.Width) / (4d * GraphicsDevice.Viewport.Height) == 1d)
            {
                return ScreenRatio.FourByThree;
            }
            else if ((9d * GraphicsDevice.Viewport.Width) / (16d * GraphicsDevice.Viewport.Height) == 1d)
            {
                return ScreenRatio.SixteenByNine;
            }
            else return ScreenRatio.Other;
        }
        public Texture2D LoadLoadingScreenBackground()
        {
            switch (ratio)
            {
                case ScreenRatio.FourByThree:
                    return Content.Load<Texture2D>("4x3 Background");
                case ScreenRatio.SixteenByNine:
                case ScreenRatio.Other:
                    return Content.Load<Texture2D>("16x9 Background");
            }
            throw new InvalidOperationException("Something is very wrong here");
        }

    }
}
