using System;
using System.Collections.Generic;
using Cashly.Core;
using Toolbox;
using Toolbox.UI;

namespace Cashly;

internal static class MenuActions
{
    public static bool SelectProfile()
    {
        List<UserProfile> profiles;
        if (!UserProfile.EnumProfiles(profiles: out profiles))
        {
            _ = Log.Error(message: "Failed to list profiles.", tag: nameof(MenuActions.SelectProfile));
            return false;
        }

        if (profiles.Count == 0)
        {
            _ = Log.Warning(message: "No profiles found. Please create a new profile first.",
                tag: nameof(MenuActions.SelectProfile));
            return false;
        }

        Program.Clear();
        MenuItemCollection profilesMenu = new();

        // list profiles
        for (int x = 0; x < profiles.Count; x++)
            // adding 1 to index to avoid the ID_EXIT (which has value 0 by default)
            profilesMenu.Add(item: new MenuItem(id: x + 1, text: profiles[index: x].Name,
                alt: profiles[index: x].Created.ToShortDateString()));

        int option = ConsoleMenu.SelectMenu(items: profilesMenu);
        switch (option)
        {
            case (int)MenuIds.ID_EXIT:
            case ConsoleMenu.KEY_ESCAPE:
                return false;

            default:
            {
                // removing 1 from option to cancel the +1 we did earlier
                UserProfile profile = profiles[index: option - 1];
                Session.Profile = profile;
                Session.ProfileLoaded = true;
                return true;
            }
        }
    }

    public static bool CreateProfile()
    {
        Program.Clear();

        string name = Terminal.Input(prompt: $"Enter profile name (leave blank to cancel){Environment.NewLine}# ",
            ensureValue: false).Trim();
        if (string.IsNullOrWhiteSpace(value: name))
        {
            _ = Log.Information(message: "Profile creation cancelled by user.", tag: nameof(MenuActions.CreateProfile));
            return false;
        }

        UserProfile profile = new()
        {
            Name = name
        };

        if (!UserProfile.Save(profile: profile))
        {
            _ = Log.Error(message: "Failed to create the new profile.", tag: nameof(MenuActions.CreateProfile));
            return false;
        }

        Session.Profile = profile;
        Session.ProfileLoaded = true;
        return true;
    }

    public static bool LoadProfileSession()
    {
        if (!Session.ProfileLoaded)
            // no profile loaded
            return false;

        // create main menu
        MenuItemCollection mainMenu = new()
        {
            new MenuItem(id: (int)MenuIds.ID_DASHBOARD, text: "Dashboard"),
            new MenuItem(id: (int)MenuIds.ID_ADD_ENTRY, text: "Add Transaction"),
            new MenuItem(id: (int)MenuIds.ID_VIEW_ENTRIES, text: "View Transactions"),
            new MenuItem(id: (int)MenuIds.ID_MANAGE_CATEGORIES, text: "Manage Categories"),
            new MenuItem(),
            new MenuItem(id: (int)MenuIds.ID_LOGOUT, text: "Logout"),
            new MenuItem(id: (int)MenuIds.ID_EXIT, text: "Exit", alt: "ESC")
        };

        while (true)
        {
            Program.Clear();

            // display menu
            int option = ConsoleMenu.SelectMenu(items: mainMenu);
            switch (option)
            {
                case (int)MenuIds.ID_EXIT:
                case ConsoleMenu.KEY_ESCAPE:
                    goto MethodExit;

                case (int)MenuIds.ID_DASHBOARD:
                {
                    Program.Clear();
                    if (SessionActions.ShowDashboard())
                    {
                        Console.WriteLine();
                        Terminal.Pause();
                    }
                }
                    break;
            }
        }

        MethodExit:
        if (!UserProfile.Save(profile: Session.Profile))
            _ = Log.Error(message: "Profile saving failed.", tag: nameof(MenuActions.LoadProfileSession));
        return true;
    }
}