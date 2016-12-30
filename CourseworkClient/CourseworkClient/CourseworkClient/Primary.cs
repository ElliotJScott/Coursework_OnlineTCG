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
using System.Threading;

namespace CourseworkClient
{
    public enum ScreenRatio
    {
        FourByThree,
        SixteenByNine,
        Other
    }
    public struct CardArtItem
    {
        public Texture2D art;
        public string cardName;
        public CardArtItem(Texture2D tex, string n)
        {
            art = tex;
            cardName = n;
        }
    }
    public class Primary : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Random rng = new Random();
        public KeyPressHandler keypresshandler = new KeyPressHandler();
        ScreenRatio ratio;
        public Texture2D loginScreenBackground, mainMenuBackground, inGameBackground;
        public Texture2D inGamePlayAreaTop, inGamePlayAreaBottom;
        public Texture2D textFieldTexture;
        public Texture2D sideBar;
        public Texture2D playButton, attackButton, discardButton;
        public Texture2D title;
        public Texture2D buttonTexture;
        public Texture2D textFieldInfoTab;
        public Texture2D greenArrowTexture, grayArrowTexture;
        public Texture2D effectDescBox;
        public Texture2D upgradeBigInner, upgradeSmallInner, techBigInner, techSmallInner, unitBigInner, unitSmallInner;
        public Texture2D cardOutlineSmall, cardOutlineBig;
        public Texture2D playSpace;
        public Texture2D lockMessageBox;
        public Texture2D screenDarkener;
        public Texture2D loadingIcon;
        public Texture2D cardBack;
        public SpriteFont mainFont, cardTextFont;
        public static Primary game;
        public Form currentForm;
        public FriendManager friendManager;
        public List<CardArtItem> cardArt = new List<CardArtItem>();
        TcpClient client;
        MemoryStream readMemoryStream, writeMemoryStream;
        BinaryReader binaryReader;
        BinaryWriter binaryWriter;
        const string ip = "127.0.0.1";
        const int port = 1337;
        const int bufferSize = 1000000;
        byte[] readBuffer;
        public bool connected = false;
        public int connectTimer = 0;
        public string username;
        public int selectedDeckNum = 0;

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
            graphics.PreferredBackBufferWidth = 1600;
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
            new Thread(new ThreadStart(ConnectClient)).Start();
            //friendManager = new FriendManager();
            base.Initialize();
        }
        protected override void LoadContent()
        {
            inGamePlayAreaTop = Content.Load<Texture2D>("InGamePlayAreaTop");
            inGamePlayAreaBottom = Content.Load<Texture2D>("InGamePlayAreaBottom");
            playSpace = Content.Load<Texture2D>("PlaySpace");
            upgradeBigInner = Content.Load<Texture2D>("Upgrade Card Inner Big");
            upgradeSmallInner = Content.Load<Texture2D>("Upgrade Card Inner Small");
            techBigInner = Content.Load<Texture2D>("Tech Card Inner Big");
            techSmallInner = Content.Load<Texture2D>("Tech Card Inner Small");
            unitBigInner = Content.Load<Texture2D>("Unit Card Inner Big");
            unitSmallInner = Content.Load<Texture2D>("Unit Card Inner Small");
            cardOutlineBig = Content.Load<Texture2D>("Card Outline Big");
            cardOutlineSmall = Content.Load<Texture2D>("Card Outline Small");
            loginScreenBackground = LoadLoadingScreenBackground();
            inGameBackground = LoadInGameBackground();
            mainMenuBackground = LoadMainMenuBackground();
            textFieldTexture = Content.Load<Texture2D>("TextFieldBox");
            effectDescBox = Content.Load<Texture2D>("Effect Description Box");
            buttonTexture = Content.Load<Texture2D>("ButtonIcon");
            currentForm = new LoginScreenForm();
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFont = Content.Load<SpriteFont>("Mainfont");
            cardTextFont = Content.Load<SpriteFont>("Cardtextfont");
            title = Content.Load<Texture2D>("Title");
            textFieldInfoTab = Content.Load<Texture2D>("InfoTab");
            greenArrowTexture = Content.Load<Texture2D>("Scroll Arrow");
            grayArrowTexture = Content.Load<Texture2D>("Grayed Out Scroll Arrow");
            screenDarkener = Content.Load<Texture2D>("Screen Darkener");
            lockMessageBox = Content.Load<Texture2D>("LockMessageBox");
            loadingIcon = Content.Load<Texture2D>("LoadingIcon");
            sideBar = Content.Load<Texture2D>("SideBar");
            playButton = Content.Load<Texture2D>("PlayButton");
            attackButton = Content.Load<Texture2D>("AttackButton");
            discardButton = Content.Load<Texture2D>("DiscardButton");
            cardBack = Content.Load<Texture2D>("Card Back");
        }

        protected override void Update(GameTime gameTime)
        {
            currentForm.Update();
            base.Update(gameTime);
            keypresshandler.UpdateOldState();
            GuiItem.UpdateOldState();
            if (!connected)
            {
                
                if (connectTimer++ >= 100)
                {

                    new Thread(new ThreadStart(ConnectClient)).Start();
                    connectTimer = 0;
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            currentForm.Draw(spriteBatch);
            currentForm.PostDraw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);

        }

        internal Texture2D GetCardArt(string name)
        {
            try
            {
                foreach (CardArtItem c in cardArt)
                {
                    if (c.cardName == name) return c.art;
                }
            }
            catch
            {
                Thread.Sleep(10);
                return GetCardArt(name);
            }
            throw new ArgumentException();
        }

        //Do not change this method ever!
        public string ComputeHash(string s)
        {
            int l = 16;
            char[] f = s.ToCharArray();
            string o = "";
            for (int i = 0; i < l; i++)
            {
                int r = f[i % f.Length];

                o += ((r ^ i) % 16).ToString("X"); ;
            }
            return o;

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
        public Texture2D LoadInGameBackground()
        {
            return loadBackground("InGameBackground4x3", "InGameBackground16x9");
        }
        public Texture2D LoadLoadingScreenBackground()
        {
            return loadBackground("4x3 Background", "16x9 Background");
        }
        public Texture2D LoadMainMenuBackground()
        {
            return loadBackground("MainMenuBackground4x3", "MainMenuBackground16x9");
        }
        public Texture2D loadBackground(string fourbythree, string sixteenbynine)
        {
            switch (ratio)
            {
                case ScreenRatio.FourByThree:
                    return Content.Load<Texture2D>(fourbythree);
                case ScreenRatio.SixteenByNine:
                case ScreenRatio.Other:
                    return Content.Load<Texture2D>(sixteenbynine);
            }
            throw new InvalidOperationException();
        }
        public void ConnectClient()
        {
            try
            {
                client.Connect(ip, port);
                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamReceived, null);               

            }
            catch (Exception)
            {
                try
                {
                    currentForm.Lock("Error connecting to server.\nCheck your internet connection\nand that your firewall has not\nblocked this application.");
                    connected = false;
                    return;
                }
                catch { }
            }
            try
            {
                if (currentForm.GetType() == typeof(CreateAccountForm))
                {
                    ((CreateAccountForm)currentForm).errorMessageText = "";
                }
                else if (currentForm.GetType() == typeof(LoginScreenForm))
                {
                    ((LoginScreenForm)currentForm).errorMessageText = "";
                }
            }
            catch (NullReferenceException) { }
            connected = true;
            try
            {
                currentForm.Unlock();
            }
            catch { }
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
            {
                data[i] = readBuffer[i];
            }
            ProcessData(data);
            try
            {
                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamReceived, null);
            }
            catch
            {
                connected = false;
            }
        }
        public void ExitGame()
        {
            System.Windows.Forms.MessageBox.Show("Game has encountered issue. Closing now");
            Environment.Exit(0);
        }
        private void ProcessData(byte[] data)
        {

            string s = byteArrayToString(data);
            List<string> l = s.Split('`').ToList();
            foreach (string x in l)
            {
                if (x != null && x.Length != 0) HandleData((Protocol)x[0], x.Substring(1));
            }


        }

        static string byteArrayToString(byte[] b)
        {
            string output = "";
            foreach (byte a in b)
            {
                output += (char)a;
            }
            return output;
        }
        private void HandleData(Protocol p, string s)
        {
            //Console.WriteLine("{0} : {1}", p, s);
            switch (p)
            {
                case Protocol.UsernameTaken:
                    ((CreateAccountForm)currentForm).errorMessageText = "Username taken";
                    currentForm.Unlock();
                    break;
                case Protocol.BadCredentials:
                    ((LoginScreenForm)currentForm).errorMessageText = "Username or Password is incorrect";
                    currentForm.Unlock();
                    break;
                case Protocol.GoodCredentials:
                    username = ((TextField)(((LoginScreenForm)currentForm).formItems[0])).text;
                    currentForm = new MainMenuForm();
                    break;
                case Protocol.FriendStatus:
                    //Add more stuff here later
                    break;
                case Protocol.LoggedIn:
                    ((LoginScreenForm)currentForm).errorMessageText = "You are already logged in on another instance.";
                    currentForm.Unlock();
                    break;
                case Protocol.EnterMatch:
                    //ShowMessage("Entering match");
                    currentForm = new InGameForm(Deck.decks[selectedDeckNum], Convert.ToBoolean(Convert.ToInt32(s.Substring(0,1))), s.Substring(1));
                    break;
                case Protocol.CardData:
                    AddNewCard(s);
                    break;
                case Protocol.EffectData:
                    AddNewEffect(s);
                    break;
                case Protocol.DeckData:
                    AddNewDeck(s);
                    break;
                case Protocol.DeckCardsData:
                    AddCardToDeck(s);
                    break;
                case Protocol.CardEffect:
                    AddEffectToCard(s);
                    break;
                case Protocol.DataTransmissionTest:
                    Console.WriteLine("{0} : {1}", s.Length, s);
                    break;
                case Protocol.UsernameNotTaken:
                    ((CreateAccountForm)currentForm).errorMessageText = "Account created!";
                    currentForm.Unlock();
                    break;
                case Protocol.AttackWithUnit:
                    ((InGameForm)currentForm).AddAttackingUnitToChain(Convert.ToInt32(s), true);//Add this attacking fellow to the chain
                    ((InGameForm)currentForm).OfferAttackCounterOptions();
                    break;
                case Protocol.DefendWithUnit:
                    ((InGameForm)currentForm).ResolveChainWithDefender(s, true);//s in the form id
                    break;
                case Protocol.DiscardTech:
                    ((InGameForm)currentForm).DiscardCardFromEnemyHand(s);//s in the form name
                    ((InGameForm)currentForm).WaitOnEnemySelection();
                    break;
                case Protocol.PlayUpgrade:
                    ((InGameForm)currentForm).AddUpgradeToChain(s, false);//s in the form name
                    ((InGameForm)currentForm).OfferCardPlayCounters();
                    break;
                case Protocol.NoCounter:
                    ((InGameForm)currentForm).ResolveChain();
                    break;
                case Protocol.PlayTech:
                    ((InGameForm)currentForm).AddTechToChain(s, false);//s in the form name
                    ((InGameForm)currentForm).OfferCardPlayCounters();
                    break;
                case Protocol.PlayUnit:
                    ((InGameForm)currentForm).AddCardPlayToChain(s);//Add this played fellow to the chain
                    ((InGameForm)currentForm).OfferCardPlayCounters();
                    break;
                case Protocol.ControlUnit:
                    ((InGameForm)currentForm).MoveUnitToEnemy(s);
                    break;
                case Protocol.DiscardFromUpgradeDeck:
                case Protocol.DiscardFromDeck:
                    ((InGameForm)currentForm).DiscardCardFromEnemyDeck(s);
                    break;
                case Protocol.KillUnit:
                    ((InGameForm)currentForm).KillUnit(Convert.ToInt32(s));
                    break;
                case Protocol.EquipUpgrade:
                    ((InGameForm)currentForm).AddUpgradeToCard(s, false); //s in the form cardid
                    break;
                case Protocol.ReplaceUnit:
                    ((InGameForm)currentForm).ReplaceUnit(s);//s in the form id (of the card to be replaced). This is for the c'tan and convertible ultramarines
                    break;
                case Protocol.ReturnUnit:
                    ((InGameForm)currentForm).MoveUnitFromEnemy(s); //s in the form id
                    break;
                case Protocol.PlayUnitFromDeck:
                    ((InGameForm)currentForm).PlayUnitFromEnemyDeck(s); //s in the form name
                    break;
                case Protocol.NoCardsInDeck:
                    ((InGameForm)currentForm).cardsInEnemyDeck = false;
                    break;
                case Protocol.NoCardsInUpgradeDeck:
                    ((InGameForm)currentForm).cardsInEnemyUpgradeDeck = false;
                    break;
                default:
                    ShowMessage("Unexpected Protocol: " + p.ToString());
                    ExitGame();
                    break;

            }
        }
        private void AddNewCard(string s)
        {
         
            string[] data = s.Split('|');
            if (data[3] != "")
            {
                Card.allCards.Add(new Card(data[0], (CardType)Convert.ToInt32(data[1]), Convert.ToInt32(data[5]), (Rarity)Convert.ToInt32(data[2]), Convert.ToInt32(data[3]), Convert.ToInt32(data[4])));
            }
            else Card.allCards.Add(new Card(data[0], (CardType)Convert.ToInt32(data[1]), Convert.ToInt32(data[5]), (Rarity)Convert.ToInt32(data[2])));
        }
        private void AddNewEffect(string s)
        {
            string[] data = s.Split('|');
            Effect.allEffects.Add(new Effect(data[0], data[1], data[2]));
        }
        private void AddEffectToCard(string s)
        {
            string[] data = s.Split('|');
            Card.AddEffectToBaseCard(data[0], data[1]);
        }
        private void AddNewDeck(string s)
        {
            string[] data = s.Split('|');
            if (Convert.ToBoolean(data[1]) == true)
                Deck.allOwnedCards = new Deck(Convert.ToInt32(data[0]));
            else Deck.decks.Add(new Deck(Convert.ToInt32(data[0])));
        }
        private void AddCardToDeck(string s)
        {
            string[] data = s.Split('|');
            Deck.AddCardToDeck(Card.getCard(data[0]), Convert.ToInt32(data[1]), Convert.ToInt32(data[2]));
        }
        public void AddNewCardArt(string cardName)
        {
            Texture2D art;
            try
            {
                
                art = Content.Load<Texture2D>("CardArt\\" + cardName.Replace(':', '_'));
            }
            catch
            {
                Console.WriteLine("Art not found for {0}. Using blank art instead.", cardName);
                art = Content.Load<Texture2D>("Blank Card Art");
            }
            cardArt.Add(new CardArtItem(art, cardName));
        }
        public static void ShowMessage(string s)
        {
            System.Windows.Forms.MessageBox.Show(s);
        }
        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            lock (ms)
            {
                byte[] result;
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
                return result;
            }


        }
        public void SendData(byte[] b)
        {
            try
            {
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch
            {
                ////Console.WriteLine("Error sending data");
            }
        }
        public void WriteDataToStream(Protocol p)
        {


            writeMemoryStream.Position = 0;
            binaryWriter.Write((byte)p);
            SendData(GetDataFromMemoryStream(writeMemoryStream));



        }
        public void WriteDataToStream(Protocol p, params string[] o)
        {
            writeMemoryStream.Position = 0;
            binaryWriter.Write((byte)p);
            foreach (string e in o)
            {
                binaryWriter.Write(e);
                SendData(GetDataFromMemoryStream(writeMemoryStream));
                writeMemoryStream.Position = 0;
                binaryWriter.Write((byte)p);

            }

        }
    }
}
