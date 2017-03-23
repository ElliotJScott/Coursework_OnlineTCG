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
    /// <summary>
    /// An enum to describe the shape of the window
    /// </summary>
    public enum ScreenRatio
    {
        FourByThree,
        SixteenByNine,
        Other
    }
    /// <summary>
    /// An item to map card art to card names
    /// </summary>
    public struct CardArtItem
    {
        public Texture2D art;
        public string cardName;
        public CardArtItem(Texture2D tex, string n)
        {
            Primary.Log("Creating CardArtItem " + n + " | " + tex);
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
        public Texture2D cardTappedIndicator;
        public Texture2D remainingHealth, missingHealth;
        public Texture2D cardCountCircle;
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
        public int elo = 0;
        public int coins = 0;

        /// <summary>
        /// Entry point of the program
        /// </summary>
        /// <param name="args">Input data on entry</param>
        static void Main(string[] args)
        {
            Log("Starting program");
            game = new Primary();
            game.Run();
        }

        /// <summary>
        /// Creates a new instance of the game
        /// </summary>
        public Primary()
        {
            Log("Creating instance of game");
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1080;
            Window.Title = "Hearthclone";
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Called when an instance of the game is created
        /// </summary>
        protected override void Initialize()
        {
            Log("Initialising the game");
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
            Log("Finished initialising");
        }
        /// <summary>
        /// Loads any media that needs to be loaded on starting the game
        /// </summary>
        protected override void LoadContent()
        {
            Log("Beginning loading content");
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
            cardTappedIndicator = Content.Load<Texture2D>("Card Reddener");
            remainingHealth = Content.Load<Texture2D>("Remaining Health");
            missingHealth = Content.Load<Texture2D>("Missing Health");
            cardCountCircle = Content.Load<Texture2D>("CardCountCircle");
            Log("Finished loading content");
        }
        /// <summary>
        /// Performs game logic every tick
        /// </summary>
        /// <param name="gameTime">The game timing state</param>
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

        /// <summary>
        /// Describes what to draw every frame
        /// </summary>
        /// <param name="gameTime">The game timing state</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            currentForm.Draw(spriteBatch);
            currentForm.PostDraw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// Gets the art for a given card
        /// </summary>
        /// <param name="name">The name of the card</param>
        /// <returns>The art of the card</returns>
        internal Texture2D GetCardArt(string name)
        {
            try
            {
                foreach (CardArtItem c in cardArt)
                {
                    if (c.cardName == name) return c.art;
                }
            }
            catch (Exception e)
            {
                Log(e);
                Log("Error loading card art. The cardArt list hasn't been initialised yet. Waiting then trying again.");
                Thread.Sleep(10);
                return GetCardArt(name);
            }
            Log("Card art not found. This is not good.");
            throw new ArgumentException();
        }

        /// <summary>
        /// Computes the hash of a given string
        /// </summary>
        /// <param name="s">The string to hash</param>
        /// <returns>The hashed string</returns>
        public string ComputeHash(string s)
        {
            Log("Calculating password hash");
            int l = 16;
            char[] f = s.ToCharArray();
            string o = "";
            for (int i = 0; i < l; i++)
            {
                int r = f[i % f.Length];

                o += ((r ^ i) % 16).ToString("X"); ;
            }
            Log("Hash calculated");
            return o;

        }

        /// <summary>
        /// Gets the ratio of the screen
        /// </summary>
        /// <returns>Returns the ratio of the screen</returns>
        public ScreenRatio CalculateRatio()
        {
            Log("Calculating screen ration");
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
        /// <summary>
        /// Loads the In Game Background
        /// </summary>
        /// <returns>The In game background</returns>
        public Texture2D LoadInGameBackground()
        {
            return loadBackground("InGameBackground4x3", "InGameBackground16x9");
        }
        /// <summary>
        /// Loads the loading screen background
        /// </summary>
        /// <returns>The loading screen background</returns>
        public Texture2D LoadLoadingScreenBackground()
        {
            return loadBackground("4x3 Background", "16x9 Background");
        }
        /// <summary>
        /// Loads the main menu background
        /// </summary>
        /// <returns>The main menu background</returns>
        public Texture2D LoadMainMenuBackground()
        {
            return loadBackground("MainMenuBackground4x3", "MainMenuBackground16x9");
        }
        /// <summary>
        /// Loads the correct background texture
        /// </summary>
        /// <param name="fourbythree">The name of the 4x3 background</param>
        /// <param name="sixteenbynine">The name of the 16x9 background</param>
        /// <returns>The correct background texture</returns>
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
        /// <summary>
        /// Attempts to connect the client to the server
        /// </summary>
        public void ConnectClient()
        {
            try
            {
                Log("Attempting to connect client");
                client.Connect(ip, port);
                client.GetStream().BeginRead(readBuffer, 0, bufferSize, StreamReceived, null);

            }
            catch (Exception)
            {
                Log("Failed to connect to server");
                try
                {
                    Log("Locking form");
                    currentForm.Lock("Error connecting to server.\nCheck your internet connection\nand that your firewall has not\nblocked this application.");
                    connected = false;
                    return;
                }
                catch
                {
                    Log("Failed to lock form");
                }
            }
            try
            {
                Log("Removing error message from form");
                if (currentForm.GetType() == typeof(CreateAccountForm))
                {
                    ((CreateAccountForm)currentForm).errorMessageText = "";
                }
                else if (currentForm.GetType() == typeof(LoginScreenForm))
                {
                    ((LoginScreenForm)currentForm).errorMessageText = "";
                }
            }
            catch (NullReferenceException)
            {
                Log("Form not loaded yet");
            }
            connected = true;
            try
            {
                Log("Unlocking form");
                currentForm.Unlock();
            }
            catch { }
        }
        /// <summary>
        /// Called whenever data is received from the server
        /// </summary>
        /// <param name="ar">Used to get the number of bytes received</param>
        private void StreamReceived(IAsyncResult ar)
        {
            Log("Data received");
            int bytesRead = 0;
            try
            {
                bytesRead = client.GetStream().EndRead(ar);
            }
            catch
            {
                Log("Bad data received");
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
                Log("Connection terminated with server");
                connected = false;
            }
        }
        /// <summary>
        /// Exits the game
        /// </summary>
        public void ExitGame()
        {
            Log("A fatal error has been encountered. Exiting now");
            System.Windows.Forms.MessageBox.Show("Game has encountered issue. Closing now");
            Environment.Exit(0);
        }
        /// <summary>
        /// Processes input data so that it can be handled effectively
        /// </summary>
        /// <param name="data">The data to process</param>
        private void ProcessData(byte[] data)
        {

            string s = byteArrayToString(data);
            List<string> l = s.Split('`').ToList();
            foreach (string x in l)
            {
                if (x != null && x.Length != 0) HandleData((Protocol)x[0], x.Substring(1));
            }


        }
        /// <summary>
        /// Converts an input byte array to a string
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns>The input byte array as a string</returns>
        static string byteArrayToString(byte[] b)
        {
            string output = "";
            foreach (byte a in b)
            {
                output += (char)a;
            }
            return output;
        }
        /// <summary>
        /// Handles received data based on the protocol it was sent with
        /// </summary>
        /// <param name="p">The received protocol</param>
        /// <param name="s">The data received</param>
        private void HandleData(Protocol p, string s)
        {
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
                    try
                    {
                        ((LoginScreenForm)currentForm).errorMessageText = "You are already logged in on another instance.";
                        currentForm.Unlock();
                    }
                    catch
                    {
                        currentForm = new LoginScreenForm();
                    }
                    break;
                case Protocol.EnterMatch:
                    //ShowMessage("Entering match");
                    currentForm = new InGameForm(Deck.decks[selectedDeckNum], Convert.ToBoolean(Convert.ToInt32(s.Substring(0, 1))), s.Substring(1));
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
                    ((InGameForm)currentForm).ResolveChainWithDefender(Convert.ToInt32(s), true);//s in the form id
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
                    ((InGameForm)currentForm).AddTechToChain(s);//s in the form name
                    ((InGameForm)currentForm).OfferCardPlayCounters();
                    break;
                case Protocol.PlayUnit:
                    ((InGameForm)currentForm).AddUnitPlayToChain(s);
                    ((InGameForm)currentForm).OfferCardPlayCounters();//Add this played fellow to the chain
                    break;
                case Protocol.ControlUnit:
                    ((InGameForm)currentForm).MoveUnitToEnemy(Convert.ToInt32(s));
                    break;
                case Protocol.DiscardFromUpgradeDeck:
                case Protocol.DiscardFromDeck:
                    ((InGameForm)currentForm).DiscardCardFromEnemyDeck(s);
                    break;
                case Protocol.KillUnit:
                    ((InGameForm)currentForm).KillUnit(Convert.ToInt32(s));
                    break;
                case Protocol.EquipUpgrade:
                    ((InGameForm)currentForm).AddUpgradeToCard(Convert.ToInt32(s), false); //s in the form cardid
                    break;
                case Protocol.ReplaceUnit:
                    ((InGameForm)currentForm).ReplaceUnit(Convert.ToInt32(s));//s in the form id (of the card to be replaced). This is for the c'tan and convertible ultramarines
                    break;
                case Protocol.ReturnUnit:
                    ((InGameForm)currentForm).MoveUnitFromEnemy(Convert.ToInt32(s));
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
                case Protocol.AddCardToEnemyHand:
                    ((InGameForm)currentForm).numEnemyCardsInHand++;
                    break;
                case Protocol.BeginSelection:
                    ((InGameForm)currentForm).WaitOnEnemySelection();
                    break;
                case Protocol.EndSelection:
                    currentForm.Unlock();
                    try
                    {
                        ((InGameForm)currentForm).chain.RemoveLast();
                        ((InGameForm)currentForm).ResolveChain();
                    }
                    catch { }
                    break;
                case Protocol.EndTurn:
                    ((InGameForm)currentForm).StartTurn();
                    break;
                case Protocol.AddToEnemyFromDiscard:
                    ((InGameForm)currentForm).AddCardToEnemyHand(s, true);
                    break;
                case Protocol.Artillery:
                    ((InGameForm)currentForm).DealDamageToUnit(Convert.ToInt32(s), 3, false);
                    break;
                case Protocol.DeathInHonour:
                    ((InGameForm)currentForm).KillUnit(Convert.ToInt32(s));
#warning put the damage component in later. Remember to put it in before the line above
                    break;
                case Protocol.RemoveCardFromEnemyHand:
                    ((InGameForm)currentForm).numEnemyCardsInHand--;
                    ((InGameForm)currentForm).enemyDiscardPile.Add(Card.getCard(s));
                    break;
                case Protocol.HealHalf:
                    ((InGameForm)currentForm).HealUnit(Convert.ToInt32(s), 0.5, false);
                    break;
                case Protocol.HealFull:
                    ((InGameForm)currentForm).HealUnit(Convert.ToInt32(s), 1, false);
                    break;
                case Protocol.PowerExtraction:
                    ((InGameForm)currentForm).enemyResource += 2;
                    break;
                case Protocol.AddCardFromDiscard:
                    ((InGameForm)currentForm).AddCardToEnemyHand(s, false);
                    break;
                case Protocol.ReturnUnitToHand:
                    ((InGameForm)currentForm).ReturnUnitToHand(Convert.ToInt32(s), false);
                    break;
                case Protocol.EloAndCoins:
                    string[] x = s.Split('a');
                    elo = Convert.ToInt32(x[0]);
                    coins = Convert.ToInt32(x[1]);
                    if (currentForm.GetType() == typeof(InGameForm))
                        currentForm = new EndGameForm();
                    break;
                case Protocol.PackCards:
                    string[] g = s.Split('|');
                    Card[] cards = new Card[g.Length];
                    for (int i = 0; i < g.Length; i++)
                    {
                        Card c = Card.getCard(g[i]);
                        Deck.allOwnedCards.AddAdditionalCard(c);
                        cards[i] = c;
                    }
                    currentForm = new PackOpeningForm(cards);
                    break;
                case Protocol.NewDBDeckID:
                    string[] r = s.Split('|');
                    int newid = Convert.ToInt32(r[0]);
                    int oldid = Convert.ToInt32(r[1]);
                    foreach (Deck d in Deck.decks)
                    {
                        if (d.dbID == oldid)
                        {
                            d.dbID = newid;
                            break;
                        }
                    }
                    DeckManagerForm cf = (DeckManagerForm)currentForm;
                    foreach (Deck d in cf.decks)
                    {
                        if (d.dbID == oldid)
                        {
                            d.dbID = newid;
                            cf.TransmitDecks(newid);
                        }
                    }
                    break;
                default:
                    ShowMessage("Unexpected Protocol: " + p.ToString());
                    ExitGame();
                    break;

            }
        }
        /// <summary>
        /// Adds a new card
        /// </summary>
        /// <param name="s">The card data to use</param>
        private void AddNewCard(string s)
        {
            string[] data = s.Split('|');
            if (data[3] != "")
            {
                Card.allCards.Add(new Card(data[0], (CardType)Convert.ToInt32(data[1]), Convert.ToInt32(data[5]), (Rarity)Convert.ToInt32(data[2]), Convert.ToInt32(data[3]), Convert.ToInt32(data[4])));
            }
            else Card.allCards.Add(new Card(data[0], (CardType)Convert.ToInt32(data[1]), Convert.ToInt32(data[5]), (Rarity)Convert.ToInt32(data[2])));
        }
        /// <summary>
        /// Adds a new effect
        /// </summary>
        /// <param name="s">The effect data to use</param>
        private void AddNewEffect(string s)
        {
            string[] data = s.Split('|');
            Effect.allEffects.Add(new Effect(data[0], data[1], data[2]));
        }
        /// <summary>
        /// Adds an effect to a card
        /// </summary>
        /// <param name="s">The card and the effect</param>
        private void AddEffectToCard(string s)
        {
            string[] data = s.Split('|');
            Card.AddEffectToBaseCard(data[0], data[1]);
        }
        /// <summary>
        /// Adds a new deck
        /// </summary>
        /// <param name="s">The deck data to use</param>
        private void AddNewDeck(string s)
        {
            string[] data = s.Split('|');
            if (Convert.ToBoolean(data[1]) == true)
                Deck.allOwnedCards = new Deck(Convert.ToInt32(data[0]));
            else Deck.decks.Add(new Deck(Convert.ToInt32(data[0])));
        }
        /// <summary>
        /// Adds a card to a deck
        /// </summary>
        /// <param name="s">The deck and card data to use</param>
        private void AddCardToDeck(string s)
        {
            string[] data = s.Split('|');
            Deck.AddCardToDeck(Card.getCard(data[0]), Convert.ToInt32(data[1]), Convert.ToInt32(data[2]));
        }
        /// <summary>
        /// Adds art to a given card
        /// </summary>
        /// <param name="cardName">The name of the card</param>
        public void AddNewCardArt(string cardName)
        {
            Texture2D art;
            try
            {
                art = Content.Load<Texture2D>("CardArt\\" + cardName.Replace(':', '_'));
            }
            catch
            {
                Log(string.Format("Art not found for {0}. Using blank art instead.", cardName));
                art = Content.Load<Texture2D>("Blank Card Art");
            }
            cardArt.Add(new CardArtItem(art, cardName));
        }
        /// <summary>
        /// Displays a message in a popup window
        /// </summary>
        /// <param name="s">The message to display</param>
        public static void ShowMessage(string s)
        {
            Log("Displaying message");
            System.Windows.Forms.MessageBox.Show(s);
        }
        /// <summary>
        /// Gets the data from a given memory stream
        /// </summary>
        /// <param name="ms">The memory stream to get the data from</param>
        /// <returns>The data from the memorystream</returns>
        private byte[] GetDataFromMemoryStream(MemoryStream ms)
        {
            lock (ms)
            {
                Log("Getting data from memory stream");
                byte[] result;
                int bytesWritten = (int)ms.Position;
                result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
                return result;
            }


        }
        /// <summary>
        /// Send the given data to the server
        /// </summary>
        /// <param name="b">The data to send</param>
        public void SendData(byte[] b)
        {
            try
            {
                Log("Sending Data length " + b.Length);
                lock (client.GetStream())
                {
                    client.GetStream().BeginWrite(b, 0, b.Length, null, null);
                }
            }
            catch
            {
                Log("Failed to send data length " + b.Length);
            }
        }
        /// <summary>
        /// Send the given protocol to the server
        /// </summary>
        /// <param name="p">The protocol to send</param>
        public void WriteDataToStream(Protocol p)
        {

            Log(string.Format("Sending data Protocol : {0}", p));
            SendData(new byte[] { (byte)p});



        }
        /// <summary>
        /// Send the given protocol and string to the server
        /// </summary>
        /// <param name="p">The protocol to send</param>
        /// <param name="o">The string to send</param>
        public void WriteDataToStream(Protocol p, string o)
        {
            Log(string.Format("Sending data Protocol : {0} Element : {1}", p, o));
            byte[] data = addProtocolToArray(toByteArray(o), p);
            SendData(data);


        }
        /// <summary>
        /// Logs a given piece of data to the console with a timestamp
        /// </summary>
        /// <param name="s">The data to log</param>
        public static void Log(object s)
        {
            DateTime now = DateTime.Now;
            string f = string.Format("[{0}:{1}:{2}:{3}] {4}", now.Hour, now.Minute, now.Second, now.Millisecond, s);
            Thread t = new Thread(() => Console.WriteLine(f));
            t.Start();
        }
        /// <summary>
        /// Adds a given protocol to the front of a byte array
        /// </summary>
        /// <param name="b">The byte array to add the protocol to</param>
        /// <param name="p">The protocol to add</param>
        /// <returns>The byte array with the protocol added</returns>
        public static byte[] addProtocolToArray(byte[] b, Protocol p)
        {
            byte[] e = new byte[b.Length + 1];
            e[0] = (byte)p;
            for (int i = 0; i < b.Length; i++)
            {
                e[i + 1] = b[i];
            }
            return e;
        }
        /// <summary>
        /// Converts a given string to a byte array
        /// </summary>
        /// <param name="s">The string to convert</param>
        /// <returns>The converted string</returns>
        public static byte[] toByteArray(string s)
        {
            char[] c = s.ToCharArray();
            byte[] b = new byte[c.Length];
            for (int i = 0; i < c.Length; i++)
            {
                b[i] = (byte)c[i];
            }
            return b;
        }
    }
}
