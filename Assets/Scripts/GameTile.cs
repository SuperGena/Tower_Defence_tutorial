using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    private Transform _arrow;

    private GameTile _north, _east, _south, _west, _nextOnPath;

    private int _distance;

    public bool HasPath => _distance != int.MaxValue;

    public bool IsAlternative { get; set; }

    private Quaternion _northRotation = Quaternion.Euler(90f, 0f, 0f);
    private Quaternion _eastRotation = Quaternion.Euler(90f, 90f, 0f);
    private Quaternion _southRotation = Quaternion.Euler(90f, 180f, 0f);
    private Quaternion _westRotation = Quaternion.Euler(90f, 270f, 0f);

    private GameTileContent _content;

    public GameTile NextTileOnPath => _nextOnPath;

    public Vector3 ExitPoint { get; private set; }

    public Direction PathDirection { get; private set; }

    public GameTileContent Content
    {
        get => _content;
        set
        {
            if (_content != null)
            {
                _content.Recycle();
            }

            _content = value;
            _content.transform.localPosition = transform.localPosition;
        }
    }

    public static void MakeEastWestNeibors(GameTile east, GameTile west)
    {
        west._east = east;
        east._west = west;
    }

    public static void MakeNorthSouthNeibors(GameTile north, GameTile south)
    {
        north._south = south;
        south._north = north;
    }

    public void ClearPath()
    {
        _distance = int.MaxValue;
        _nextOnPath = null;
    }

    public void BecomeDestination()
    {
        _distance = 0;
        _nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    private GameTile GrowPathTo(GameTile neigbor, Direction direction)
    {
        if (!HasPath || neigbor == null || neigbor.HasPath)
        {
            return null;
        }

        neigbor._distance = _distance + 1;
        neigbor._nextOnPath = this;
        neigbor.ExitPoint = (neigbor.transform.localPosition + transform.localPosition) * 0.5f;
        neigbor.PathDirection = direction;
        return neigbor.Content.Type != GameTileContentType.Wall ? neigbor : null;
    }

    public GameTile GrowPathNorth() => GrowPathTo(_north, Direction.North);
    public GameTile GrowPathEast() => GrowPathTo(_east, Direction.East);
    public GameTile GrowPathSouth() => GrowPathTo(_south, Direction.South);
    public GameTile GrowPathWest() => GrowPathTo(_west, Direction.West);

    public void ShowPath()
    {
        if (_distance == 0)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }
        _arrow.gameObject.SetActive(true);
        _arrow.localRotation =
            _nextOnPath == _north ? _northRotation :
            _nextOnPath == _east ? _eastRotation :
            _nextOnPath == _south ? _southRotation :
            _westRotation;
    }
}