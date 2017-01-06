using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    public interface IFormItem
    {
        void Draw(SpriteBatch sb);
        void Update();
    }
}