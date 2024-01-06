using System.Collections.Generic;
using UnityEngine;

using static Colors.Console;

public interface IScreenLogicExecutionObserver
{
    ICachedReference<LogicUiScreen> CurrentScreen { get; }
}

public interface ILogicUiScreenManager
{
    bool HasStack { get; }
    void Back();
    T Open<T>() where T : LogicUiScreen, new();
    T OpenNew<T>() where T : LogicUiScreen, new();
}

public class LogicUiScreenManager : ILogicUiScreenManager, IScreenLogicExecutionObserver
{
    bool _debugLogs = true;
    bool _debugErrors = true;

    List<LogicUiScreen> _stack = new List<LogicUiScreen>();

    CachedReference<LogicUiScreen> _currentScreen = new CachedReference<LogicUiScreen>();

    public LogicUiScreenManager( bool logErrors = true, bool normalLogs = true )
    {
        _debugLogs = normalLogs;
        _debugErrors = logErrors;
    }

    public ICachedReference<LogicUiScreen> CurrentScreen => _currentScreen;
    
    public bool HasStack => _stack.Count > 1;
    public void Back()
    {
        if( !HasStack )
        {
            if( _debugErrors ) Debug.LogError( $"Do {"not".Colorfy(Names)} has screen to get back\n{StackDebug()}" );
        }
    }

    public string StackDebug() => $"{"LogicUiScreenManager".Colorfy(TypeName)}.{"stack".Colorfy(Fields)}: {string.Join( ", ", _stack )}";

    public T Open<T>() where T : LogicUiScreen, new()
    {
        if( _debugLogs ) Debug.Log( $"{"LogicUiScreenManager".Colorfy(TypeName)}.{"Open".Colorfy(Verbs)}<{typeof(T).ToStringOrNull().Colorfy(Keyword)}>()" );
        var index = IndexOf<T>();
        if( index == -1 ) return OpenNew<T>();
        var screen = _stack[index];
        // _stack.RemoveAt( index );
        SetCurrentScreen( screen );
        return screen as T;
    }

    public T OpenNew<T>() where T : LogicUiScreen, new()
    {
        if( _debugLogs ) Debug.Log( $"{"LogicUiScreenManager".Colorfy(TypeName)}.{"OpenNew".Colorfy(Verbs)}<{typeof(T).ToStringOrNull().Colorfy(Keyword)}>()\n" );
        var screen = new T();
        screen.SetUiManager( this );
        SetCurrentScreen( screen );
        return screen;
    }

    private void SetCurrentScreen( LogicUiScreen screen )
    {
        _stack.Add( screen );
        _currentScreen.ChangeReference( screen );
        if( _debugLogs ) Debug.Log( screen.ActionsDebug() );
    }

    public int IndexOf<T>() where T : LogicUiScreen
    {
        for( int i = 0; i < _stack.Count; i++ )
        {
            var screen = _stack[i];
            if( screen != null && screen is T ) return i;
        }
        return -1;
    }

    public void RemoveNulls()
    {
        for( int i = _stack.Count - 1; i >= 0; i-- )
        {
            var screen = _stack[i];
            if( screen != null ) continue;
            _stack.RemoveAt( i );
        }
    }
}