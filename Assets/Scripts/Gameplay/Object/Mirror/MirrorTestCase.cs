using UnityEngine;

public class MirrorTestCase : MonoBehaviour
{
    private MirrorFactory _mirrorFactory;
    [SerializeField] private Vector3 _position;
    [SerializeField] private Quaternion _rotation;
    private int _ID;
    public int ID
    {
        get => _ID++;
    }

    public void Install(MirrorFactory mirrorFactory)
    {
        _mirrorFactory = mirrorFactory;
    }

    [ContextMenu("Create Rotate Mirror")]
    public void CreateRotateMirror()
    {
        _mirrorFactory.CreateRotateMirror(ID, _position, _rotation);
    }

    [ContextMenu("Create Slide Mirror")]
    public void CreateSlideMirror()
    {
        _mirrorFactory.CreateSlideMirror(ID, _position, _rotation);
    }

    public void Start()
    {
        CreateRotateMirror();
        _position += new Vector3(3, 1, 0);
        CreateSlideMirror();
    }

}