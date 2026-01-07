using System;
using Osiris.Data;

namespace Osiris.System;

public static class OsiSystem
{
    // Single static class implementing the locator pattern: https://gameprogrammingpatterns.com/service-locator.html
    static OsiSession Session_;
    public static OsiSession Session
    {
        get => Session_ ??= LoadSession();
    }
    public static OsiSession LoadSession()
    {
        // Todo: if old session exists, save it to disk.
        // Todo: if session file exists with given id, load it with TryFromNode.
        Session_ = new(Guid.NewGuid());
        OsiLoadProject.Load();
        return Session_;
    }
    static OsiUser User_;
    public static OsiUser User
    {
        get {
            if(User_ is null) throw new Exception("Attempted to read user data before it was set!");
            return User_;
        }
    }
    public static void SetUser(OsiUser user)
    {
        User_ = user;
    }
    public static bool IsGm()
    {
        return Session.Gms.Contains(User.Id);
    }
    public static bool IsPlayer()
    {
        return Session.Players.Contains(User.Id);
    }
    static OsiLogger Logger_;
    public static OsiLogger Logger
    {
        get => Logger_ ??= new();
    }
    static OsiFs Fs_;
    public static OsiFs Fs
    {
        get => Fs_ ??= new();
    }
}
