using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CharacterLoad : View
{
    private bool _loaded;

    public override void Awake()
    {
        Controller = new CharacterLoadController(this);
        _loaded = false;
    }

    private CharacterLoadController _controller;

    public override IViewController Controller { get { return _controller; } protected set
    {
        _controller = value as CharacterLoadController;
    } }

    void Update()
    {
        if (!_loaded)
        {
            _loaded = true;
            _controller.SendCharacterLoading();
        }
    }

}
