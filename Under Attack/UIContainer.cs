using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UnderAttack
{
    public class UIContainer
    {
        private Dictionary<string, UIElement> _elementMap = null;
        private List<Texture2D> _texMap = null;
        private Color _tint = new Color(1f, 1f, 1f, 1f);
        private Texture2D _backgroundTex = null;
        private Rectangle _bounds = new Rectangle(0, 0, 0, 0);
        private Rectangle _bufferRect = new Rectangle(0, 0, 0, 0);
        private SpriteFont _font = null;
        private Vector2 _textLoc = new Vector2(0, 0);
        private Vector2 _textSize = new Vector2(0, 0);
        public UIContainer(int x, int y,int width, int height)
        {
            _bounds.X = x;
            _bounds.Y = y;
            _bounds.Width = width;
            _bounds.Height = height;


            _texMap = new List<Texture2D>();
            _elementMap = new Dictionary<string, UIElement>();
        }

        public UIContainer(int x, int y,int width, int height,Texture2D background)
            : this (x,y,width,height)
        {
            _backgroundTex = background;
        }

        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public int AddTexture(Texture2D tex)
        {
            _texMap.Add(tex);
            return _texMap.Count - 1;
        }

        public void AddElement(UIElement element)
        {
            _elementMap.Add(element.Name, element);
        }

        public UIElement GetElement(string name)
        {
            return _elementMap[name];            
        }

        public void RemoveElement(string name)
        {
            _elementMap.Remove(name);
        }

        public void Update(MouseState _state)
        {
            Point loc = new Point(_state.X, _state.Y);
            foreach (UIElement e in _elementMap.Values)
            {
                if (e.Bounds.Contains(loc))
                {
                    if (_state.LeftButton == ButtonState.Pressed)
                        e.ElementState = UIElementState.Pressed;
                    else
                        e.ElementState = UIElementState.Hover;
                }
                else
                    e.ElementState = UIElementState.None;
            }
        }

        public void Render(SpriteBatch batch, byte alpha)
        {
            _tint.A = alpha;
         
            if (_backgroundTex != null)
                batch.Draw(_backgroundTex, _bounds, _tint);
            
            foreach (UIElement e in _elementMap.Values)
            {
                if (e.Visible)
                {
                    _bufferRect.Width = e.Width;
                    _bufferRect.Height = e.Height;
                    _bufferRect.X = _bounds.X + e.Bounds.X;
                    _bufferRect.Y = _bounds.Y + e.Bounds.Y;

                    _textSize = Font.MeasureString(e.Text);
                    _textLoc.X = ( _bufferRect.Width - _textSize.X)/2 + _bufferRect.X;
                    _textLoc.Y = (_bufferRect.Height - _textSize.Y) / 2 + _bufferRect.Y;

                    batch.Draw(_texMap[e.ActiveSprite], _bufferRect, _tint);
                   
                    batch.DrawString(Font, e.Text, _textLoc, _tint);
                }
            }
        }
    }
}
