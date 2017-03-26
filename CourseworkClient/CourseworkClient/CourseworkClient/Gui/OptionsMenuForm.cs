using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CourseworkClient.Gui
{
    class OptionsMenuForm : Form
    {
        public NumericalTextField[] resolutionFields;
        public OptionsMenuForm()
        {
            background = Primary.game.mainMenuBackground;
            formItems.Add(new FormChangeButton(new Rectangle(0, 0, 200, 50), "Back", FormChangeButtonTypes.OptionsToMainMenu));
            formItems.Add(new FullScreenButton(new Rectangle(300, 300, 300, 50)));
            formItems.Add(new UpdateResolutionButton(new Rectangle(350 + Primary.game.textFieldTexture.Width, 500, 300, 50)));
            resolutionFields = new NumericalTextField[] {
                new NumericalTextField(new Rectangle(300, 450, Primary.game.textFieldTexture.Width, Primary.game.textFieldTexture.Height), 4, "Resolution X"),
                new NumericalTextField(new Rectangle(300, 500 + Primary.game.textFieldTexture.Height, Primary.game.textFieldTexture.Width, Primary.game.textFieldTexture.Height), 4, "Resolution Y"),
            };
            resolutionFields[0].text = Primary.game.GraphicsDevice.Viewport.Width.ToString();
            resolutionFields[1].text = Primary.game.GraphicsDevice.Viewport.Height.ToString();
        }
        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            foreach (NumericalTextField t in resolutionFields) t.Draw(sb);
        }

        public override void Update()
        {
            base.Update();
            foreach (NumericalTextField t in resolutionFields) t.Update();
            if ((resolutionFields[0].selected || resolutionFields[1].selected) && Primary.game.keypresshandler.HasKeyBeenPressed(Keys.Tab))
            {
                foreach (NumericalTextField t in resolutionFields) t.selected = !t.selected;
            }
        }
    }
}
