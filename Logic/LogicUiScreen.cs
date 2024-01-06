using System.Collections.Generic;
using System.Linq;
using static Colors.Console;

public class ScreenActionAttribute : System.Attribute
{
    public ScreenActionAttribute()
    {
    }
}

public abstract class LogicUiScreen
{
    private ILogicUiScreenManager _manager;

    protected ILogicUiScreenManager Manager => _manager;

    public bool HasBack => _manager.HasStack;
    public void Back() { _manager.Back(); }

    public void SetUiManager( ILogicUiScreenManager manager )
    {
        _manager = manager;
    }

    private List<System.Reflection.MethodInfo> _cachedActions;
    
    public IEnumerable<System.Reflection.MethodInfo> Actions
    {
        get
        {
            if( _cachedActions == null )
            {
                var attType = typeof(ScreenActionAttribute);
                var type = this.GetType();
                var allMethods = type.GetMethods();
                _cachedActions = new List<System.Reflection.MethodInfo>();
                foreach( var method in allMethods )
                {
                    if( !System.Attribute.IsDefined( method, attType ) ) continue;
                    _cachedActions.Add( method );
                }
            }
            return _cachedActions;
        }
    }

    public string ActionsDebug() => $"{this.GetType().ToStringOrNull().Colorfy(TypeName)}.{"Actions".Colorfy(Fields)}:\n    {string.Join(",\n    ", Actions.Numerated())}";
}