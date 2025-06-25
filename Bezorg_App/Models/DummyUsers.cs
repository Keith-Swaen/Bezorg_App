namespace Bezorg_App.Views;

public static class DummyUsers
{
    public static readonly List<User> Users = new()
    {
        new User { Name = "Keith", Email = "keith@bezorgapp.nl" },
        new User { Name = "Lars", Email = "lars@bezorgapp.nl" },
        new User { Name = "Milan", Email = "milan@bezorgapp.nl" },
        new User { Name = "Vince", Email = "vince@bezorgapp.nl" },
    };

    public const string DefaultPassword = "matrix123";
} 