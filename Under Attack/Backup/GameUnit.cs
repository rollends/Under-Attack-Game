using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace UnderAttack
{
    class GameUnit
    {  
        private String spriteName = "";

        private Vector2 position = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;

        private Rectangle bounds = Rectangle.Empty;
        private Vector2 size = Vector2.One;

        private int currentFrame = 0;
        private bool animating = false;

        private int cooldown = 0;

        public GameUnit( String spriteName, Vector2 initPos, Vector2 size )
        {
            this.spriteName = spriteName;
            this.position = initPos;

            this.size = size;
            
            UpdateBounds();
        }

        #region Attributes

        public int CoolDown
        {
            get
            {
                return cooldown;
            }
            set
            {
                cooldown = value;
            }
        }

        public String SpriteName
        {
            get
            {
                return this.spriteName;
            }
            set
            {
                this.spriteName = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
                UpdateBounds();
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                this.velocity = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        public bool IsAnimating
        {
            get
            {
                return this.animating;
            }
            set
            {
                this.animating = value;
            }
        }

        public int CurrentFrame
        {
            get
            {
                return this.currentFrame;
            }
            set
            {
                this.currentFrame = value;
            }
        }

        #endregion

        #region Methods

        public void UpdateBounds( )
        {
            this.bounds.X = (int)Math.Round(this.position.X - (this.size.X/2));
            this.bounds.Y = (int)Math.Round(this.position.Y - (this.size.Y/2));
            this.bounds.Width = (int)Math.Round(this.size.X);
            this.bounds.Height = (int)Math.Round(this.size.Y);
        }
 
        public bool IsCollidingWith( GameUnit unit )
        {
            return (bounds.Intersects(unit.bounds));
        }

        #endregion

    }
}
