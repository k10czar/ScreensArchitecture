using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public static class AsyncOperationHandleExtension
{
    public static void ExecuteWhenItsDone<T>( this AsyncOperationHandle<T> op, System.Action<AsyncOperationHandle<T>> actionToExecute )
    {
        if( op.IsDone ) actionToExecute( op );
        else op.Completed += actionToExecute;
    }
}

public static class AssetReferenceExtension
{
    public static void ExecuteWhenItsDone<T>( this AssetReference ar, System.Action<T> actionToExecute )
    {
        var asset = ar.Asset;
        if( asset != null ) actionToExecute( (T)(object)ar.Asset );
        else 
        {
            var op = ar.OperationHandle;
            if( op.IsValid() )
            {
                if( op.IsDone ) actionToExecute( (T)op.Result );
                else op.Completed += ( op ) => actionToExecute( (T)op.Result );
            } 
            else ar.LoadAssetAsync<T>().ExecuteWhenItsDone( ( op ) => actionToExecute( op.Result ) );
        }
    }
}