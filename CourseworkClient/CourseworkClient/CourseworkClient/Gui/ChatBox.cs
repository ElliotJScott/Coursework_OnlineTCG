using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CourseworkClient.Gui
{
    class ChatBox : GuiItem
    {
        List<string> chatMessages = new List<string>();
        int position;
        public Friend selectedFriend
        {
            set
            {
                selectedFriend = value;
                chatMessages = value.chatMessages;
                position = value.chatMessages.Count < 10 ? 0 : value.chatMessages.Count;
            }
        }
        public ChatBox(Rectangle rect)
        {
            boundingBox = rect;
            subItems.Add(new TextField(new Rectangle(rect.X, (rect.Y  + rect.Height) - Primary.game.textFieldTexture.Height, Primary.game.textFieldTexture.Width, Primary.game.textFieldTexture.Width), 30));
        }
        public override void Draw(SpriteBatch sb)
        {
            //throw new NotImplementedException();
        }

        public override void Update()
        {
            //throw new NotImplementedException();
        }
    }
}
