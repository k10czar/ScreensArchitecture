using static Colors.Console;

public class LogicScreenConsole : IConsoleInteractor, ICustomDisposableKill
{
    bool _isDirty;
    CachedReference<LogicUiScreen> _currentScreen = new CachedReference<LogicUiScreen>();

    public LogicScreenConsole( LogicUiScreenManager uiManager )
    {
        uiManager.CurrentScreen.Synchronize( _currentScreen );
        _currentScreen.OnReferenceSet.Register( SetDirty );
    }

    private void SetDirty()
    {
        _isDirty = true;
    }

    public int ActionsCount => _currentScreen?.CurrentReference?.Actions?.Count ?? 0;
    public bool HasBack => _currentScreen?.CurrentReference?.HasBack ?? false;
    public bool IsDirty => _isDirty;

    public string Text => _currentScreen?.CurrentReference?.ActionsDebugColored() ?? "NULL Screen";

    public void Back() { _currentScreen?.CurrentReference?.Back(); }
    public void DoAction( int index )
    {
        var screen = _currentScreen?.CurrentReference;
        var action = screen?.Actions[index];
        action.Invoke(screen, null);
    }

    public string GetActionDebugName(int index)
    {
        var screen = _currentScreen?.CurrentReference;
        var action = screen?.Actions[index];
        return action.ToStringOrNull();
    }

    public string GetActionDebugNameColored(int index)
    {
        var screen = _currentScreen?.CurrentReference;
        var action = screen?.Actions[index];
        return action.ToStringOrNullColored(EventName);
    }

    public void Kill()
    {
        _currentScreen?.Kill();
        _currentScreen = null;
    }
}