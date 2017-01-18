using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace CourseworkClient.Gui
{
    enum PlayState
    {
        Success,
        Countered,
        NotExecuted,
    }
    enum Location
    {
        InHand,
        InMainDeck,
        InUpgradeDeck,
        InUnits,
        InDiscardPile,
        InEnemyUnits,
        InEnemyDiscardPile,
    }
    enum Function
    {
        KillUnit,
        EquipUpgrade,
        DefendWithUnit,
        ControlUnit,
        ReplaceUnit,
        PlayUnitFromDeck,
        AddCardToHand,
        RepairPack,
        ReturnUnitToHand,
        EquivalentExchange,
        DiscardCard,
        AddFromEnemyDiscard,
        ReturnCardFromDiscard,
        PowerExtraction,
        HealUnit,
        DeathInHonour,
        AntiVehicleArtillery,
        HealHalf,
        Counter,
    }
    class ChainItem
    {
        public SmallCard card;
        public PlayState state;
        public bool playerPlayed;
        public bool needSelection;
        public ChainItem(SmallCard c, bool plPlayed, bool select)
        {
            needSelection = select;
            playerPlayed = plPlayed;
            card = c;
            state = PlayState.NotExecuted;
        }
        public override string ToString()
        {
            string username;
            if (playerPlayed)
            {
                username = Primary.game.username;
            }
            else
            {
                username = ((InGameForm)Primary.game.currentForm).enemyUsername;
            }
            if (card.id > 0)
            {
                return string.Format("{0} attacked with {1}", username, card.card.name);
            }
            else return string.Format("{0} played {1}", username, card.card.name);
        }
    }

    struct SelectionCondition
    {
        public Effect[] requiredEffects;
        public CardType? type;
        public int maxCost;
        public Location? location;
        public bool fulfil;

        #region Conditions
        public static SelectionCondition ultramarine = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Ultramarine"));
        public static SelectionCondition chaosSpaceMarine = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Chaos Space Marine"));
        public static SelectionCondition eldar = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Eldar"));
        public static SelectionCondition necron = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Necron"));
        public static SelectionCondition tyranid = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Tyranid"));
        public static SelectionCondition brood = new SelectionCondition(true, Location.InMainDeck, CardType.Unit, 1000, null, Effect.GetEffect("Brood"));
        public static SelectionCondition counterTech = new SelectionCondition(true, Location.InHand, CardType.Tech);
        public static SelectionCondition transportUnit = new SelectionCondition(true, Location.InMainDeck, CardType.Unit, 5);
        public static SelectionCondition notVehicle = new SelectionCondition(false, null, CardType.Unit, 1000, null, Effect.GetEffect("Vehicle"));
        public static SelectionCondition unit = new SelectionCondition(true, null, CardType.Unit);
        public static SelectionCondition alliedUnit = new SelectionCondition(true, Location.InUnits, CardType.Unit);
        public static SelectionCondition alliedUntappedUnit = new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, true);
        public static SelectionCondition notNecron = new SelectionCondition(false, null, CardType.Unit, 1000, null, Effect.GetEffect("Necron"));
        public static SelectionCondition corruptable = new SelectionCondition(false, null, CardType.Unit, 1000, null, Effect.GetEffect("Uncorruptable"));
        public static SelectionCondition enemyUnit = new SelectionCondition(true, Location.InEnemyUnits, CardType.Unit);
        public static SelectionCondition noPsychicHood = new SelectionCondition(false, null, CardType.Unit, 1000, null, Effect.GetEffect("Psychic Hood"));
        public static SelectionCondition psychicHoodAllowed = new SelectionCondition(false, null, CardType.Unit, 1000, null, Effect.GetEffect("Psyker"));
        public static SelectionCondition handCard = new SelectionCondition(true, Location.InHand);
        public static SelectionCondition playerDiscarded = new SelectionCondition(true, Location.InDiscardPile);
        #endregion

        public SelectionCondition(bool f, Location? loc, CardType? t = null, int c = 1000, bool? tapped = null, params Effect[] ef)
        {
            fulfil = f;
            location = loc;
            maxCost = c;
            type = t;
            requiredEffects = ef;
        }
    }
    class Selection
    {
        public SelectionCondition[] conditions;
        public int quantity;
        public bool fulfilAll;
        public Function function;
        public Selection(int num, Function func, bool f = true, params SelectionCondition[] sel)
        {
            fulfilAll = f;
            quantity = num;
            conditions = sel;
        }
        public List<SmallCard> GetCards()
        {
            List<SmallCard> output = new List<SmallCard>();
            Location[] locationsArray = new Location[0];
            if (fulfilAll) locationsArray = (Location[])Enum.GetValues(typeof(Location));
            List<Location> locations = new List<Location>(locationsArray);
            foreach (SelectionCondition c in conditions)
            {
                if (fulfilAll == true)
                {
                    if (c.location != null)
                    {
                        if (locations.Contains(c.location.Value))
                        {
                            locations = new List<Location>(new Location[] { c.location.Value });
                        }
                        else
                        {
                            locations = new List<Location>();
                        }
                    }
                }
                else if (c.location != null)
                {
                    if (locations.Contains(c.location.Value))
                    {
                        locations.Add(c.location.Value);
                    }
                }
                else
                {
                    Location[] locationsArray2 = (Location[])Enum.GetValues(typeof(Location));
                    locations = new List<Location>(locationsArray2);
                }
            }
            if (locations.Count == 0)
            {
                throw new InvalidOperationException();
            }
            foreach (Location l in locations)
            {
                List<SmallCard> cards = ((InGameForm)Primary.game.currentForm).GetValidCardsAtLocation(l, conditions, fulfilAll);
                foreach (SmallCard c in cards) output.Add(c);
            }
            return output;
        }
    }
    struct SelectionItem
    {
        public Selection selection;
        public string text;
        public SelectionItem(Selection s, string x)
        {
            selection = s;
            text = x;
        }
    }
#warning InGameForm
    class InGameForm : Form
    {
        public bool myTurn;
        MouseState oldState;
        List<Card> deck = new List<Card>();
        List<Card> upgradeDeck = new List<Card>();
        public List<SmallCard> hand = new List<SmallCard>();
        public List<SmallCard> units = new List<SmallCard>();
        List<Card> discardPile = new List<Card>();
        public BigCard bigCard = null;
        Button[] counterOptionButtons = new Button[0];
        public bool bigCardChange = false;
        public int numEnemyCardsInHand;
        public bool cardsInEnemyDeck = true;
        public bool cardsInEnemyUpgradeDeck = true;
        List<Card> enemyDiscardPile = new List<Card>();
        public List<SmallCard> enemyUnits = new List<SmallCard>();
        public List<SmallCard> playerUpgrades = new List<SmallCard>();
        public List<SmallCard> enemyUpgrades = new List<SmallCard>();
        public LinkedList<ChainItem> chain = new LinkedList<ChainItem>();
        List<Upgrade> upgradesInPlay = new List<Upgrade>();
        const int maxUnitsInPlay = 10;
        int yOffset;
        public string enemyUsername;
        const int minYOffset = 0;
        readonly int maxYOffset;
        const int yAccel = 20;
        int nextID = 0;
        const double researchPT = 0.3;
        const double startingResource = 3;
        const double startingRPT = 3;
        const double startingResearch = 0;
        const int startingHealth = 50;
        int playerHealth, enemyHealth;
        const int startingCardsInHand = 5;
        const int deckPlacementModifier = 23;
        double playerResourcePT, enemyRPT, playerResource, enemyResource, playerResearch, enemyResearch;

        public InGameForm(Deck d, bool start, string e)
        {
            myTurn = start;
            playerResourcePT = startingRPT;
            enemyRPT = startingRPT;
            playerResource = startingResource;
            enemyResource = startingResource;
            enemyResearch = startingResearch;
            playerResearch = startingResearch;
            playerHealth = startingHealth;
            enemyHealth = startingHealth;
            background = Primary.game.inGameBackground;
            enemyUsername = e;
            maxYOffset = -Primary.game.GraphicsDevice.Viewport.Height + Primary.game.inGamePlayAreaTop.Height + Primary.game.inGamePlayAreaBottom.Height;
            CalculateYOffset();
            oldState = Mouse.GetState();
            foreach (DeckItem f in d.mainDeck)
            {
                for (int i = 0; i < f.quantity; i++)
                {
                    deck.Add(f.card);
                }
            }
            foreach (DeckItem f in d.upgrades)
            {
                for (int i = 0; i < f.quantity; i++)
                {
                    upgradeDeck.Add(f.card);
                }
            }
            Shuffle(deck);
            for (int i = 0; i < startingCardsInHand; i++) DrawACard();
            numEnemyCardsInHand = startingCardsInHand;
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            formItems.Add(new BCPlayButton(new Rectangle(((v.Width / 6) - (Primary.game.playButton.Width * 3)) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.playButton.Width, Primary.game.playButton.Height), Primary.game.playButton));
            formItems.Add(new BCAttackButton(new Rectangle(((v.Width / 6) - Primary.game.attackButton.Width) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.attackButton.Width, Primary.game.attackButton.Height), Primary.game.attackButton));
            formItems.Add(new BCDiscardButton(new Rectangle(((v.Width / 6) + Primary.game.discardButton.Width) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.discardButton.Width, Primary.game.discardButton.Height), Primary.game.discardButton));
            if (start) StartTurn();
            else StartEnemyTurn();
        }

        internal void HandleSelection(SmallCard card, Function func)
        {
            switch (func)
            {
                case Function.AddCardToHand:
                    DrawSpecificCard(card.card);
                    break;
                case Function.AddFromEnemyDiscard:
                    enemyDiscardPile.Remove(card.card);
                    hand.Add(card);
                    Primary.game.WriteDataToStream(Protocol.AddToEnemyFromDiscard, card.card.name);
                    break;
                case Function.AntiVehicleArtillery:
                    DealDamageToUnit(card.id, 3, true);
                    Primary.game.WriteDataToStream(Protocol.Artillery);
                    break;
                case Function.ControlUnit:
                    enemyUnits.Remove(card);
                    units.Add(card);
                    UpdateCardPositions();
                    Primary.game.WriteDataToStream(Protocol.ControlUnit, card.id.ToString());
                    break;
                case Function.Counter:
                    AddTechToChain(card.card.name, true);
                    Primary.game.WriteDataToStream(Protocol.PlayTech, card.card.name);
                    break;
                case Function.DeathInHonour:
                    KillUnit(card.id);
                    Primary.game.WriteDataToStream(Protocol.DeathInHonour, card.id.ToString());
#warning need to put the damage component in
                    break;
                case Function.DefendWithUnit:
                    ResolveChainWithDefender(card.id, false);
                    Primary.game.WriteDataToStream(Protocol.DefendWithUnit, card.id.ToString());
                    break;
                case Function.DiscardCard:
                    hand.Remove(card);
                    Primary.game.WriteDataToStream(Protocol.RemoveCardFromEnemyHand, card.card.name);
                    break;
                case Function.EquipUpgrade:
                    AddUpgradeToCard(card.id, true);
                    Primary.game.WriteDataToStream(Protocol.EquipUpgrade, card.id.ToString());
                    break;
                case Function.EquivalentExchange:
                    hand.Remove(card);
                    Primary.game.WriteDataToStream(Protocol.RemoveCardFromEnemyHand, card.card.name);
                    DrawACard();
                    break;
                case Function.HealHalf:
                    HealUnit(card.id, 0.5, true);
                    Primary.game.WriteDataToStream(Protocol.HealHalf, card.id.ToString());
                    break;
                case Function.HealUnit:
                    HealUnit(card.id, 1, true);
                    Primary.game.WriteDataToStream(Protocol.HealFull, card.id.ToString());
                    break;
                case Function.KillUnit:
                    KillUnit(card.id);
                    Primary.game.WriteDataToStream(Protocol.KillUnit, card.id.ToString());
                    break;
                case Function.PlayUnitFromDeck:
                    PlayUnit(card.card, true);
                    deck.Remove(card.card);
                    Primary.game.WriteDataToStream(Protocol.PlayUnitFromDeck, card.card.name);
                    break;
                case Function.PowerExtraction:
                    hand.Remove(card);
                    playerResourcePT++;
                    Primary.game.WriteDataToStream(Protocol.RemoveCardFromEnemyHand, card.card.name);
                    Primary.game.WriteDataToStream(Protocol.PowerExtraction);
                    break;
                case Function.RepairPack:
                    discardPile.Remove(card.card);
                    hand.Add(card);
                    Primary.game.WriteDataToStream(Protocol.AddCardFromDiscard, card.card.name);
                    break;
                case Function.ReplaceUnit:
                    ReplaceUnit(card.id);
                    break;
                case Function.ReturnCardFromDiscard:
                    discardPile.Remove(card.card);
                    hand.Add(card);
                    Primary.game.WriteDataToStream(Protocol.AddCardFromDiscard, card.card.name);
                    break;
                case Function.ReturnUnitToHand:
                    ReturnUnitToHand(card.id, true);
                    Primary.game.WriteDataToStream(Protocol.ReturnUnitToHand, card.id.ToString());
                    break;
            }
        }

        private void ReturnUnitToHand(int id, bool p)
        {
            /*
            foreach (SmallCard c in units)
            {
                if (c == u)
                {
                    if (playerCard)
                    {
                        units.Remove(c);
                        discardPile.Add(Card.getCard(c.card.name));
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, playerCard);
                            playerUpgrades.Remove(upgrade);
                            discardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                    else
                    {
                        enemyUnits.Remove(c);
                        enemyDiscardPile.Add(Card.getCard(c.card.name));
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, playerCard);
                            enemyUpgrades.Remove(upgrade);
                            enemyDiscardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                }
            }
            */
            if (p)
            {
                foreach (SmallCard c in units)
                {
                    if (c.id == id)
                    {
                        units.Remove(c);
                        hand.Add(new SmallCard(Card.getCard(c.card.name), new Vector2(0)));
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, p);
                            playerUpgrades.Remove(upgrade);
                            discardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                foreach (SmallCard c in enemyUnits)
                {
                    if (c.id == id)
                    {
                        enemyUnits.Remove(c);
                        numEnemyCardsInHand++;
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, p);
                            enemyUpgrades.Remove(upgrade);
                            enemyDiscardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                }
            }
        }

        internal void AddCardToEnemyHand(string s)
        {
            throw new NotImplementedException();
#warning this method needs writing
        }

        public void HealUnit(int id, double f, bool p)
        {
            List<SmallCard> list = p ? units : enemyUnits;
            foreach (SmallCard c in list)
            {
                if (c.id == id)
                {

                    HealUnit(c, f);
                    break;
                }
            }
        }
        public void HealUnit(SmallCard c, double factor)
        {

            double maxHealth = Card.getCard(c.card.name).health.Value;
            int change = (int)Math.Ceiling(factor * maxHealth);
            c.card.health = Math.Min((int)maxHealth, c.card.health.Value + change);
        }

        public void DealDamageToUnit(int id, int damage, bool p)
        {
            List<SmallCard> list = p? units : enemyUnits;
            foreach (SmallCard card in list)
            {
                if (card.id == id)
                    card.card.health -= damage;
                if (card.card.health <= 0) KillUnit(card.id);
            }
            
        }

        private void UpdateCardPositions()
        {
            List<SmallCard>[] lists = new List<SmallCard>[] { hand, units, enemyUnits, };
            for (int x = 0; x < lists.Length; x++)
            {
                int y = 0;
                switch (x)
                {
                    case 0:
                        y = GetHandCardY();
                        break;
                    case 1:
                        y = GetUnitCardY();
                        break;
                    case 2:
                        y = GetEnemyUnitCardY();
                        break;
                }
                for (int i = 0; i < lists[x].Count; i++)
                {
                    lists[x][i].UpdateLocation(new Vector2(GetHandCardX(lists[x].Count, i), y));
                }
            }
        }

#warning Draw method
        public override void Draw(SpriteBatch sb)
        {
            //Draw units with upgrades
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            //sb.Draw(background, new Rectangle(0, 0, v.Width, v.Height), Color.White);
            sb.Draw(Primary.game.inGamePlayAreaTop, new Rectangle(v.Width / 6, -yOffset, (v.Width * 2) / 3, Primary.game.inGamePlayAreaTop.Height), Color.White);
            sb.Draw(Primary.game.inGamePlayAreaBottom, new Rectangle(v.Width / 6, Primary.game.inGamePlayAreaTop.Height - yOffset, (v.Width * 2) / 3, Primary.game.inGamePlayAreaBottom.Height), Color.White);
            sb.Draw(Primary.game.playSpace, new Rectangle((v.Width - Primary.game.playSpace.Width) / 2, (Primary.game.inGamePlayAreaBottom.Height + Primary.game.inGamePlayAreaTop.Height) - (yOffset + 50 + Primary.game.playSpace.Height), Primary.game.playSpace.Width, Primary.game.playSpace.Height), Color.White);
            sb.Draw(Primary.game.playSpace, new Rectangle((v.Width - Primary.game.playSpace.Width) / 2, -yOffset + 50, Primary.game.playSpace.Width, Primary.game.playSpace.Height), null, Color.White, (float)Math.PI, new Vector2(Primary.game.playSpace.Width, Primary.game.playSpace.Height), SpriteEffects.None, 1);
            sb.Draw(Primary.game.sideBar, new Rectangle(0, 0, v.Width / 6, v.Height), Color.White);
            sb.Draw(Primary.game.sideBar, new Rectangle((v.Width * 5) / 6, 0, v.Width / 6, v.Height), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.FlipHorizontally, 1);
            foreach (SmallCard c in hand) c.Draw(sb);
            Texture2D cb = Primary.game.cardBack;
            for (int i = 0; i < numEnemyCardsInHand; i++)
                sb.Draw(cb, new Rectangle(GetHandCardX(numEnemyCardsInHand, i), GetEnemyHandCardY(), cb.Width, cb.Height), null, Color.White, (float)Math.PI, new Vector2(cb.Width, cb.Height), SpriteEffects.None, 1);
            foreach (SmallCard c in units) c.Draw(sb, c.tapped ? Orientation.Right : Orientation.Up);
            foreach (SmallCard c in enemyUnits) c.Draw(sb, c.tapped ? Orientation.Left : Orientation.Down);
            for (int i = 0; i < deck.Count; i++)
            {
                int c = Color.White.B - (deck.Count - 1) + i;
                sb.Draw(cb, new Vector2(-deckPlacementModifier - cb.Width + ((v.Width + Primary.game.playSpace.Width) / 2) + i, 9 + GetHandCardY() - i), new Color(c, c, c));
            }

            for (int i = 0; i < upgradeDeck.Count; i++)
            {
                int c = Color.White.B - (upgradeDeck.Count - 1) + i;
                sb.Draw(cb, new Vector2(deckPlacementModifier + ((v.Width - Primary.game.playSpace.Width) / 2) - i, 9 + GetHandCardY() - i), new Color(c, c, c));
            }

            if (cardsInEnemyDeck)
            {
                for (int i = 0; i < 20; i++)
                {
                    int c = Color.White.B - 19 + i;
                    sb.Draw(cb, new Rectangle(deckPlacementModifier + ((v.Width - Primary.game.playSpace.Width) / 2) - i, 12 + GetEnemyHandCardY() - i, cb.Width, cb.Height), null, new Color(c, c, c), (float)Math.PI, new Vector2(cb.Width, cb.Height), SpriteEffects.None, 1);
                }
            }
            if (cardsInEnemyUpgradeDeck)
            {
                for (int i = 0; i < 5; i++)
                {
                    int c = Color.White.B - 4 + i;
                    sb.Draw(cb, new Rectangle(-deckPlacementModifier - Primary.game.cardBack.Width + ((v.Width + Primary.game.playSpace.Width) / 2) + i, 12 + GetEnemyHandCardY() - i, cb.Width, cb.Height), null, new Color(c, c, c), (float)Math.PI, new Vector2(cb.Width, cb.Height), SpriteEffects.None, 1);
                }
            }
            bigCard?.Draw(sb);
            for (int i = 0; i < 3; i++)
            {
                ((TexturedButton)formItems[i]).Draw(sb);
            }
            if (bufferAnimTimer++ >= (numBufferFrames * 10 - 1)) bufferAnimTimer = 0;
            if (chain.Count > 0)
            {
                if (!chain.Last.Value.playerPlayed && !locked)
                {
                    try
                    {
                        foreach (Button b in counterOptionButtons)
                        {
                            b.Draw(sb);
                            b.Update();
                        }
                    }
                    catch
                    {
                        Primary.Log("FF");
                    }
                }
                DrawChain(chain.First, sb);
            }
        }

        internal void OfferAttackCounterOptions()
        {
            Button[] b = getCommonCounterButtons();
            IGSelectButton defendButton = new IGSelectButton(new Rectangle(100, 300, 100, 100));
            Button[] f = new Button[b.Length + 1];
            for (int i = 0; i < b.Length; i++) f[i] = b[i];
            f[f.Length - 1] = defendButton;
            counterOptionButtons = f;
        }

        void DrawChain(LinkedListNode<ChainItem> c, SpriteBatch sb, int ticker = 0)
        {
            try
            {
                sb.DrawString(Primary.game.mainFont, c.Value.ToString(), new Vector2(0, 20 * ticker), Color.Red);
            }
            catch { }
            if (ticker < chain.Count - 1)
            {
                DrawChain(c.Next, sb, ticker);
            }
        }
        public void StartTurn()
        {
            myTurn = true;
            CalculateYOffset();
            DrawACard();
            playerResource += playerResourcePT;
            playerResearch += researchPT;
        }

        internal void AddAttackingUnitToChain(int id, bool enemy)
        {
            SmallCard card = getCardFromId(id);
            chain.AddLast(new ChainItem(card, enemy, false));
        }

        internal void ResolveChainWithDefender(int id, bool attackerPlayerOwned)
        {
            SmallCard defender = getCardFromId(id);
            SmallCard attacker = chain.First.Value.card;
            CalculateCombat(attacker, defender, attackerPlayerOwned);
            chain.Clear();
        }

        internal void AddUpgradeToChain(string s, bool playerPlayed)
        {
            SmallCard smallCard = new SmallCard(Card.getCard(s), GetNextID(), false);
            chain.AddLast(new ChainItem(smallCard, playerPlayed, true));
        }

        internal void OfferCardPlayCounters()
        {
            counterOptionButtons = getCommonCounterButtons();
        }

        internal void DiscardCardFromEnemyHand(string s)
        {
            enemyDiscardPile.Add(Card.getCard(s));
            numEnemyCardsInHand--;
        }

        Button[] getCommonCounterButtons()
        {
            IGCancelButton noSelectionButton = new IGCancelButton(new Rectangle(0, 40, 150, 30));
            IGCounterButton counterButton = new IGCounterButton(new Rectangle(0, 0, 150, 30), "Counter", GetCounterCards(), Function.Counter);
#warning Change this null later
            return new Button[] { noSelectionButton, counterButton };
        }
        public List<SmallCard> GetCounterCards()
        {
            List<SmallCard> output = new List<SmallCard>();
            foreach (SmallCard c in hand)
            {
                if (c.card.type == CardType.Tech)
                {
                    if (c.card.hasEffect("Tech Jammer"))
                    {
                        if (chain.Last.Value.card.card.type == CardType.Tech) output.Add(c);
                    }
                    else if (c.card.hasEffect("Ambush"))
                    {
                        if (chain.Last.Value.card.card.type == CardType.Unit && chain.Last.Value.card.id >= 0) output.Add(c);
                    }
                    else output.Add(c);
                }
            }
            return output;
        }
        Button[] getSelectionButtons(SelectionItem s)
        {
            IGCancelButton noSelectionButton = new IGCancelButton(new Rectangle(0, 40, 150, 30));
            IGSelectButton selectButton = new IGSelectButton(new Rectangle(0, 0, 150, 30), "Select", s);
            return new Button[] { noSelectionButton, selectButton };
        }
        internal void AddTechToChain(string s, bool playerPlayed)
        {
            Card c = Card.getCard(s);
            SmallCard smallCard = new SmallCard(c, GetNextID(), false);
            //bool needsSelection = GetNeedTechSelection(c);
            chain.AddLast(new ChainItem(smallCard, playerPlayed, false));
        }
        private bool GetNeedTechSelection(Card c)
        {
            return EffectSelection.GetTechSelection(c).Equals(null);
        }
        private class EffectSelection
        {
            static EffectSelection[] techSelections = {
                new EffectSelection("Repair Pack", Function.RepairPack, SelectionCondition.playerDiscarded),
                    new EffectSelection("Demilitarisation", Function.ReturnUnitToHand, SelectionCondition.alliedUnit),
                    new EffectSelection("Equivalent Exchange", Function.EquivalentExchange, SelectionCondition.handCard),
                    new EffectSelection("Propoganda", Function.ControlUnit, SelectionCondition.enemyUnit),
                    new EffectSelection("Purify", Function.DiscardCard, SelectionCondition.handCard),
                    new EffectSelection("Repair and Recover", new Selection(2, Function.ReturnCardFromDiscard, true, SelectionCondition.playerDiscarded)),
                    new EffectSelection("Salvage", Function.AddFromEnemyDiscard, new SelectionCondition(true, Location.InEnemyDiscardPile)),
                    new EffectSelection("Power Extraction", Function.PowerExtraction, new SelectionCondition(true, Location.InHand, CardType.Tech)),
                    new EffectSelection("Stimpack", new Selection(1, Function.HealUnit, true, SelectionCondition.alliedUnit, SelectionCondition.notVehicle)),
                    new EffectSelection("Call to Arms", Function.PlayUnitFromDeck, new SelectionCondition(true, Location.InMainDeck, CardType.Unit, 3)),
                    new EffectSelection("Death in Honour", Function.DeathInHonour, new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Leader"))),
                    new EffectSelection("Anti-Vehicle Artillery", Function.AntiVehicleArtillery, SelectionCondition.enemyUnit),
                    new EffectSelection("Repair Vehicle", Function.HealHalf, new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect("Vehicle"))),
                    new EffectSelection("Call the Void", Function.AddCardToHand, new SelectionCondition(true, Location.InMainDeck, CardType.Tech, 1000, null, Effect.GetEffect("Corrupt"))),
            };
            Selection selection;
            Effect effect;
            EffectSelection(string f, Selection s)
            {
                selection = s;
                effect = Effect.GetEffect(f);
            }
            EffectSelection(string f, Function func, SelectionCondition s)
            {
                selection = new Selection(1, func, true, s);
                effect = Effect.GetEffect(f);
            }
            public static Selection GetTechSelection(Card c)
            {
                foreach (EffectSelection s in techSelections)
                {
                    if (c.effects.Contains(s.effect)) return s.selection;
                }
                return null;
            }
        }
        private SelectionItem GetSelectionFromTechCard(Card c)
        {

            Selection s = EffectSelection.GetTechSelection(c);
            if (s.Equals(null)) throw new ArgumentException();
            else return new SelectionItem(s, "Make a selection");
#warning this is probably ok now but might not be
        }
        internal void WaitOnEnemySelection()
        {
            Lock("Waiting on input from the enemy");
        }
        public void CalculateCombat(SmallCard attacker, SmallCard defender, bool attackerPlayerOwned)
        {
            if (attacker.id < 0 || defender.id < 0) throw new ArgumentException("Not good.");
            if (attacker.card.attack == null || defender.card.attack == null || attacker.card.health == null || defender.card.health == null) throw new ArgumentException("Very not good");
            int attackingAtk = attacker.card.attack.Value;
            int defendingAtk = defender.card.attack.Value;
            //Adjust the attack values based on abilities
            attacker.card.health -= defendingAtk;
            defender.card.health -= attackingAtk;
            if (attacker.card.health < 0)
            {
                DiscardUnit(attacker, attackerPlayerOwned);
            }
            if (defender.card.health < 0)
            {
                DiscardUnit(defender, !attackerPlayerOwned);
            }

        }

        internal void PlayUnitFromEnemyDeck(string s)
        {
            PlayUnit(Card.getCard(s), false);
        }

        public void CalculateCombat(Card attacker, bool attackerPlayerOwned)
        {
            int attack = attacker.attack.Value;
            if (attackerPlayerOwned) enemyHealth -= attack;
            else playerHealth -= attack;
#warning add code for when a player wins/loses
            if (enemyHealth <= 0)
            {
                throw new NotImplementedException();
            }
            else if (playerHealth <= 0)
            {
                throw new NotImplementedException();
            }
        }

        public void DiscardUnit(SmallCard u, bool playerCard)
        {
#warning this doesn't work obviously
            if (playerCard)
            {
                foreach (SmallCard c in units)
                {
                    if (c == u)
                    {

                        units.Remove(c);
                        discardPile.Add(Card.getCard(c.card.name));
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, playerCard);
                            playerUpgrades.Remove(upgrade);
                            discardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                foreach (SmallCard c in enemyUnits)
                {
                    if (c == u)
                    {

                        enemyUnits.Remove(c);
                        enemyDiscardPile.Add(Card.getCard(c.card.name));
                        List<int> upgradesToRemove = new List<int>();
                        for (int i = 0; i < upgradesInPlay.Count; i++)
                        {
                            if (upgradesInPlay[i].unitID == c.id)
                            {
                                upgradesToRemove.Add(i);
                            }
                        }
                        foreach (int i in upgradesToRemove)
                        {
                            Upgrade m = upgradesInPlay[i];
                            SmallCard upgrade = GetUpgradeFromID(m.upgradeID, playerCard);
                            enemyUpgrades.Remove(upgrade);
                            enemyDiscardPile.Add(Card.getCard(upgrade.card.name));
                            upgradesInPlay.RemoveAt(i);
                        }
                    }
                }
            }
        }
        int GetNextID()
        {
            return nextID++ - 1;
        }
        internal void AddUpgradeToCard(int id, bool player)
        {
            LinkedListNode<ChainItem> item = chain.Last;
            SmallCard upgrade = item.Value.card;
            if (upgrade.card.type != CardType.Upgrade) throw new InvalidOperationException("very not good");
            upgrade.id = GetNextID();
            if (player) playerUpgrades.Add(upgrade);
            else enemyUpgrades.Add(upgrade);
            upgradesInPlay.Add(new Upgrade(upgrade.id, id));
        }

        internal void KillUnit(int id)
        {
            SmallCard c = getCardFromId(id);
            bool playerOwned;
            if (units.Contains(c)) playerOwned = true;
            else if (enemyUnits.Contains(c)) playerOwned = false;
            else throw new ArgumentException();
            DiscardUnit(c, playerOwned);
        }

        internal void MoveUnitFromEnemy(string s)
        {
            SmallCard c = getCardFromId(Convert.ToInt32(s));
            enemyUnits.Remove(c);
            units.Add(c);
        }
        internal void MoveUnitToEnemy(string s)
        {
            SmallCard c = getCardFromId(Convert.ToInt32(s));
            enemyUnits.Add(c);
            units.Remove(c);
        }

        internal void ReplaceUnit(int id)
        {
            SmallCard card = getCardFromId(id);
            card.card = GetReplacementCard(card.card);
        }
        Card GetReplacementCard(Card c)
        {
            if (c.hasEffect("Ultramarine") && !c.hasEffect("Uncorruptable")) return Card.getCard("Chaos " + c.name);
            else if (c.name == "C'tan Shard of the Nightbringer") return Card.getCard("Transcendent Nightbringer");
            else if (c.name == "C'tan Shard of the Deceiver") return Card.getCard("Transcendent Deceiver");
            else if (c.name == "Transcendent Nightbringer") return Card.getCard("C'tan Shard of the Nightbringer");
            else if (c.name == "Transcendent Deceiver") return Card.getCard("C'tan Shard of the Deceiver");
            else return c;
        }
        public SmallCard GetUpgradeFromID(int id, bool? playerOwned)
        {
            if (playerOwned != true)
            {
                foreach (SmallCard c in enemyUpgrades) if (c.id == id) return c;
            }
            else if (playerOwned != false)
            {
                foreach (SmallCard c in playerUpgrades) if (c.id == id) return c;
            }
            throw new ArgumentException();
        }
        public void StartEnemyTurn()
        {
            myTurn = false;
            yOffset = 0;
            numEnemyCardsInHand += cardsInEnemyDeck ? 1 : 0;
            enemyResource += enemyRPT;
            enemyResearch += researchPT;
        }
        private void CalculateYOffset()
        {
            yOffset = -Primary.game.GraphicsDevice.Viewport.Height + Primary.game.inGamePlayAreaTop.Height + Primary.game.inGamePlayAreaBottom.Height;
        }
#warning Update method
        public override void Update()
        {
            if (!locked)
            {
                UpdateCardPositions();
                UpdateYOffset();
                bigCard?.Update();
                foreach (SmallCard c in hand) c.Update();
                foreach (SmallCard c in units) c.Update();
                foreach (SmallCard c in enemyUnits) c.Update();
                if (bigCard != null && myTurn && chain.Count == 0) UpdateButtonPressable();
                else UpdateButtons(false, false, false);
                if (chain.Count > 0)
                {
                    if (!chain.Last.Value.playerPlayed)
                    {
                        try
                        {
                            foreach (Button b in counterOptionButtons)
                            {
                                b.Update();
                            }
                        }
                        catch
                        {
                            Primary.Log("FF");
                        }
                    }
                }
                base.Update();

            }
        }
        void UpdateButtonPressable()
        {
            int location = GetDrawnCardLocation();
            if (location == 2 || location == 3 || location == 4)
            {
                UpdateButtons(false, false, false);
                return;
            }
            Card card = bigCard.card; //Remember card is not a reference type
            SmallCard smallCard = GetDrawnSmallCard();
            if (smallCard.Equals(null)) throw new InvalidOperationException();
            bool play = false, discard = false, attack = false;
            if (card.hasEffect("Corrupt") && location == 0) discard = true;
            if (card.type == 0 && !smallCard.tapped && myTurn && location == 1) attack = true;
            if (playerResource >= card.cost && units.Count < maxUnitsInPlay && location == 0) play = true;
            UpdateButtons(play, attack, discard);
        }

        internal void ResolveChain()
        {
            LinkedListNode<ChainItem> end = chain.Last;
            ChainItem item = end.Value;
            if (item.state != PlayState.Countered)
            {
                if (item.needSelection)
                {
                    if (item.playerPlayed)
                    {
                        Primary.game.WriteDataToStream(Protocol.EndSelection);//this must be removed before the stuff is done
#warning add selection for the player based on the card- will leave for the moment
                    }
                    else WaitOnEnemySelection();
                    return;
                }
                else
                {
                    switch (item.card.card.type)
                    {
                        case CardType.Unit:
                            if (item.card.id < 0)
                            {
                                PlayUnit(item.card.card, item.playerPlayed);
                            }
                            else
                            {
                                CalculateCombat(item.card.card, item.playerPlayed);
                            }
                            break;
                        case CardType.Tech:
                            ExecuteTech(item.card.card, item.playerPlayed);
                            break;
                        case CardType.Upgrade:
                            throw new InvalidOperationException(); //All upgrades require selection so if it ends up here with an upgrade something's gone very wrong
                    }
                }
            }
            chain.RemoveLast();
            if (chain.Count > 0) ResolveChain();
        }

        private void ExecuteTech(Card card, bool playerPlayed)
        {
            if (!playerPlayed)
            {
                numEnemyCardsInHand--;
            }
#warning not finished yet, doesn't matter just yet though
        }

        private void PlayUnit(Card card, bool playerPlayed)
        {
            if (playerPlayed)
            {
                SmallCard c = new SmallCard(card, new Vector2(GetHandCardX(units.Count, units.Count - 1) - (Primary.game.cardOutlineSmall.Width / 2), GetUnitCardY()));
                units.Add(c);
                c.id = GetNextID();

                UpdateCardPositions();
            }
            else
            {
                SmallCard c = new SmallCard(card, new Vector2(GetHandCardX(units.Count, units.Count - 1) - (Primary.game.cardOutlineSmall.Width / 2), GetEnemyUnitCardY()));
                enemyUnits.Add(c);
                c.id = GetNextID();
                numEnemyCardsInHand--;
            }
        }

        internal void DiscardCardFromEnemyDeck(string s)
        {
            enemyDiscardPile.Add(Card.getCard(s));
        }


        void UpdateButtons(params bool[] b) //should be given as play, attack, discard
        {
            for (int i = 0; i < b.Length; i++)
            {
                ((TexturedButton)formItems[i]).canBePressed = b[i];
            }
        }
        private int GetDrawnCardLocation()
        {
            foreach (SmallCard c in hand)
                if (c.drawnBig) return 0;
            foreach (SmallCard c in units)
                if (c.drawnBig) return 1;
            foreach (SmallCard c in enemyUnits)
                if (c.drawnBig) return 2;
            foreach (SmallCard c in playerUpgrades)
                if (c.drawnBig) return 3;
            foreach (SmallCard c in enemyUpgrades)
                if (c.drawnBig) return 4;
            return -1;
        }
        public SmallCard GetDrawnSmallCard()
        {
            foreach (SmallCard c in hand)
                if (c.drawnBig) return c;
            foreach (SmallCard c in units)
                if (c.drawnBig) return c;
            foreach (SmallCard c in enemyUnits)
                if (c.drawnBig) return c;
            foreach (SmallCard c in playerUpgrades)
                if (c.drawnBig) return c;
            foreach (SmallCard c in enemyUpgrades)
                if (c.drawnBig) return c;
            return null;
        }

        void UpdateYOffset()
        {
            MouseState m = Mouse.GetState();
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            if (m.Y < v.Height / 10 && yOffset - yAccel >= minYOffset) yOffset -= yAccel;
            else if (m.Y > (9 * v.Height) / 10 && yOffset + yAccel <= maxYOffset) yOffset += yAccel;
            UpdateCardPositions();
        }
        public void DiscardSelectedCard()
        {
            foreach (SmallCard c in hand)
            {
                if (c.drawnBig)
                {
                    hand.Remove(c);
                    //discardPile.Add(Card.getCard(c.card.name));
                    c.drawnBig = false;
                    chain.AddLast(new ChainItem(c, true, true));
                    break;
                }
            }
        }
        public void PlaySelectedCard()
        {
            foreach (SmallCard c in hand)
            {
                if (c.drawnBig)
                {
                    hand.Remove(c);
                    c.drawnBig = false;
                    playerResource -= c.card.cost;
                    UpdateCardPositions();
                    switch (c.card.type)
                    {
                        case CardType.Unit:
                            AddPlayersUnitPlayToChain(c);
                            Primary.game.WriteDataToStream(Protocol.PlayUnit, c.card.name);
                            break;
                        case CardType.Tech:
                            AddTechToChain(c.card.name, true);
                            Primary.game.WriteDataToStream(Protocol.PlayTech, c.card.name);
                            break;
                        case CardType.Upgrade:
                            AddUpgradeToChain(c.card.name, true);
                            Primary.game.WriteDataToStream(Protocol.PlayUpgrade, c.card.name);
                            break;

                    }
                    break;
                }
            }
        }
        public int GetHandCardY()
        {
            return (135 + Primary.game.inGamePlayAreaBottom.Height + Primary.game.inGamePlayAreaTop.Height) - (yOffset + Primary.game.playSpace.Height);
        }
        public int GetEnemyHandCardY()
        {
            return 70 - yOffset;
        }
        public int GetUnitCardY()
        {
            return (-35 + Primary.game.inGamePlayAreaBottom.Height + Primary.game.inGamePlayAreaTop.Height) - (yOffset + Primary.game.playSpace.Height);
        }
        public int GetEnemyUnitCardY()
        {
            return 170 - yOffset;
        }
        public void DrawACard()
        {

            hand.Add(new SmallCard(deck[0], new Vector2(GetHandCardX(hand.Count + 1, hand.Count), GetHandCardY())));
            UpdateCardPositions();
            deck.RemoveAt(0);

        }
        public void DrawSpecificCard(Card c)
        {
            hand.Add(new SmallCard(c, new Vector2(GetHandCardX(hand.Count + 1, hand.Count), GetHandCardY())));
            UpdateCardPositions();
            deck.Remove(c);
            Shuffle(deck);
        }
        public SmallCard getCardFromId(int id)
        {
            foreach (SmallCard c in units) if (c.id == id) return c;
            foreach (SmallCard c in enemyUnits) if (c.id == id) return c;
            foreach (SmallCard c in playerUpgrades) if (c.id == id) return c;
            foreach (SmallCard c in enemyUpgrades) if (c.id == id) return c;
            return null;
        }
        public void AddUnitPlayToChain(string s)
        {
            Card f = Card.getCard(s);
            SmallCard c = new SmallCard(f, -1, !f.hasEffect("Assault"));
            chain.AddLast(new ChainItem(c, false, GetSelectionNeeded(f)));
        }
        public void AddPlayersUnitPlayToChain(SmallCard c)
        {
            c.tapped = !c.card.hasEffect("Assault");
            bool needSelection = GetSelectionNeeded(c.card);
            chain.AddLast(new ChainItem(c, true, needSelection));
        }
        public bool GetSelectionNeeded(Card c)
        {
            return !(GetSelectionFromUnit(c) == null);
        }

        public SelectionItem? GetSelectionFromUnit(Card c)
        {
            if (c.hasEffect("Transport"))
            {
                return new SelectionItem(new Selection(1, Function.PlayUnitFromDeck, true, SelectionCondition.transportUnit), "Select a unit to put into play");
            }
            return null;
        }
        public static int GetHandCardX(int numCards, int ord)
        {
            if (numCards <= 5)
            {
                int leftX = (Primary.game.GraphicsDevice.Viewport.Width - (numCards * (Primary.game.cardOutlineSmall.Width + 2))) / 2;
                return leftX + (ord * (Primary.game.cardOutlineSmall.Width + 2));
            }
            else
            {
                int leftX = (Primary.game.GraphicsDevice.Viewport.Width - (5 * (Primary.game.cardOutlineSmall.Width + 2))) / 2;
                int rightX = leftX + (4 * (Primary.game.cardOutlineSmall.Width + 2));
                int diff = rightX - leftX;
                return leftX + (ord * (diff / numCards));
            }
        }
        public static void Shuffle(List<Card> c)
        {
            List<ShuffleItem> items = new List<ShuffleItem>();
            foreach (Card d in c) items.Add(new ShuffleItem(d));
            c.Clear();
            items.Sort();
            foreach (ShuffleItem i in items) c.Add((Card)i);

        }
        public void ClearCounterOptions()
        {
            counterOptionButtons = new Button[0];
        }

        internal List<SmallCard> GetValidCardsAtLocation(Location l, SelectionCondition[] conditions, bool allConditions)
        {
            List<SmallCard> output = new List<SmallCard>();
            List<Card> tempCardStore = new List<Card>();
            List<SmallCard> tempSmallCardStore = new List<SmallCard>();
            switch (l)
            {
                case Location.InHand:
                    tempSmallCardStore = hand;
                    break;
                case Location.InDiscardPile:
                    tempCardStore = discardPile;
                    break;
                case Location.InEnemyDiscardPile:
                    tempCardStore = enemyDiscardPile;
                    break;
                case Location.InEnemyUnits:
                    tempSmallCardStore = enemyUnits;
                    break;
                case Location.InMainDeck:
                    tempCardStore = deck;
                    break;
                case Location.InUnits:
                    tempSmallCardStore = units;
                    break;
                case Location.InUpgradeDeck:
                    tempCardStore = upgradeDeck;
                    break;

            }
            foreach (Card c in tempCardStore) tempSmallCardStore.Add(new SmallCard(c, -1, false));
            foreach (SmallCard c in tempSmallCardStore)
            {
                bool fulfilled = allConditions;
                foreach (SelectionCondition s in conditions)
                {
                    //remember the s.fulfil
                    bool cost = c.card.cost <= s.maxCost;
                    bool effects = true;
                    foreach (Effect e in s.requiredEffects)
                    {
                        if (!c.card.effects.Contains(e)) effects = false;
                    }
                    bool type = c.card.type == s.type;
                    bool total = cost && effects && type;
                    if (allConditions)
                        fulfilled = (total == s.fulfil) && fulfilled;
                    else fulfilled = (total == s.fulfil) || fulfilled;
                }
                if (fulfilled) output.Add(c);
            }
            return output;
        }

        struct ShuffleItem : IComparable
        {
            Card item;
            int num;
            public ShuffleItem(Card i)
            {
                item = i;
                num = Primary.game.rng.Next();
            }
            public int CompareTo(object obj)
            {
                return num.CompareTo(((ShuffleItem)obj).num);
            }
            public static explicit operator Card(ShuffleItem f) => f.item;
        }
    }
    struct Upgrade
    {
        public int upgradeID, unitID;
        public Upgrade(int up, int un)
        {
            upgradeID = up;
            unitID = un;
        }
    }
}
