using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using ExitGames.Client.Photon;
using MayhemCommon;
using MayhemCommon.MessageObjects;
using UnityEngine;

public class CurrentPlayer : PlayerCharacter
{
    private Vector2 _moveDirection = Vector2.zero;
    private float _movementSpeed = 4f;
    private InGameController _gameController;

    private bool _isControlling = false;

    private bool _positionChanged = false;
    private const float UpdateTimesPerSecond = 4;
    private float _timeSinceLastUpdate = 0;
    private OperationRequest _positionUpdateRequest;
    private PositionData _positionData;

    private DirectionManager _directionManager;
    private Rigidbody2D _rigidBody;

    public int PlayerInstanceId;

    private Transform _rotationTransform;
    private int _lastScale = 0;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
    }

    public override void OnAwake()
    {
        base.OnAwake();
        _rotationTransform = transform.FindChild("Rotation");

        PlayerInstanceId = InstanceId;
        _rigidBody = GetComponent<Rigidbody2D>();

        _positionData = new PositionData();
        _directionManager = GetComponentInChildren<DirectionManager>();

        // Caching a request
        var para = new Dictionary<byte, object>();
        para.Add((byte)ClientParameterCode.SubOperationCode, MessageSubCode.PositionUpdate);
        para.Add((byte)ClientParameterCode.Object, new object());
        _positionUpdateRequest = new OperationRequest() { OperationCode = (byte)ClientOperationCode.Game, Parameters = para };
    }

    public override void OnStart()
    {
        base.OnStart();
        _gameController = InGameController.Instance;
        var feetGlow = GameObject.Find("FeetGlow");
        if (feetGlow != null)
        {
            feetGlow.GetComponent<TargetFollow>().Target = this;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        //if (Input.GetKey(KeyCode.W) ||
        //    Input.GetKey(KeyCode.A) ||
        //    Input.GetKey(KeyCode.S) ||
        //    Input.GetKey(KeyCode.D))
        //{
        //    return;
        //}

        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.y = Input.GetAxisRaw("Vertical");

        //_rigidBody.velocity = _moveDirection * MovementSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }

        if (_isMovingLocally)
        {
            gameObject.transform.position = Vector2.MoveTowards(transform.position, _localMovementDestination,
                MovementSpeed*Time.deltaTime);
            if (Vector2.Distance(transform.position, _localMovementDestination) < 0.2f)
            {
                OnStopControlling();
                _isMovingLocally = false;
            }

            _positionChanged = true;
        }

        if ((_moveDirection.x != 0 || _moveDirection.y != 0) && CanMove())
        {
            _directionManager.Angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * 180 / (float)Math.PI;
            gameObject.transform.Translate(_moveDirection * MovementSpeed * Time.deltaTime);

            if (_moveDirection.x > 0 && _lastScale != 1)
            {
                _lastScale = 1;
                _rotationTransform.localScale = new Vector3(_lastScale, 1, 1);
            }
            if (_moveDirection.x < 0 && _lastScale != -1)
            {
                _lastScale = -1;
                _rotationTransform.localScale = new Vector3(_lastScale, 1, 1);
            }

            if (!_isControlling)
            {
                OnStartControlling();
            }
            _isControlling = true;
            _positionChanged = true;
        }
        else
        {
            if (_isControlling)
            {
                OnStopControlling();
            }
            _isControlling = false;
        }

        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate > 1 / UpdateTimesPerSecond)
        {
            _timeSinceLastUpdate = 0;

            if (_positionChanged)
            {
                _positionData.X = gameObject.transform.position.x;
                _positionData.Y = gameObject.transform.position.y;
                _positionData.Z = gameObject.transform.position.z;

                _positionUpdateRequest.Parameters[(byte)ClientParameterCode.Object] = PacketHandler.Serialize(_positionData);
                //_gameController.SendOperation(_positionUpdateRequest, false, 0, false);
                _positionChanged = false;
            }
        }
    }

    public bool CanMove()
    {
        return true;
    }

    public void Attack()
    {
        Animator.SetTrigger("Attack");
    }

    private bool _isMovingLocally = false;
    private Vector3 _localMovementDestination;

    public void MoveLocallyTo(Vector3 position)
    {
        _isMovingLocally = true;
        _localMovementDestination = position;
        OnStartControlling();
    }

    public void OnStartControlling()
    {
        // Fixing an issue where walking while making an action disrupts following
        //_gameController.InGameView.Gui.Actions.SetButtonsEnabled(false);
        StartMoving();
    }

    public void OnStopControlling()
    {
        //_gameController.InGameView.Gui.Actions.SetButtonsEnabled(true);
        StopMoving();
    }
}
