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
        public List<DeckCardItem> allCardItems = new List<DeckCardItem>();
        public List<DeckCardItem> deckCardItems = new List<DeckCardItem>();
        public DeckManagerForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 100, 50), "Main Menu", FormChangeButtonTypes.DeckManagerToMainMenu));
            formItems.Add(new SaveDeckButton(new Rectangle(0, 60, 100, 50), "Save"));
            for (int i = 0; i < numDeckSlots; i++)
            {
                formItems.Add(new DeckButton(new Rectangle(Primary.game.GraphicsDevice.Viewport.Width - 100, i * 50, 100, 50), i, decks.Count));
            }
            decks = Deck.decks;
            currentDeck = Primary.game.selectedDeckNum;
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
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (List<DeckCardItem> d in new List<DeckCardItem>[] { allCardItems, deckCardItems })
                for (int i = d.Count - 1; i >= 0; i--)
                    d[i].Draw(sb);


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
            foreach (Deck d in decks)
            {
                if (d.dbID < 0)
                {
                    Primary.game.WriteDataToStream(Protocol.NewDeckCards, d.dbID + "|" + d.mainDeck[0].card.name + "|" + d.mainDeck[0].quantity);
                    return;
                }
                else
                {
                    foreach (List<DeckItem> l in new List<DeckItem>[] { d.mainDeck, d.upgrades })
                    {
                        foreach (DeckItem r in l)
                        {
                            Primary.game.WriteDataToStream(Protocol.NewDeckCards, d.dbID + "|" + r.card.name + "|" + r.quantity);
                        }
                    }
                }
                Unlock();

            }

        }
        public void UpdateDeckCardItems()
        {
            allCardItems.Clear();
            deckCardItems.Clear();
            int startingAllCardIndex = allCardPageNumber * allCardsAcross * cardsDown;
            int startingDeckIndex = deckPageNumber * deckCardsAcross * cardsDown;
            {
                bool upgrade = false;
                List<DeckItem> source = Deck.allOwnedCards.mainDeck;
                for (int i = 0; i < allCardsAcross; i++)
                {
                    for (int j = 0; j < cardsDown; j++)
                    {

                        int index = j + (cardsDown * i);
                        if (upgrade) index -= Deck.allOwnedCards.mainDeck.Count;
                        if (index >= source.Count)
                        {
                            index = 0;
                            source = Deck.allOwnedCards.upgrades;
                            upgrade = true;
                        }
                        allCardItems.Add(new DeckCardItem(source[index].card, new Vector2(120 + (i * 220), 25 + (j * 320)), true));
                        if (upgrade && index == source.Count - 1) break;
                    }
                    if (allCardItems.Count == Deck.allOwnedCards.mainDeck.Count + Deck.allOwnedCards.upgrades.Count) break;
                }
            }
            {
                for (int i = 0; i < deckCardsAcross; i++)
                {
                    bool upgrade = false;
                    List<DeckItem> source = decks[currentDeck].mainDeck;
                    for (int j = 0; j < cardsDown; j++)
                    {

                        int index = j + (cardsDown * i);
                        if (upgrade) index -= decks[currentDeck].mainDeck.Count;
                        if (index > source.Count || source.Count == 0)
                        {
                            index = 0;
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
                    if (deckCardItems.Count == decks[currentDeck].mainDeck.Count + decks[currentDeck].upgrades.Count) break;
                }
            }
        }
    }
}
