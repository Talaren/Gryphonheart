﻿
namespace GH.Model.Defaults
{
    using BlizzardApi.Global;
    using CsLua.Collection;
    using GH.Integration;
    using GH.ObjectHandling;

    public static class DefaultQuickButtons
    {
        private static CsLuaList<IQuickButton> list = new CsLuaList<IQuickButton>()
        {
            new QuickButton(
                "helm", 1, true, "Toggle helm", "Interface\\Icons\\INV_Helmet_03",
                () =>
                {
                    Global.Api.ShowHelm(!Global.Api.ShowingHelm());
                }, 
                AddOnReference.None
            ),
            new QuickButton(
                "cloak", 2, true, "Toggle cloak", "Interface\\Icons\\INV_Misc_Cape_18",
                () =>
                {
                    Global.Api.ShowCloak(!Global.Api.ShowingCloak());
                },
                AddOnReference.None
            ),
            new QuickButton(
                "news", 10, true, "AddOn Features and News", "Interface\\Icons\\INV_Misc_Horn_04",
                () =>
                {

                },
                AddOnReference.None
            ),
            new QuickButton(
                "options", 20, true, "Options", "Interface\\Icons\\INV_MISC_QUESTIONMARK",
                () =>
                {

                },
                AddOnReference.None
            ),
        };

        public static void RegisterDefaultButton(IQuickButton button)
        {
            list.Add(button);
        }

        public static void AddToModel(IIdObjectListWithDefaults<IQuickButton, string> buttonList)
        {
            foreach (var quickButton in list)
            {
                buttonList.SetDefault(quickButton);
            }
        }
    }
}
