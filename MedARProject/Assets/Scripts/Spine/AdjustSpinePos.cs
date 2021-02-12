using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSpinePos : MonoBehaviour
{
    public enum Mode
    { 
        Box,
        Spine
    }

    [System.Serializable]
    public class AdjustObj
    {
        [SerializeField]
        private string name;
        
        [SerializeField]
        private GameObject goThis;
        [SerializeField]
        private Transform transStyrofoam;
        private Renderer[] _renStyrofoam;
        private Collider[] _colStyrofoam;
        [SerializeField]
        private Transform transSpine;
        private Renderer[] _renSpine;
        private Collider[] _colSpine;

        [SerializeField]
        private Vuforia.ImageTargetBehaviour marker;

        private Transform transActive;
        private bool active;

        public Vector3 Pos { get { return transActive.localPosition; } }
        public Vector3 Rot { get { return transActive.localEulerAngles; } }
        public string Name { get { return name; } }

        public void init()
        {
            _renStyrofoam = transStyrofoam.GetComponentsInChildren<Renderer>();
            _colStyrofoam = transStyrofoam.GetComponentsInChildren<Collider>();

            _renSpine = transSpine.GetComponentsInChildren<Renderer>();
            _colSpine = transSpine.GetComponentsInChildren<Collider>();
        }

        public void setMode(Mode m)
        {
            transActive = m == Mode.Box ? transStyrofoam : transSpine;

            bool bStyrofoam = m == Mode.Box;
            transStyrofoam.gameObject.SetActive(bStyrofoam);

            bool bSpine = m == Mode.Spine;
            transSpine.gameObject.SetActive(bSpine);
        }

        private void setRenAndCol()
        {
            for (int i = 0; i < _renStyrofoam.Length; i++)
                _renStyrofoam[i].enabled = true;
            for (int i = 0; i < _colStyrofoam.Length; i++)
                _colStyrofoam[i].enabled = true;

            for (int i = 0; i < _renSpine.Length; i++)
                _renSpine[i].enabled = true;
            for (int i = 0; i < _colSpine.Length; i++)
                _colSpine[i].enabled = true;
        }

        public void setActive(bool a)
        {
            active = a;
        }

        public void update()
        {
            goThis.SetActive(active &&
                (marker.CurrentStatus == Vuforia.TrackableBehaviour.Status.TRACKED ||
                marker.CurrentStatus == Vuforia.TrackableBehaviour.Status.EXTENDED_TRACKED)
                );

            goThis.transform.position = marker.transform.position;
            goThis.transform.rotation = marker.transform.rotation;

            setRenAndCol(); //No idea why they keep being disabled...
        }

        public void startAdjust(RegisterSpine.SpineMarker m)
        {
            transStyrofoam.localPosition = m.transStyrofoam.localPosition;
            transStyrofoam.localEulerAngles = m.transStyrofoam.localEulerAngles;
            transSpine.localPosition = m.transSpine.localPosition;
            transSpine.localEulerAngles = m.transSpine.localEulerAngles;
        }
        public void endAdjust(RegisterSpine.SpineMarker m)
        {
            m.transStyrofoam.localPosition = transStyrofoam.localPosition;
            m.transStyrofoam.localEulerAngles = transStyrofoam.localEulerAngles;
            m.transSpine.localPosition = transSpine.localPosition;
            m.transSpine.localEulerAngles = transSpine.localEulerAngles;
        }
    }

    [SerializeField]
    private AdjustObj _marker01;
    [SerializeField]
    private AdjustObj _marker02;
    [SerializeField]
    private AdjustObj _marker03;
    [SerializeField]
    private AdjustObj _marker04;

    [SerializeField]
    private RegisterSpine _spine;

    [SerializeField]
    private Transform _transMenu;
    [SerializeField] private float _menuOffset;

    [SerializeField]
    private TMPro.TMP_Text _textMode;
    [SerializeField]
    private TMPro.TMP_Text _textActive;
    [SerializeField]
    private TMPro.TMP_Text _textPos;
    [SerializeField]
    private TMPro.TMP_Text _textRot;

    private AdjustObj active = null;
    private Mode _mode = Mode.Box;

    private void Start()
    {
        Camera cam = Camera.main;
        _transMenu.position = cam.transform.position + cam.transform.forward * _menuOffset;
        _transMenu.LookAt(_transMenu.position + cam.transform.forward.normalized);

        _marker01.init();
        _marker02.init();
        _marker03.init();
        _marker04.init();

        setBox();
        set01();
    }

    private void Update()
    {
        _marker01.update();
        _marker02.update();
        _marker03.update();
        _marker04.update();

        _textMode.text = "Mode: \n" + _mode.ToString();
        _textActive.text = "Active: \n" + active.Name;
        _textPos.text = "Position: \n" + active.Pos.ToString("F4");
        _textRot.text = "Rotation: \n" + active.Rot.ToString("F4");
    }

    //

    public void setBox()
    {
        setMode(Mode.Box);
    }

    public void setSpine()
    {
        setMode(Mode.Spine);
    }

    private void setMode(Mode mode)
    {
        _mode = mode;

        _marker01.setMode(mode);
        _marker02.setMode(mode);
        _marker03.setMode(mode);
        _marker04.setMode(mode);
    }

    //

    public void set01()
    {
        active = _marker01;
        setMarkerActive();
    }

    public void set02()
    {
        active = _marker02;
        setMarkerActive();
    }

    public void set03()
    {
        active = _marker03;
        setMarkerActive();
    }

    public void set04()
    {
        active = _marker04;
        setMarkerActive();
    }

    private void setMarkerActive()
    {
        _marker01.setActive(_marker01 == active);
        _marker02.setActive(_marker02 == active);
        _marker03.setActive(_marker03 == active);
        _marker04.setActive(_marker04 == active);
    }

    //

    public void startAdjust()
    {
        List<RegisterSpine.SpineMarker> liSpineMarker = _spine.liMarker;

        _marker01.startAdjust(liSpineMarker[0]);
        _marker02.startAdjust(liSpineMarker[1]);
        _marker03.startAdjust(liSpineMarker[2]);
        _marker04.startAdjust(liSpineMarker[3]);

        gameObject.SetActive(true);
    }
    //called via voice command
    public void endAdjust()
    {
        List<RegisterSpine.SpineMarker> liSpineMarker = _spine.liMarker;

        _marker01.endAdjust(liSpineMarker[0]);
        _marker02.endAdjust(liSpineMarker[1]);
        _marker03.endAdjust(liSpineMarker[2]);
        _marker04.endAdjust(liSpineMarker[3]);

        gameObject.SetActive(false);
    }
}
