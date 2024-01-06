using UnityEngine;

[System.Serializable]
public class ImageAnimationController
{
    [SerializeField] UnityEngine.UI.Image _image;
    [SerializeField] float _factor = 5;

    [SerializeField] ColorAnimator _colorController;
    
    ColorAnimator ColorController => _colorController ?? ( _colorController = new ColorAnimator( _factor, _factor, _factor ) );
    bool _started = false;

    public Color CurrentColor => _image.color;

    public void Start()
    {
        var desiredColor = _image.color;
        var colorController = ColorController;
        colorController.Start( new Color( desiredColor.r, desiredColor.g, desiredColor.b, 0 ) );
        colorController.OnChange.Register( UpdateColor );
        colorController.SetColor( desiredColor, true );
        _started = true;
    }

    void UpdateColor( Color color )
    {
        _image.color = color;
    }

    public void Update( float deltaTime )
    {
        ColorController.Update( deltaTime );
    }

    public void SetColor( Color color )
    {
        if( Application.isPlaying && _started ) 
        {
            ColorController.SetColor( color, true );
        }
        else 
        {
            _image.color = color;
        }
    }
}
