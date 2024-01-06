using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

using static Colors.Console;

public enum EScreenHookSystemLog
{
    AssetLoad,
    Instantiation,
    AsyncInstantiation,
    Setup,
    ObjectReadyReaction,
}

public class ScreenHookSystemLog : TimeLogging<EScreenHookSystemLog>
{
	private static ScreenHookSystemLog _instance;
	public static ScreenHookSystemLog Instance => _instance ?? ( _instance = new ScreenHookSystemLog() );
	public static void Clear() { _instance = null; }
	
	private ScreenHookSystemLog() 
	{
        _onLogEnd.Register( Debug.Log );
        _onLogSectionEnd.Register( Debug.Log );
	}
}

public class UiScreenHookSystem
{
    IScreenHooksDataSource _data;
    IScreenLogicExecutionObserver _logicExecution;

    CachedReference<IPrefabAddressableTypeHookData> _currentData = new CachedReference<IPrefabAddressableTypeHookData>();
    CachedReference<IUiElementPresenter> _currentPresentationScreen = new CachedReference<IUiElementPresenter>();
    CachedReference<GameObject> _currentGameObject = new CachedReference<GameObject>();

	public IReferenceHolder<GameObject> CurrentGameObject => _currentGameObject;

    public UiScreenHookSystem( IScreenHooksDataSource data, IScreenLogicExecutionObserver logicExecution )
    {
        _data = data;
        _logicExecution = logicExecution;
        _currentData.OnReferenceRemove.Register( OnDataRemove );
        _currentData.Synchronize( OnDataSet );
        BindSystems();
    }

    public void Kill()
    {
        _data = null;
        _logicExecution = null;
        _currentData?.Kill();
        _currentPresentationScreen?.Kill();
        _currentGameObject?.Kill();
    }

    private void BindSystems()
    {
        _logicExecution.CurrentScreen.Synchronize( OnCurrentScreenSet );
    }

	private void OnDataSet( IPrefabAddressableTypeHookData screenData )
    {
        Debug.Log( $"{"UiScreenHookSystem".Colorfy(TypeName)}.{"OnDataSet".Colorfy(EventName)}( {screenData.ToStringOrNull().ToStringOrNullColored(Numbers)} )" );

        if( screenData == null )
        {
            _currentPresentationScreen.ChangeReference( null );
            return;
        }

        var logger = ScreenHookSystemLog.Instance;
        logger.StartLog( screenData.GetType().ToStringOrNull() );

        var logicScreen = _logicExecution.CurrentScreen.CurrentReference;
        logger.StartSection( EScreenHookSystemLog.AsyncInstantiation );
        screenData.Prefab.InstantiateAsync().ExecuteWhenItsDone( ( AsyncOperationHandle<GameObject> opHandle ) =>
        {
            var go = opHandle.Result;
            logger.EndSection( EScreenHookSystemLog.AsyncInstantiation );
            logger.StartSection( EScreenHookSystemLog.Setup );
            var hook = go.GetComponent<IPresentationScreenLogicHook>();
            var presenter = go.GetComponent<IUiElementPresenter>();
            if( presenter == null ) Debug.LogError( $"{"Cannot find".Colorfy(Negation)} the {"UI presenter".Colorfy(Abstraction)} to {"control element presentation".Colorfy(TypeName)}" );
            _currentPresentationScreen.ChangeReference( presenter );
            presenter?.Show();
            hook.SetHook( logicScreen );
            logger.EndSection( EScreenHookSystemLog.Setup );
            logger.StartSection( EScreenHookSystemLog.ObjectReadyReaction );
            _currentGameObject.ChangeReference( go );
            logger.EndSection( EScreenHookSystemLog.ObjectReadyReaction );
            logger.EndLog();
        } );
    }

    private IAsyncOperationObserver HideCurrentScreen()
    {
        var element = _currentPresentationScreen.CurrentReference;
        Debug.Log( $"{"UiScreenHookSystem".Colorfy(TypeName)}.{"HideCurrentScreen".Colorfy(Negation)}( {element.ToStringOrNullColored(Numbers)} )" );

        if( element == null ) return null;
        return element.HideAndDestroy();
    }

	private void OnDataRemove( IPrefabAddressableTypeHookData screenData )
    {
        Debug.Log( $"{"UiScreenHookSystem".Colorfy(TypeName)}.{"OnDataRemove".Colorfy(Negation)}( {screenData.ToStringOrNullColored(Numbers)} )" );
    }

	private void OnCurrentScreenSet( LogicUiScreen logicScreen )
	{
        Debug.Log( $"{"UiScreenHookSystem".Colorfy(TypeName)}.{"OnCurrentScreenSet".Colorfy(EventName)}( {logicScreen.ToStringOrNullColored(Numbers)} )" );
        if( logicScreen == null ) 
        {
            _currentData.ChangeReference( null );
            return;
        }
        
        var data = _data.GetPresentationScreenData( logicScreen.GetType() );
        var screenFadeOp = HideCurrentScreen();
        screenFadeOp.ExecuteWhenOperationEnd( () => _currentData.ChangeReference( data ) );
	}
}
