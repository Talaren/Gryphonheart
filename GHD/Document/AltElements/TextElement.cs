﻿namespace GHD.Document.AltElements
{
    using System;
    using System.Collections.Generic;
    using BlizzardApi.Global;
    using BlizzardApi.WidgetEnums;
    using GHD.Document.Elements;
    using GHD.Document.Flags;
    using Lua;

    public class TextElement : IElement
    {
        private readonly IFlags flags;
        private string text;
        private int insertPosition;
        private FormattedTextFrame frame;

        public TextElement(IFlags flags, string text)
        {
            this.flags = flags;
            this.text = text;
            this.insertPosition = 0;
            this.frame = new FormattedTextFrame(flags);
            this.frame.Region.SetPoint(FramePoint.TOPLEFT, Global.Frames.UIParent, FramePoint.TOPLEFT); // TODO: Do in the group
            this.frame.Region.Show();
        }

        public void Insert(IFlags newFlags, string newText)
        {
            if (!this.flags.Equals(newFlags))
            {
                this.InsertNewTextElement(newFlags, newText);
            }
            else
            {
                this.InsertTextAtInsertPosition(newText);
            }
        }

        private void InsertTextAtInsertPosition(string newText)
        {
            this.text = this.text.Substring(0, this.insertPosition) + newText + this.text.Substring(this.insertPosition);
            this.TextChanged();
            this.insertPosition += Strings.strlenutf8(newText);
        }

        private void InsertNewTextElement(IFlags newFlags, string newText)
        {
            if (this.insertPosition == 0)
            {
                this.InsertNewTextElementBeforeThis(newFlags, newText);
            }
            else if (this.insertPosition == Strings.strlenutf8(this.text))
            {
                this.InsertNewTextElementAfterThis(newFlags, newText);
            }
            else
            {
                this.InsertNewTextElementAtInsertPosition(newFlags, newText);
            }
        }

        private void InsertNewTextElementBeforeThis(IFlags newFlags, string newText)
        {
            var newElement = new TextElement(newFlags, newText)
            {
                Next = this,
                Prev = this.Prev,
                Group = this.Group
            };
            this.Prev.Next = newElement;
            this.Prev = newElement;
        }

        private void InsertNewTextElementAfterThis(IFlags newFlags, string newText)
        {
            var newElement = new TextElement(newFlags, newText)
            {
                Next = this.Next,
                Prev = this,
                Group = this.Group,
            };
            this.Next.Prev = newElement;
            this.Next = newElement;
        }

        private void InsertNewTextElementAtInsertPosition(IFlags newFlags, string newText)
        {
            var textBeforeInsertPosition = this.text.Substring(0, this.insertPosition);
            var textAfterInsertPosition = this.text.Substring(this.insertPosition);
            this.text = textBeforeInsertPosition;
            this.InsertNewTextElementAfterThis(newFlags, newText);
            ((TextElement)this.Next).InsertNewTextElementAfterThis(this.flags, textAfterInsertPosition);
            this.TextChanged();
        }

        private void TextChanged()
        {
            this.frame.SetText(this.text, 100); //  TODO: Get width
        }

        public IElement Prev { get; set; }

        public IElement Next { get; set; }

        public HorizontalGroup Group { get; set; }

        public IFlags Flags => this.flags;
    }
}