using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace UnderAttack
{
    class Player : GameUnit 
    {
        private int hp = 100;
        private float score = 0;

        public Player( String spriteName, Vector2 initPos, Vector2 size ) : base ( spriteName, initPos, size )
        {

        }

        #region Attributes

        public int HP
        {
            get
            {
                return this.hp;
            }
            set
            {
                this.hp = value;
            }
        }

        public float Score
        {
            get
            {
                return this.score;
            }
            set
            {
                this.score = value;
            }
        }

        public long IntegerScore
        {
            get
            {
                return (long)Math.Ceiling(score);
            }
        }

        #endregion

    }
}
