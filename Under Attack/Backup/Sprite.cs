using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UnderAttack
{
    class Sprite
    {
        private Texture2D tex = null;
        private int frameCount = 1;
        private Rectangle frameSize = Rectangle.Empty;

        public Sprite( Texture2D texture, Rectangle frameSize, int numFrames )
        {
            this.tex = texture;
            this.frameSize = frameSize;
            this.frameCount = numFrames;
        }

        #region Attributes

        public Texture2D Texture
        {
            get
            {
                return this.tex;
            }
        }

        public int FrameCount
        {
            get
            {
                return this.frameCount;
            }
        }

        public Rectangle FrameSize
        {
            get
            {
                return this.frameSize;
            }
        }

        #endregion

        #region Methods

        public Rectangle GetFrame( int frameNum )
        {
            Rectangle rect = new Rectangle();

            rect.X = frameNum * this.frameSize.Width;
            rect.Y = 0;
            rect.Width = this.frameSize.Width;
            rect.Height = this.frameSize.Height;

            return rect;
        }

        #endregion

    }
}
