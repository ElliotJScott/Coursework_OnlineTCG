using Microsoft.Xna.Framework.Graphics;

namespace CourseworkClient.Gui
{
    /// <summary>
    /// The base of all gui items/forms used in the project
    /// </summary>
    public interface IFormItem
    {
        /// <summary>
        /// Draw the gui item/form. This should be called every frame.
        /// </summary>
        /// <param name="sb">The SpriteBatch used to draw the FormItem</param>
        void Draw(SpriteBatch sb);
        /// <summary>
        /// Update the state of the item. This should be called every frame.
        /// </summary>
        void Update();
    }
}