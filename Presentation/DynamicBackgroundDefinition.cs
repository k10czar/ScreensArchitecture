using UnityEngine;


[CreateAssetMenu(fileName="BackgroundColor",menuName="Knowledge/UI/BackgroundColors",order=21)]
public class DynamicBackgroundDefinition : ScriptableObject
{
    [SerializeField] Color _background;
    [SerializeField] Color _topBright;
    [SerializeField] Color _leftGradient;
    [SerializeField] Color _rightGradient;
    [SerializeField] Color _bottomBright;
    [SerializeField] Color _middleBright;
    
    public Color Background => _background;
    public Color TopBright => _topBright;
    public Color LeftGradient => _leftGradient;
    public Color RightGradient => _rightGradient;
    public Color BottomBright => _bottomBright;
    public Color MiddleBright => _middleBright;
}