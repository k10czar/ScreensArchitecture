using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBackgroundController : VisibleUiElement, IVisualWidget
{
    //TODO_OPTIMIZATION: Instead of many colored textures we can do with shader that pass once at each pixel avoiding overdrawing transparent pixels

    [SerializeField] ImageAnimationController _background;
    [SerializeField] ImageAnimationController _topBright;
    [SerializeField] ImageAnimationController _leftGradient;
    [SerializeField] ImageAnimationController _rightGradient;
    [SerializeField] ImageAnimationController _bottomBright;
    [SerializeField] ImageAnimationController _middleBright;

	public Color BackgroundColor => _background.CurrentColor;
	public Color TopBrightColor => _topBright.CurrentColor;
	public Color LeftGradientColor => _leftGradient.CurrentColor;
	public Color RightGradientColor => _rightGradient.CurrentColor;
	public Color BottomBrightColor => _bottomBright.CurrentColor;
	public Color MiddleBrightColor => _middleBright.CurrentColor;

    void Start()
    {
        _background.Start();
        _topBright.Start();
        _leftGradient.Start();
        _rightGradient.Start();
        _bottomBright.Start();
        _middleBright.Start();
    }

	public void SetColors( DynamicBackgroundDefinition colors )
    {        
        _background.SetColor( colors.Background );
        _topBright.SetColor( colors.TopBright );
        _leftGradient.SetColor( colors.LeftGradient );
        _rightGradient.SetColor( colors.RightGradient );
        _bottomBright.SetColor( colors.BottomBright );
        _middleBright.SetColor( colors.MiddleBright );
    }

    void Update()
    {
        var deltaTime = Time.deltaTime;
        _background.Update( deltaTime );
        _topBright.Update( deltaTime );
        _leftGradient.Update( deltaTime );
        _rightGradient.Update( deltaTime );
        _bottomBright.Update( deltaTime );
        _middleBright.Update( deltaTime );
    }
}
