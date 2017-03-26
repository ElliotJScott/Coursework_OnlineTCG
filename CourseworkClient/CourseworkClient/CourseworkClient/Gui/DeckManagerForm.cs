using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    class DeckManagerForm : Form
    {
        public List<Deck> decks = new List<Deck>();
        public int currentDeck;
        public int allCardPageNumber = 0;
        public int deckPageNumber = 0;
        public const int allCardsAcross = 4;
        public const int cardsDown = 2;
        public const int deckCardsAcross = 2;
        public const int numDeckSlots = 8;
        const int arrowScale = 1;
        public List<DeckCardItem> allCardItems = new List<DeckCardItem>();
        public List<DeckCardItem> deckCardItems = new List<DeckCardItem>();
        public ScrollArrow[] arrows;

        public DeckManagerForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 100, 50), "Main Menu", FormChangeButtonTypes.DeckManagerToMainMenu));
            formItems.Add(new SaveDeckButton(new Rectangle(0, 60, 100, 50), "Save"));
            formItems.Add(new CurrentDeckButton(new Rectangle(0, 120, 100, 50)));
            foreach (Deck d in Deck.decks)
            {
                Deck m = new Deck(d.dbID);
                foreach (DeckItem r in d.mainDeck)
                    m.mainDeck.Add(new DeckItem(r.card, r.quantity));
                foreach (DeckItem r in d.upgrades)
                    m.upgrades.Add(new DeckItem(r.card, r.quantity));
                decks.Add(m);
            }
            for (int i = 0; i < numDeckSlots; i++)
            {
                formItems.Add(new DeckButton(new Rectangle(Primary.game.GraphicsDevice.Viewport.Width - 100, i * 50, 100, 50), i, decks.Count));
            }
            currentDeck = Primary.game.selectedDeckNum;
            arrows = new ScrollArrow[] {
                new ScrollArrow(new Rectangle(150, 700, arrowScale*Primary.game.greenArrowTexture.Width, arrowScale*Primary.game.greenArrowTexture.Height), Orientation.Left),
                new ScrollArrow(new Rectangle((220 * allCardsAcross) + 25, 700, arrowScale*Primary.game.greenArrowTexture.Width, arrowScale*Primary.game.greenArrowTexture.Height), Orientation.Right),
                new ScrollArrow(new Rectangle((220 * allCardsAcross) + 175, 700, arrowScale*Primary.game.greenArrowTexture.Width, arrowScale*Primary.game.greenArrowTexture.Height), Orientation.Left),
                new ScrollArrow(new Rectangle((220 * (allCardsAcross + deckCardsAcross)) + 175, 700, arrowScale*Primary.game.greenArrowTexture.Width, arrowScale*Primary.game.greenArrowTexture.Height), Orientation.Right),
            };
            
        }
        public override void Update()
        {
            //UpdateDeckCardItems();
            base.Update();
            foreach (List<DeckCardItem> d in new List<DeckCardItem>[] { allCardItems, deckCardItems })
            {

                for (int m = 0; m < d.Count; m++)
                {
                    int x = d.Count;
                    d[m].Update();
                }
            }
            if (arrows[0].Clicked())
            {
                if (allCardPageNumber > 0) allCardPageNumber--;
                UpdateDeckCardItems();
            }
            else if (arrows[1].Clicked())
            {
                int r = (allCardPageNumber + 1) * allCardsAcross * cardsDown;
                int p = Deck.allOwnedCards.mainDeck.Count + Deck.allOwnedCards.upgrades.Count;
                if (r < p) allCardPageNumber++;
                UpdateDeckCardItems();
            }
            else if (arrows[2].Clicked())
            {
                if (deckPageNumber > 0) deckPageNumber--;
                UpdateDeckCardItems();
            }
            else if (arrows[3].Clicked())
            {
                int r = (deckPageNumber + 1) * deckCardsAcross * cardsDown;
                int p = decks[currentDeck].mainDeck.Count + decks[currentDeck].upgrades.Count;
                if (r < p) deckPageNumber++;
                UpdateDeckCardItems();
            }
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (List<DeckCardItem> d in new List<DeckCardItem>[] { allCardItems, deckCardItems })
                for (int i = d.Count - 1; i >= 0; i--)
                    d[i].Draw(sb);
            foreach (ScrollArrow r in arrows)
                r.Draw(sb);
            sb.DrawString(Primary.game.mainFont, string.Format("Cards in main deck : {0}\nCards in upgrade deck : {1}", decks[currentDeck].GetDeckCount(true), decks[currentDeck].GetDeckCount(false)), new Vector2(1100, 650), Color.White);


        }
        public void TransmitDecks(int initID = -1)
        {
            int initNum = 0;
            if (initID >= 0)
            {
                for (int i = 0; i < decks.Count; i++)
                {
                    if (decks[i].dbID == initID)
                    {
                        initNum = i;
                        break;
                    }
                }
            }
            for (int i = initNum; i < decks.Count; i++)
            {
                if (decks[i].dbID < 0)
                {
                    Primary.game.WriteDataToStream(Protocol.NewDeckCards, decks[i].dbID + "|" + decks[i].mainDeck[0].card.name + "|" + decks[i].mainDeck[0].quantity);
                    return;
                }
                else
                {
                    foreach (List<DeckItem> l in new List<DeckItem>[] { decks[i].mainDeck, decks[i].upgrades })
                    {
                        foreach (DeckItem r in l)
                        {
                            Primary.game.WriteDataToStream(Protocol.NewDeckCards, decks[i].dbID + "|" + r.card.name + "|" + r.quantity);
                        }
                    }
                }

            }
            Unlock();

        }
        public void UpdateDeckCardItems()
        {
            allCardItems.Clear();
            deckCardItems.Clear();
            foreach (Deck d in decks)
            {
                d.mainDeck.Sort();
                d.upgrades.Sort();
                Deck.allOwnedCards.mainDeck.Sort();
                Deck.allOwnedCards.upgrades.Sort();
            }
            int startingAllCardIndex = allCardPageNumber * allCardsAcross * cardsDown;
            int startingDeckIndex = deckPageNumber * deckCardsAcross * cardsDown;
            {
                bool upgrade = false;
                List<DeckItem> source = Deck.allOwnedCards.mainDeck;
                for (int i = 0; i < allCardsAcross; i++)
                {
                    for (int j = 0; j < cardsDown; j++)
                    {

                        int index = j + (cardsDown * i) + startingAllCardIndex;
                        if (upgrade) index -= Deck.allOwnedCards.mainDeck.Count;
                        if (index >= source.Count)
                        {
                            index -= source.Count;
                            source = Deck.allOwnedCards.upgrades;
                            upgrade = true;
                        }
                        allCardItems.Add(new DeckCardItem(source[index].card, new Vector2(120 + (i * 220), 25 + (j * 320)), true));
                        if (upgrade && index == source.Count - 1) break;
                    }
                    if (allCardItems.Count + startingAllCardIndex == Deck.allOwnedCards.mainDeck.Count + Deck.allOwnedCards.upgrades.Count) break;
                }
            }
            {
                for (int i = 0; i < deckCardsAcross; i++)
                {
                    bool upgrade = false;
                    List<DeckItem> source = decks[currentDeck].mainDeck;
                    for (int j = 0; j < cardsDown; j++)
                    {

                        int index = j + (cardsDown * i) + startingDeckIndex;
                        if (upgrade) index -= decks[currentDeck].mainDeck.Count;
                        if (index >= source.Count || source.Count == 0)
                        {
                            index = Math.Max(index - source.Count, 0);
                            source = decks[currentDeck].upgrades;
                            upgrade = true;

                        }
                        try
                        {
                            deckCardItems.Add(new DeckCardItem(source[index].card, new Vector2(160 + ((i + allCardsAcross) * 220), 25 + (j * 320)), false));
                        }
                        catch { }
                        if (upgrade && index == source.Count - 1) break;
                    }
                    if (deckCardItems.Count + startingDeckIndex == decks[currentDeck].mainDeck.Count + decks[currentDeck].upgrades.Count) break;
                }
            }
        }
    }
}
