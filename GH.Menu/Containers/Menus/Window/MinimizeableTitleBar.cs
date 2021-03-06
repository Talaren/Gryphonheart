﻿namespace GH.Menu.Containers.Menus.Window
{
    using System;

    using BlizzardApi.Global;
    using BlizzardApi.WidgetEnums;
    using BlizzardApi.WidgetInterfaces;

    public class MinimizeableTitleBar : TitleBar
    {
        private const string MinimizeTexturePath = "Interface\\Buttons\\UI-Panel-HideButton";
        private const string RestoreTexturePath = "Interface\\Buttons\\UI-Panel-ExpandButton";

        private readonly ContentContainer attachedFrame;
        private readonly IButton minimizeButton;
        private readonly IButton restoreButton;

        public MinimizeableTitleBar(ContentContainer attachedFrame) : base(attachedFrame)
        {
            this.attachedFrame = attachedFrame;
            this.minimizeButton = CreateButton(this.GetFrame(), MinimizeTexturePath, this.Minimize);
            this.restoreButton = CreateButton(this.GetFrame(), RestoreTexturePath, this.Restore);
            this.restoreButton.Hide();
        }

        public void Restore(IUIObject self, object a, object b)
        {
            this.minimizeButton.Show();
            this.restoreButton.Hide();
            this.attachedFrame.Show();
        }

        public void Minimize(IUIObject self, object a, object b)
        {
            this.restoreButton.Show();
            this.minimizeButton.Hide();
            this.attachedFrame.Hide();
        }

        private static IButton CreateButton(IFrame parent, string texture, Action<IUIObject, object, object> click)
        {
            var button = (IButton)Global.FrameProvider.CreateFrame(FrameType.Button, null, parent, "UIPanelCloseButton");
            button.SetPoint(FramePoint.RIGHT, parent, FramePoint.RIGHT, -BarHeight/2 - BorderSize/4, 0);
            button.SetHeight(BarHeight);
            button.SetWidth(BarHeight);
            button.SetNormalTexture(texture + "-Up");
            button.SetPushedTexture(texture + "-Down");
            button.SetScript(ButtonHandler.OnClick, click);
            return button;
        }
    }
}