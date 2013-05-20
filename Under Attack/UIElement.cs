using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UnderAttack
{
    public class UIElement
    {
        private Dictionary<UIElementState,int> _spriteMap = null;
        private Rectangle _bounds;
        private UIElementState _activeState;
        private int _activeSprite;
        private string _text = "";
        private string _name = "";
        private string _command = "";
        private bool _visible = true;

        public event EventHandler<UIElementPressedEventArgs> RaiseUIElementPressedEvent;
        public event EventHandler<UIElementReleasedEventArgs> RaiseUIElementReleasedEvent;

        public UIElement(string elementName,Rectangle bounds,Dictionary<UIElementState, int> spriteMap, string text)
        {
            _bounds = bounds;
            _name = elementName;
            _text = text;
            if (!spriteMap.Keys.Contains(UIElementState.None))
                throw new NotImplementedException("The UIElement must have a sprite for the default state!");

            _spriteMap = spriteMap;
            SetActiveState(UIElementState.None);
        }

        public string Name
        {
            get { return _name; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string Command
        {
            get { return _command; }
            set { _command = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public int ActiveSprite
        {
            get { return _activeSprite; }
        }

        public Rectangle Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        public int Width
        {
            get { return _bounds.Width; }
            set { _bounds.Width = value; }
        }

        public int Height
        {
            get { return _bounds.Height; }
            set { _bounds.Height = value; }
        }

        public int X
        {
            get { return _bounds.X; }
            set { _bounds.X = value; }
        }

        public int Y
        {
            get { return _bounds.Y; }
            set { _bounds.Y = value; }
        }

        public UIElementState ElementState
        {
            get { return _activeState; }
            set { SetActiveState(value); }
        }

        protected virtual void OnRaiseUIElementPressedEvent(UIElementPressedEventArgs e)
        {
            EventHandler<UIElementPressedEventArgs> handler = RaiseUIElementPressedEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnRaiseUIElementReleasedEvent(UIElementReleasedEventArgs e)
        {
            EventHandler<UIElementReleasedEventArgs> handler = RaiseUIElementReleasedEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void SetActiveState(UIElementState newState)
        {
            if (newState == UIElementState.Pressed && _activeState != UIElementState.Pressed)
            {
                OnRaiseUIElementPressedEvent(new UIElementPressedEventArgs(this, _command));
            }
            else if (newState != UIElementState.Pressed && _activeState == UIElementState.Pressed)
            {
                OnRaiseUIElementReleasedEvent(new UIElementReleasedEventArgs(this, _command));
            }

            _activeState = newState;

            if (_spriteMap.Keys.Contains(newState))
                _activeSprite = _spriteMap[newState];
            else
                _activeSprite = _spriteMap[UIElementState.None];
        }

    }

    public class UIElementPressedEventArgs : EventArgs
    {
        private UIElement _element = null;
        private string _command = "";

        public UIElementPressedEventArgs(UIElement element, string cmd)
        {
            _element = element;
            _command = cmd;
        }

        public UIElement Element
        {
            get { return _element; }
        }

        public string Command
        {
            get { return _command; }
        }
    }
    public class UIElementReleasedEventArgs : EventArgs
    {
        private UIElement _element = null;
        private string _command = "";

        public UIElementReleasedEventArgs(UIElement element, string cmd)
        {
            _element = element;
            _command = cmd;
        }

        public UIElement Element
        {
            get { return _element; }
        }

        public string Command
        {
            get { return _command; }
        }
    }
}
