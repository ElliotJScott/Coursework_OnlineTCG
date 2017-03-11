using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// The state of an item in the chain
    /// </summary>
    enum PlayState
    {
        Success,
        Countered,
        NotExecuted,
    }
    /// <summary>
    /// The location of a given card
    /// </summary>
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
    /// <summary>
    /// The different things that can be done with a card selected from the SelectionForm
    /// </summary>
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
    /// <summary>
    /// An item in the chain
    /// </summary>
    class ChainItem
    {
        public SmallCard card;
        public PlayState state;
        public bool playerPlayed;
        public bool needSelection;
        /// <summary>
        /// Creates a new item to add to the chain
        /// </summary>
        /// <param name="c">The card that is performing some action</param>
        /// <param name="plPlayed">Whether or not the player played the card</param>
        /// <param name="select">Whether the card requires any selection</param>
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
            if (card.id > 0 && card.id < 10000)
            {
                return string.Format("{0} attacked with {1}", username, card.card.name);
            }
            else return string.Format("{0} played {1}", username, card.card.name);
        }
    }
    /// <summary>
    /// A condition to filter down cards that can be selected
    /// </summary>
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
            function = func;
            fulfilAll = f;
            quantity = num;
            conditions = sel;
        }
        /// <summary>
        /// Gets all valid cards for the selection
        /// </summary>
        /// <returns>A list of all valid cards</returns>
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
    /// <summary>
    /// A selection with accompanying text to display
    /// </summary>
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
        int turnNum; //This is the number of turns that the player has taken. Doesn't count enemy turns
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
        public List<Card> enemyDiscardPile = new List<Card>();
        public List<SmallCard> enemyUnits = new List<SmallCard>();
        public List<SmallCard> playerUpgrades = new List<SmallCard>();
        public List<SmallCard> enemyUpgrades = new List<SmallCard>();
        public LinkedList<ChainItem> chain = new LinkedList<ChainItem>();
        public List<Upgrade> upgradesInPlay = new List<Upgrade>();
        const int maxUnitsInPlay = 10;
        int yOffset;
        public string enemyUsername;
        const int minYOffset = 0;
        string[] races = {"Ultramarine", "Chaos Space Marine", "Eldar", "Necron", "Tyranid"};
        readonly int maxYOffset;
        const int yAccel = 20;
        int nextID = 0;
        int upgradesDrawn = 0;
        int nextHandID = 10000;
        const double researchPT = 0.3;
        const double startingResource = 3;
        const double startingRPT = 3;
        const double startingResearch = 0;
        const int startingHealth = 50;
        int playerHealth, enemyHealth;
        const int startingCardsInHand = 5;
        const int deckPlacementModifier = 23;
        public double playerResourcePT, enemyRPT, playerResource, enemyResource, playerResearch, enemyResearch;
        EndTurnButton endTurnButton;
        /// <summary>
        /// Creates a new InGameForm
        /// </summary>
        /// <param name="d">The deck that the player should use</param>
        /// <param name="start">Whether the player starts first</param>
        /// <param name="e">The enemy's username</param>
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
            endTurnButton = new EndTurnButton(new Rectangle(v.Width - Primary.game.buttonTexture.Width, 0, Primary.game.buttonTexture.Width, Primary.game.buttonTexture.Height));
            formItems.Add(new BCPlayButton(new Rectangle(((v.Width / 6) - (Primary.game.playButton.Width * 3)) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.playButton.Width, Primary.game.playButton.Height), Primary.game.playButton));
            formItems.Add(new BCAttackButton(new Rectangle(((v.Width / 6) - Primary.game.attackButton.Width) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.attackButton.Width, Primary.game.attackButton.Height), Primary.game.attackButton));
            formItems.Add(new BCDiscardButton(new Rectangle(((v.Width / 6) + Primary.game.discardButton.Width) / 2, 10 + (Primary.game.cardOutlineBig.Height + v.Height / 2), Primary.game.discardButton.Width, Primary.game.discardButton.Height), Primary.game.discardButton));
            if (start) StartTurn();
            else StartEnemyTurn();
        }
        /// <summary>
        /// Performs the appropriate action for a selection
        /// </summary>
        /// <param name="card">The card selected</param>
        /// <param name="func">The function that should be performed</param>
        internal void HandleSelection(SmallCard card, Function func)
        {
            switch (func)
            {
                case Function.AddCardToHand:
                    DrawSpecificCard(card.card);
                    Primary.game.WriteDataToStream(Protocol.AddCardToEnemyHand);
                    break;
                case Function.AddFromEnemyDiscard:
                    enemyDiscardPile.Remove(card.card);
                    card.id = GetNextHandID();
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
#warning need to put the damage component in before the unit is killed
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
                    if (deck.Count == 0)
                    {
                        Primary.game.WriteDataToStream(Protocol.NoCardsInDeck);
                    }
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
                    card.id = GetNextHandID();
                    hand.Add(card);
                    Primary.game.WriteDataToStream(Protocol.AddCardFromDiscard, card.card.name);
                    break;
                case Function.ReplaceUnit:
                    ReplaceUnit(card.id);
                    break;
                case Function.ReturnCardFromDiscard:
                    discardPile.Remove(card.card);
                    card.id = GetNextHandID();
                    hand.Add(card);
                    Primary.game.WriteDataToStream(Protocol.AddCardFromDiscard, card.card.name);
                    break;
                case Function.ReturnUnitToHand:
                    ReturnUnitToHand(card.id, true);
                    Primary.game.WriteDataToStream(Protocol.ReturnUnitToHand, card.id.ToString());
                    break;
            }
            chain.RemoveLast();
            if (chain.Count > 0) ResolveChain();
        }
        /// <summary>
        /// Returns a unit to the owner's hand
        /// </summary>
        /// <param name="id">The id of the unit</param>
        /// <param name="p">Whether the player played the unit</param>
        public void ReturnUnitToHand(int id, bool p)
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
                        SmallCard f = new SmallCard(Card.getCard(c.card.name), new Vector2(0));
                        f.id = GetNextHandID();
                        hand.Add(f);
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
        /// <summary>
        /// Heals a unit
        /// </summary>
        /// <param name="id">The id of the unit</param>
        /// <param name="f">The amount to heal the unit by</param>
        /// <param name="p">Whether the player played the unit</param>
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
        /// <summary>
        /// Adds a card from a discard pile to the enemy's hand
        /// </summary>
        /// <param name="s">The card to add</param>
        /// <param name="p">Whether the player's discard pile will be used</param>
        internal void AddCardToEnemyHand(string s, bool p)
        {
#warning this might be fine
            List<Card> list = !p ? discardPile : enemyDiscardPile;
            foreach (Card c in list)
            {
                if (c == Card.getCard(s))
                {
                    list.Remove(c);
                    numEnemyCardsInHand++;

                }
            }
        }
        /// <summary>
        /// Heals a unit as a fraction of its max health
        /// </summary>
        /// <param name="c">The card to heal</param>
        /// <param name="factor">The factor by which to heal the unit</param>
        public void HealUnit(SmallCard c, double factor)
        {

            double maxHealth = Card.getCard(c.card.name).health.Value;
            int change = (int)Math.Ceiling(factor * maxHealth);
            c.card.health = Math.Min((int)maxHealth, c.card.health.Value + change);
        }
        /// <summary>
        /// Damages the given unit
        /// </summary>
        /// <param name="id">The unit to damage</param>
        /// <param name="damage">The amount to damage the unit by</param>
        /// <param name="p">Whether the unit is owned by the player</param>
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
        /// <summary>
        /// Ensures all cards in play and in hands are drawn in the correct positions
        /// </summary>
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
            foreach (SmallCard c in units) c.Draw(sb, true);
            foreach (SmallCard c in enemyUnits) c.Draw(sb, false);
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

                }
                DrawChain(chain.First, sb);
            }
            else
            {
                if (myTurn) endTurnButton.Draw(sb);
            }
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
            DrawPlayerData(sb);
        }

        public void DrawPlayerData(SpriteBatch sb)
        {
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            int sw = v.Width;
            int sh = v.Height;
            sb.DrawString(Primary.game.mainFont, string.Format("Your ({0}) current resources : {1}\nResources Per Turn : {2}", Primary.game.username, playerResource, playerResourcePT), new Vector2(sw - 200, 100), Color.Red);
            sb.DrawString(Primary.game.mainFont, string.Format("The enemy's ({0}) current resources : {1}\nResources Per Turn : {2}", enemyUsername, enemyResource, enemyRPT), new Vector2(sw - 200, 200), Color.Red);
            sb.DrawString(Primary.game.mainFont, string.Format("Your ({0}) current research : {1}\nResearch Per Turn : {2}", Primary.game.username, playerResearch, researchPT), new Vector2(sw - 200, 300), Color.Red);
            sb.DrawString(Primary.game.mainFont, string.Format("The enemy's ({0}) current research : {1}\nResearch Per Turn : {2}", enemyUsername, enemyResearch, researchPT), new Vector2(sw - 200, 400), Color.Red);
        }
        /// <summary>
        /// Adds the appropriate buttons for the player to counter an enemy's attack appropriately
        /// </summary>
        internal void OfferAttackCounterOptions()
        {
            Button[] b = getCommonCounterButtons();
            IGSelectButton defendButton = new IGSelectButton(new Rectangle(100, 300, 100, 100));
            Button[] f = new Button[b.Length + 1];
            for (int i = 0; i < b.Length; i++) f[i] = b[i];
            f[f.Length - 1] = defendButton;
            counterOptionButtons = f;
        }
        /// <summary>
        /// Draws the chain in text to screen. Note that I use recursion here.
        /// </summary>
        /// <param name="c">The current node to draw</param>
        /// <param name="sb">The SpriteBatch to draw with</param>
        /// <param name="ticker">How many nodes have been drawn</param>
        void DrawChain(LinkedListNode<ChainItem> c, SpriteBatch sb, int ticker = 0)
        {
            try
            {
                sb.DrawString(Primary.game.mainFont, c.Value.ToString(), new Vector2(0, 20 * ticker), Color.Red);
            }
            catch { }
            if (ticker < chain.Count - 1)
            {
                DrawChain(c.Next, sb, ticker + 1);
            }
        }
        /// <summary>
        /// Performs the actions when the player's turn begins
        /// </summary>
        public void StartTurn()
        {
            myTurn = true;
            turnNum++;
            CalculateYOffset();
            DrawACard();
            playerResource += playerResourcePT;
            playerResearch += researchPT;
            if (haveSufficientResearch())
            {
                DrawUpgradeCard();
            }
        }
        public bool haveSufficientResearch()
        {
            double researchRequired = 12d * Math.Exp((upgradesDrawn / 8d) - (turnNum / 5d));
            return playerResearch >= researchRequired;
        }
        public void DrawUpgradeCard()
        {
            Shuffle(upgradeDeck);
            if (upgradeDeck.Count > 0)
            {
                SmallCard c = new SmallCard(upgradeDeck[0], new Vector2(GetHandCardX(hand.Count + 1, hand.Count), GetHandCardY()));
                c.id = GetNextHandID();
                hand.Add(c);
                UpdateCardPositions();
                upgradeDeck.RemoveAt(0);
                if (upgradeDeck.Count == 0)
                {
                    Primary.game.WriteDataToStream(Protocol.NoCardsInUpgradeDeck);
                }
            }
        }
        /// <summary>
        /// Adds an attacking unit to the end of the chain
        /// </summary>
        /// <param name="id">The id of the unit</param>
        /// <param name="enemy">Whether it is the enemy's unit attacking</param>
        internal void AddAttackingUnitToChain(int id, bool enemy)
        {
            SmallCard card = getCardFromId(id);
            chain.AddLast(new ChainItem(card, enemy, false));
        }
        /// <summary>
        /// Resolves the chain using a defending unit
        /// </summary>
        /// <param name="id">The unit of the defending unit</param>
        /// <param name="attackerPlayerOwned">Whether the player controls the attacking unit</param>
        internal void ResolveChainWithDefender(int id, bool attackerPlayerOwned)
        {
            SmallCard defender = getCardFromId(id);
            SmallCard attacker = chain.First.Value.card;
            CalculateCombat(attacker, defender, attackerPlayerOwned);
            chain.Clear();
        }
        /// <summary>
        /// Adds the playing of an upgrade to the chain
        /// </summary>
        /// <param name="s">The name of the upgrade being played</param>
        /// <param name="playerPlayed">Whether the player played the upgrade</param>
        internal void AddUpgradeToChain(string s, bool playerPlayed)
        {
            SmallCard smallCard = new SmallCard(Card.getCard(s), GetNextID(), false);
            chain.AddLast(new ChainItem(smallCard, playerPlayed, true));
        }
        /// <summary>
        /// Adds the buttons for countering without the option to defend to the screen
        /// </summary>
        internal void OfferCardPlayCounters()
        {
            counterOptionButtons = getCommonCounterButtons();
        }
        /// <summary>
        /// Discards a card from the enemy hand
        /// </summary>
        /// <param name="s">The name of the card to discard</param>
        internal void DiscardCardFromEnemyHand(string s)
        {
            enemyDiscardPile.Add(Card.getCard(s));
            numEnemyCardsInHand--;
        }
        /// <summary>
        /// Gets the buttons for countering
        /// </summary>
        /// <returns>An array of the buttons for countering</returns>
        Button[] getCommonCounterButtons()
        {
            IGCancelButton noSelectionButton = new IGCancelButton(new Rectangle(0, 40, 150, 30));
            IGCounterButton counterButton = new IGCounterButton(new Rectangle(0, 0, 150, 30), "Counter", GetCounterCards(), Function.Counter);
            return new Button[] { noSelectionButton, counterButton };
        }

        /// <summary>
        /// Gets all cards that could be used as counters
        /// </summary>
        /// <returns>A list of all cards that could be used as counters</returns>
        public List<SmallCard> GetCounterCards()
        {
            List<SmallCard> output = new List<SmallCard>();
            List<int> idsInChain = new List<int>();
            foreach (ChainItem c in chain)
            {
                idsInChain.Add(c.card.id);
            }
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
                    else if (c.card.cost <= playerResource && !idsInChain.Contains(c.id)) output.Add(c);
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the buttons for making a selection
        /// </summary>
        /// <param name="s">The selection that would be made</param>
        /// <returns>An array of the buttons for selection</returns>
        Button[] getSelectionButtons(SelectionItem s)
        {
            IGCancelButton noSelectionButton = new IGCancelButton(new Rectangle(0, 40, 150, 30));
            IGSelectButton selectButton = new IGSelectButton(new Rectangle(0, 0, 150, 30), "Select", s);
            return new Button[] { noSelectionButton, selectButton };
        }
        /// <summary>
        /// Adds the playing of a tech card to the end of the chain
        /// </summary>
        /// <param name="s"></param>
        /// <param name="playerPlayed"></param>
        internal void AddTechToChain(string s, bool playerPlayed)
        {
#warning change this later
            Card c = Card.getCard(s);
            SmallCard smallCard = new SmallCard(c, GetNextID(), false);
            bool needsSelection = GetNeedTechSelection(c); 
            chain.AddLast(new ChainItem(smallCard, playerPlayed, needsSelection));
        }
        /// <summary>
        /// Gets whether a given card needs a selection when it is played
        /// </summary>
        /// <param name="c">The card to check</param>
        /// <returns>Whether the card needs selection</returns>
        private bool GetNeedTechSelection(Card c)
        {
            return EffectSelection.GetNeedsTechSelection(c);
        }
        /// <summary>
        /// A mapping of the selections that different effects need and what selections they make
        /// </summary>
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
            public static bool GetNeedsTechSelection(Card c)
            {
                foreach (EffectSelection s in techSelections)
                {
                    if (c.effects.Contains(s.effect)) return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Gets the selection item that a given tech card need
        /// </summary>
        /// <param name="c">The tech card</param>
        /// <returns>The appropriate SelectionItem</returns>
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
        /// <summary>
        /// Processes combat between an attacker and a defender
        /// </summary>
        /// <param name="attacker">The attacking unit</param>
        /// <param name="defender">The defending unit</param>
        /// <param name="attackerPlayerOwned">Whether the attacking unit is controlled by the player</param>
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
        /// <summary>
        /// Plays a unit for the enemy from their deck
        /// </summary>
        /// <param name="s">The name of the unit to play</param>
        internal void PlayUnitFromEnemyDeck(string s)
        {
            PlayUnit(Card.getCard(s), false);
        }
        /// <summary>
        /// Calculates the combat for an attacker with no defending unit
        /// </summary>
        /// <param name="attacker">The attacking unit</param>
        /// <param name="attackerPlayerOwned">Whether the unit is controlled by the player</param>
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
        /// <summary>
        /// Discards a given unit from the field
        /// </summary>
        /// <param name="u">The unit to discard</param>
        /// <param name="playerCard">Whether the unit is controlled by the player</param>
        public void DiscardUnit(SmallCard u, bool playerCard)
        {
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
        /// <summary>
        /// Gets the next available ID for a unit or upgrade
        /// </summary>
        /// <returns>The next ID</returns>
        int GetNextID()
        {
            return nextID++ - 1;
        }
        /// <summary>
        /// Adds an upgrade from the chain to the correct unit
        /// </summary>
        /// <param name="id">The unit to add the upgrade to</param>
        /// <param name="player">Whether the upgrade/unit is player controlled</param>
        internal void AddUpgradeToCard(int id, bool player)
        {
#warning need to add the effect from the upgrade to the unit it is equipped to
            LinkedListNode<ChainItem> item = chain.Last;
            SmallCard upgrade = item.Value.card;
            if (upgrade.card.type != CardType.Upgrade) throw new InvalidOperationException("very not good"); //If this gets called it means that the game is trying to equip a non-upgrade card
            upgrade.id = GetNextID();
            if (player) playerUpgrades.Add(upgrade);
            else enemyUpgrades.Add(upgrade);
            upgradesInPlay.Add(new Upgrade(upgrade.id, id));
            AddUpgradeEffectsToCard(getCardFromId(id), upgrade);
        }
        public void AddUpgradeEffectsToCard(SmallCard unit, SmallCard upgrade)
        {
            string[] effectBlacklist = { "Ultramarine Upgrade", "Chaos Space Marine Upgrade", "Eldar Upgrade", "Necron Upgrade", "Tyranid Upgrade", "Semi-Unique", "Unique"};
            //foreach (string s in upgrade.card.getEffectNames())
            //{
            //    if (!unit.card.hasEffect(s))
            //    {
            //        bool 
            //        foreach (string x in effectBlacklist)
            //        {
            //            if (x == s) break;
            //        }
            //    }
            //}
            List<string> effects = upgrade.card.getEffectNames();
            foreach (string s in effectBlacklist) effects.Remove(s);
            foreach (string s in effects) if (!unit.card.hasEffect(s)) unit.AddEffect(Effect.GetEffect(s));
        }
        /// <summary>
        /// Kills a given unit
        /// </summary>
        /// <param name="id">The id of the unit to kill</param>
        internal void KillUnit(int id)
        {
            SmallCard c = getCardFromId(id);
            bool playerOwned;
            if (units.Contains(c)) playerOwned = true;
            else if (enemyUnits.Contains(c)) playerOwned = false;
            else throw new ArgumentException();
            DiscardUnit(c, playerOwned);
        }
        /// <summary>
        /// Moves a unit from the enemy to the player
        /// </summary>
        /// <param name="s">The id to move (as a string)</param>
        internal void MoveUnitFromEnemy(string s)
        {
            SmallCard c = getCardFromId(Convert.ToInt32(s));
            enemyUnits.Remove(c);
            units.Add(c);
        }
        /// <summary>
        /// Moves a unit to the enemy to the player
        /// </summary>
        /// <param name="s">The id to move (as a string)</param>
        internal void MoveUnitToEnemy(string s)
        {
            SmallCard c = getCardFromId(Convert.ToInt32(s));
            enemyUnits.Add(c);
            units.Remove(c);
        }
        /// <summary>
        /// Replaces a given unit
        /// </summary>
        /// <param name="id">The id of the unit to replace</param>
        internal void ReplaceUnit(int id)
        {
            SmallCard card = getCardFromId(id);
            card.card = GetReplacementCard(card.card);
        }
        /// <summary>
        /// Gets the card that a given card should be replaced with
        /// </summary>
        /// <param name="c">The card to be replaced</param>
        /// <returns>The card to replace it with</returns>
        Card GetReplacementCard(Card c)
        {
            if (c.hasEffect("Ultramarine") && !c.hasEffect("Uncorruptable")) return Card.getCard("Chaos " + c.name);
            else if (c.name == "C'tan Shard of the Nightbringer") return Card.getCard("Transcendent Nightbringer");
            else if (c.name == "C'tan Shard of the Deceiver") return Card.getCard("Transcendent Deceiver");
            else if (c.name == "Transcendent Nightbringer") return Card.getCard("C'tan Shard of the Nightbringer");
            else if (c.name == "Transcendent Deceiver") return Card.getCard("C'tan Shard of the Deceiver");
            else return c;
        }
        /// <summary>
        /// Gets the upgrade with a given id
        /// </summary>
        /// <param name="id">The id of the upgrade</param>
        /// <param name="playerOwned">Whether the upgrade is controlled by the player. If it is not know null will check both.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Performs the actions for starting the enemy's turn
        /// </summary>
        public void StartEnemyTurn()
        {
            myTurn = false;
            yOffset = 0;
            numEnemyCardsInHand += cardsInEnemyDeck ? 1 : 0;
            enemyResource += enemyRPT;
            enemyResearch += researchPT;
        }
        public void DiscardCardFromHand(Card c)
        {
            foreach (SmallCard f in hand)
            {
                if (f.card == c)
                {
                    hand.Remove(f);
                    discardPile.Add(c);
                        
                }
            }
        }
        /// <summary>
        /// Calculates the initial Y-offset of form items for when the game begins
        /// </summary>
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
                        catch (Exception e)
                        {
                            Primary.Log(e);
                        }
                    }
                }
                else
                {
                    if (myTurn) endTurnButton.Update();
                }
                base.Update();

            }
        }
        /// <summary>
        /// Updates whether the buttons to perform actions with the selected card can be pressed
        /// </summary>
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
#warning ResolveChain method
        /// <summary>
        /// Resolves the last item in the chain
        /// </summary>
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
                        switch (item.card.card.type)
                        {
                            case CardType.Unit:
                                counterOptionButtons = getSelectionButtons(GetSelectionFromUnit(item.card.card).Value);
                                //This here applies only to the transport ability
                                break;
                            case CardType.Tech:
                                counterOptionButtons = getSelectionButtons(GetSelectionFromTechCard(item.card.card));
                                break;
                            case CardType.Upgrade:
                                counterOptionButtons = getSelectionButtons(GetSelectionFromUpgrade(item.card.card));
                                break;
                        }
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
                            if (item.card.id < 0 || item.card.id >= 10000)
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

        private SelectionItem GetSelectionFromUpgrade(Card card)
        {

            string effectName = "";
            foreach (string s in races)
            {
                if (card.hasEffect(s))
                {
                    effectName = s + " Upgrade";
                    break;
                }
            }
            if (effectName == "") throw new ArgumentException();
            return new SelectionItem(new Selection(1, Function.EquipUpgrade, true, new SelectionCondition(true, Location.InUnits, CardType.Unit, 1000, null, Effect.GetEffect(effectName))), "Select a unit to equip " + card.name + " to.");
        }

        /// <summary>
        /// Executes a given tech card
        /// </summary>
        /// <param name="card">The tech card to execute</param>
        /// <param name="playerPlayed">Whether the card is controlled by the player or not</param>
        private void ExecuteTech(Card card, bool playerPlayed)
        {
            if (!playerPlayed)
            {
                numEnemyCardsInHand--;
            }
            else DiscardCardFromHand(card);
#warning not finished yet, doesn't matter just yet though
        }
        /// <summary>
        /// Plays a given unit
        /// </summary>
        /// <param name="card">The unit to play</param>
        /// <param name="playerPlayed">Whether the player played the unit</param>
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
        /// <summary>
        /// Discards a given card from the enemy's deck
        /// </summary>
        /// <param name="s">The name of the card to discard</param>
        internal void DiscardCardFromEnemyDeck(string s)
        {
            enemyDiscardPile.Add(Card.getCard(s));
        }


        /// <summary>
        /// Updates the buttons for choosing what to do with the currently selected card
        /// </summary>
        /// <param name="b">The buttons to update</param>
        void UpdateButtons(params bool[] b) //should be given as play, attack, discard
        {
            for (int i = 0; i < b.Length; i++)
            {
                ((TexturedButton)formItems[i]).canBePressed = b[i];
            }
        }
        /// <summary>
        /// Gets the location of the currently selected card
        /// </summary>
        /// <returns>The location of the currently draw card (Probably should change this to use the Location enum)</returns>
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
        /// <summary>
        /// Gets the currently drawn card
        /// </summary>
        /// <returns>The currently drawn card</returns>
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
        /// <summary>
        /// Updates the Y-Offset based on the position of the mouse
        /// </summary>
        void UpdateYOffset()
        {
            MouseState m = Mouse.GetState();
            Viewport v = Primary.game.GraphicsDevice.Viewport;
            if (m.Y < v.Height / 10 && yOffset - yAccel >= minYOffset) yOffset -= yAccel;
            else if (m.Y > (9 * v.Height) / 10 && yOffset + yAccel <= maxYOffset) yOffset += yAccel;
            UpdateCardPositions();
        }
        /// <summary>
        /// Adds to the chain to discard the currently selected card
        /// </summary>
        public void DiscardSelectedCard()
        {
#warning need to modify this
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
        /// <summary>
        /// Adds to the chain to play the currently selected card and transmits it
        /// </summary>
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
        /// <summary>
        /// Gets the Y position that cards in the player's hand should be drawn on screen
        /// </summary>
        /// <returns>The Y position</returns>
        public int GetHandCardY()
        {
            return (135 + Primary.game.inGamePlayAreaBottom.Height + Primary.game.inGamePlayAreaTop.Height) - (yOffset + Primary.game.playSpace.Height);
        }
        /// <summary>
        /// Gets the Y position that cards in the enemy's hand should be drawn on screen
        /// </summary>
        /// <returns>The Y position</returns>
        public int GetEnemyHandCardY()
        {
            return 70 - yOffset;
        }
        /// <summary>
        /// Gets the Y position that the player's units should be drawn at
        /// </summary>
        /// <returns>The Y position</returns>
        public int GetUnitCardY()
        {
            return (-35 + Primary.game.inGamePlayAreaBottom.Height + Primary.game.inGamePlayAreaTop.Height) - (yOffset + Primary.game.playSpace.Height);
        }
        /// <summary>
        /// Gets the Y position that the enemy's units should be drawn at
        /// </summary>
        /// <returns>The Y position</returns>
        public int GetEnemyUnitCardY()
        {
            return 170 - yOffset;
        }
        /// <summary>
        /// Draws a card from the player's deck
        /// </summary>
        public void DrawACard()
        {
            if (deck.Count > 0)
            {
                SmallCard c = new SmallCard(deck[0], new Vector2(GetHandCardX(hand.Count + 1, hand.Count), GetHandCardY()));
                c.id = GetNextHandID();
                hand.Add(c);
                UpdateCardPositions();
                deck.RemoveAt(0);
                if (deck.Count == 0)
                {
                    Primary.game.WriteDataToStream(Protocol.NoCardsInDeck);
                }
            }
        }
        
        /// <summary>
        /// Gets the next SmallCard id for a card in the player's hand
        /// </summary>
        /// <returns>The next id</returns>
        public int GetNextHandID() => nextHandID++ - 1;
        
        /// <summary>
        /// Draws a specific card from the deck
        /// </summary>
        /// <param name="c">The card to draw</param>
        public void DrawSpecificCard(Card c)
        {
            SmallCard f = new SmallCard(c, new Vector2(GetHandCardX(hand.Count + 1, hand.Count), GetHandCardY()));
            f.id = GetNextHandID();
            hand.Add(f);
            UpdateCardPositions();
            deck.Remove(c);
            if (deck.Count == 0)
            {
                Primary.game.WriteDataToStream(Protocol.NoCardsInDeck);
            }
            Shuffle(deck);
        }
        /// <summary>
        /// Gets the card with the given id
        /// </summary>
        /// <param name="id">The id of the card to get</param>
        /// <returns>The card</returns>
        public SmallCard getCardFromId(int id)
        {
            foreach (SmallCard c in units) if (c.id == id) return c;
            foreach (SmallCard c in enemyUnits) if (c.id == id) return c;
            foreach (SmallCard c in playerUpgrades) if (c.id == id) return c;
            foreach (SmallCard c in enemyUpgrades) if (c.id == id) return c;
            return null;
        }
        /// <summary>
        /// Adds the play of an enemy unit to the chain
        /// </summary>
        /// <param name="s">The name of the card to add</param>
        public void AddUnitPlayToChain(string s)
        {
            Card f = Card.getCard(s);
            SmallCard c = new SmallCard(f, -1, !f.hasEffect("Assault"));
            chain.AddLast(new ChainItem(c, false, GetSelectionNeeded(f)));
        }
        /// <summary>
        /// Adds the play of a player's unit to the chain
        /// </summary>
        /// <param name="c">The unit to play</param>
        public void AddPlayersUnitPlayToChain(SmallCard c)
        {
            c.tapped = !c.card.hasEffect("Assault");
            bool needSelection = GetSelectionNeeded(c.card);
            chain.AddLast(new ChainItem(c, true, needSelection));
        }
        /// <summary>
        /// Gets whether a given card requires selection
        /// </summary>
        /// <param name="c">The card to check</param>
        /// <returns>Whether the card needs selection</returns>
        public bool GetSelectionNeeded(Card c)
        {
            return !(GetSelectionFromUnit(c) == null);
        }
        /// <summary>
        /// Gets what selection a given unit needs
        /// </summary>
        /// <param name="c">The card to check</param>
        /// <returns>What selection the card needs</returns>
        public SelectionItem? GetSelectionFromUnit(Card c)
        {
            if (c.hasEffect("Transport"))
            {
                return new SelectionItem(new Selection(1, Function.PlayUnitFromDeck, true, SelectionCondition.transportUnit), "Select a unit to put into play");
            }
            return null;
        }
        /// <summary>
        /// Gets the X Position on the screen for a card in the hand
        /// </summary>
        /// <param name="numCards">The number of cards in the hand</param>
        /// <param name="ord">The position of the card in the hand</param>
        /// <returns>The X Position</returns>
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
        /// <summary>
        /// Shuffles a list of cards (The deck)
        /// </summary>
        /// <param name="c">The list to shuffle</param>
        public static void Shuffle(List<Card> c)
        {
            List<ShuffleItem> items = new List<ShuffleItem>();
            foreach (Card d in c) items.Add(new ShuffleItem(d));
            c.Clear();
            items.Sort();
            foreach (ShuffleItem i in items) c.Add((Card)i);

        }
        /// <summary>
        /// Removes any counter buttons
        /// </summary>
        public void ClearCounterOptions()
        {
            counterOptionButtons = new Button[0];
        }
        /// <summary>
        /// Gets the valid cards for some conditions at a location
        /// </summary>
        /// <param name="l">The location to check</param>
        /// <param name="conditions">The conditions to fulfil</param>
        /// <param name="allConditions">Whether all conditions need to be fulfilled or just one</param>
        /// <returns></returns>
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
        /// <summary>
        /// An item used to shuffle the deck
        /// </summary>
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
    /// <summary>
    /// Defines an upgrade in play and what unit it is equipped to
    /// </summary>
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
