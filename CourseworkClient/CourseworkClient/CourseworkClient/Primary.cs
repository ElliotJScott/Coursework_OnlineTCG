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
using System.IO;
using System.Net.Sockets;

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
        public Texture2D loginScreenBackground;
        public Texture2D textFieldTexture;
        public Texture2D title;
        public Texture2D buttonTexture;
        public Texture2D textFieldInfoTab;
        public SpriteFont mainFont;
        public static Primary game;
        public Form currentForm;
        TcpClient client;
        MemoryStream readMemoryStream, writeMemoryStream;
        BinaryReader binaryReader;
        BinaryWriter binaryWriter;
        const string ip = "127.0.0.1";
        const int port = 1337;
        const int bufferSize = 2048;
        byte[] readBuffer;
        public bool connected = false;


        static void Main(string[] args)
        {
            game = new Primary();
            game.Run();
        }
        public Primary()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1200;
            Window.Title = "Hearthclone";

        }

        protected override void Initialize()
        {
            client = new TcpClient();
            client.NoDelay = true;
            readBuffer = new byte[bufferSize];
            readMemoryStream = new MemoryStream();
            writeMemoryStream = new MemoryStream();
            binaryReader = new BinaryReader(readMemoryStream);
            binaryWriter = new BinaryWriter(writeMemoryStream);
            IsMouseVisible = true;
            ratio = CalculateRatio();
            connected = ConnectClient();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            loginScreenBackground = LoadLoadingScreenBackground();
            textFieldTexture = Content.Load<Texture2D>("TextFieldBox");
            buttonTexture = Content.Load<Texture2D>("ButtonIcon");
            currentForm = new LoginScreenForm();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("Mainfont");
            title = Content.Load<Texture2D>("Title");
            textFieldInfoTab = Content.Load<Texture2D>("InfoTab");
            
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
        public bool ConnectClient()
        {
            try
            {
                client.Connect(ip, port);
                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamReceived, null);
            }
            catch {
                return false;
            }
            return true;
        }
        private void StreamReceived(IAsyncResult ar)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = client.GetStream().EndRead(ar);
                
            }
            catch
            {
            }
            byte[] data = new byte[bytesRead];
            for (int i = 0; i < bytesRead; i++)
                data[i] = readBuffer[i];
            ProcessData(data);
            client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamReceived, null);
        }
        private void ProcessData(byte[] data)
        {
            readMemoryStream.SetLength(0);
            readMemoryStream.Position = 0;

            readMemoryStream.Write(data, 0, data.Length);
            readMemoryStream.Position = 0;
            Protocol p;
            p = (Protocol)binaryReader.ReadByte();
            //Console.WriteLine(p);
            HandleData(p);

        }

        private void HandleData(Protocol p)
        {
            throw new NotImplementedException();
        }
    }
}
