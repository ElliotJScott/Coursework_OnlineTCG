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

namespace CourseworkClient
{

    public class Primary : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        HMACMD5 hasher = new HMACMD5();
        string test = "fff";
        KeyPressHandler keypresshandler = new KeyPressHandler();
        static void Main(string[] args)
        {
            using (Primary game = new Primary())
            {
                game.Run();
            }
        }
        public Primary()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
         
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            test = keypresshandler.NewTypedString(test, 20);
            Window.Title = test;
            //Window.Title = ComputeHash(new Random().Next().ToString());
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGoldenrodYellow);
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
        
    }
}
