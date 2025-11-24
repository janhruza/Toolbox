using Cashly.Core;

using System;
using System.Collections.Generic;

using Toolbox;
using Toolbox.UI;

using static Cashly.MenuIds;

namespace Cashly;

internal static class MenuActions
{
    public static bool SelectProfile()
    {
        List<UserProfile> profiles;
        if (UserProfile.EnumProfiles(out profiles) == false)
        {
            Log.Error("Failed to list profiles.", nameof(SelectProfile));
            return false;
        }

        if (profiles.Count == 0)
        {
            Log.Warning("No profiles found. Please create a new profile first.", nameof(SelectProfile));
            return false;
        }

        Program.Clear();
        MenuItemCollection profilesMenu = new MenuItemCollection();

        // list profiles
        for (int x = 0; x < profiles.Count; x++)
        {
            // adding 1 to index to avoid the ID_EXIT (which has value 0 by default)
            profilesMenu.Add(new MenuItem(x + 1, profiles[x].Name, profiles[x].Created.ToShortDateString()));
        }

        int option = ConsoleMenu.SelectMenu(profilesMenu);
        switch (option)
        {
            case (int)ID_EXIT:
            case ConsoleMenu.KEY_ESCAPE:
                return false;

            default:
                {
                    // removing 1 from option to cancel the +1 we did earlier
                    UserProfile profile = profiles[option - 1];
                    Session.Profile = profile;
                    Session.ProfileLoaded = true;
                    return true;
                }
        }
    }

    public static bool CreateProfile()
    {
        Program.Clear();

        string name = Terminal.Input($"Enter profile name (leave blank to cancel){Environment.NewLine}# ", false).Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Log.Information("Profile creation cancelled by user.", nameof(CreateProfile));
            return false;
        }

        UserProfile profile = new UserProfile()
        {
            Name = name
        };

        if (UserProfile.Save(profile) == false)
        {
            Log.Error("Failed to create the new profile.", nameof(CreateProfile));
            return false;
        }

        Session.Profile = profile;
        Session.ProfileLoaded = true;
        return true;
    }

    public static bool LoadProfileSession()
    {
        if (Session.ProfileLoaded == false)
        {
            // no profile loaded
            return false;
        }

        // create main menu
        MenuItemCollection mainMenu = new MenuItemCollection()
        {
            new MenuItem((int)ID_DASHBOARD, "Dashboard"),
            new MenuItem((int)ID_ADD_ENTRY, "Add Transaction"),
            new MenuItem((int)ID_VIEW_ENTRIES, "View Transactions"),
            new MenuItem((int)ID_MANAGE_CATEGORIES, "Manage Categories"),
            new MenuItem(),
            new MenuItem((int)ID_LOGOUT, "Logout"),
            new MenuItem((int)ID_EXIT, "Exit", "ESC")
        };

        while (true)
        {
            Program.Clear();

            // display menu
            int option = ConsoleMenu.SelectMenu(mainMenu);
            switch (option)
            {
                case (int)ID_EXIT:
                case ConsoleMenu.KEY_ESCAPE:
                    goto MethodExit;

                case (int)ID_DASHBOARD:
                    {
                        Program.Clear();
                        if (SessionActions.ShowDashboard() == true)
                        {
                            Console.WriteLine();
                            Terminal.Pause();
                        }

                    }
                    break;

                default: break;
            }

        }

    MethodExit:
        if (UserProfile.Save(Session.Profile) == false)
        {
            Log.Error($"Profile saving failed.", nameof(LoadProfileSession));
        }
        return true;
    }
}
