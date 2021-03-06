﻿namespace MenuTest
{
    using GH.Menu;
    using GH.Menu.Containers.Line;
    using GH.Menu.Containers.Menus;
    using GH.Menu.Containers.Page;
    using GH.Menu.Objects;
    using GH.Menu.Objects.Button;

    using Lua;

    public class ButtonsTest
    {
        public ButtonsTest(IMenuHandler handler)
        {
            var menu = handler.CreateMenu(
                new MenuProfile("Buttons test", 400, () => { Core.print("OK"); }, true, () => Core.print("Show"), null, null)
                {
                    title = "Buttons tests",
                    icon = "Interface\\Icons\\Ability_Creature_Cursed_04",
                    [0] = new PageProfile()
                    {
                        new LineProfile()
                        {
                            new ButtonProfile()
                            {
                                align = ObjectAlign.l,
                                text = "Default",
                                onClick = (() =>
                                {
                                    Core.print("Click");
                                }),
                                tooltip = "Default profile values"
                            },
                            new ButtonProfile()
                            {
                                width = 80,
                                height = 20,
                                align = ObjectAlign.c,
                                text = "Normal",
                                onClick = (() =>
                                {
                                    Core.print("Click");
                                }),
                                tooltip = "Normal with given width and height"
                            },
                            new ButtonProfile()
                            {
                                width = 80,
                                height = 20,
                                align = ObjectAlign.r,
                                text = "Compact",
                                compact = true,
                                onClick = (() =>
                                {
                                    Core.print("Click");
                                }),
                                tooltip = "Compact button"
                            }
                        }
                    }
                }
                );

            menu.AnimatedShow();
        }
    }
}