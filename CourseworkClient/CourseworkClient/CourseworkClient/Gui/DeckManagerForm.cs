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
        public DeckManagerForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 100, 50), "Main Menu", FormChangeButtonTypes.DeckManagerToMainMenu));
            decks = Deck.decks;
            currentDeck = Primary.game.selectedDeckNum;
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
        }

    }
}
