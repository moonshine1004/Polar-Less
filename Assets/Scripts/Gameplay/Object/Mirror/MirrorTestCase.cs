using System.Collections.Generic;
using UnityEngine;

public class MirrorTestCase : MonoBehaviour
{
    private ObjectFactory _mirrorFactory;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Quaternion _rotation;
    private int _ID;
    public int ID
    {
        get => _ID++;
    }

    public void Install(ObjectFactory mirrorFactory)
    {
        _mirrorFactory = mirrorFactory;
    }

    [ContextMenu("Create Rotate Mirror")]
    public void CreateRotateMirror()
    {
        _mirrorFactory.CreateObject(new ObjectData
        {
            objectType = ObjectType.Rotate,
            ID = ID,
            Position = _position,
            Rotation = _rotation
        });
    }

    [ContextMenu("Create Slide Mirror")]
    public void CreateSlideMirror()
    {
        _mirrorFactory.CreateObject(new ObjectData
        {
            objectType = ObjectType.Slide,
            ID = ID,
            Position = _position,
            Rotation = _rotation
        });
    }

    public void Start()
    {
        CreateRotateMirror();
        _position += new Vector3(3, 1, 0);
        CreateSlideMirror();
    }

}