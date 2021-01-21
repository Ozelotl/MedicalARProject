//Stella

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

/// <summary>
/// Manages all screw guides
/// </summary>
public class ScrewGuideCollection : SingletonMonoMortal<ScrewGuideCollection>
{
    public enum Phase
    { 
        Placement, 
        Visualization
    }

    private const string _pathToModelFolder = "ScrewModels/";

    [SerializeField]
    private GameObject _screwGuidePrefab;

    private List<ScrewGuide> _liGuides = new List<ScrewGuide>();
    public ReadOnlyCollection<ScrewGuide> liGuidesReadonly
    {
        get
        {
            return _liGuides.AsReadOnly();
        }
    }
    
    //being adjusted or being visualized
    private ScrewGuide _focusedScrewGuide;
    public ScrewGuide focusedScrewGuide
    {
        get { return _focusedScrewGuide; }
        set 
        { 
            _focusedScrewGuide = value;
        }
    }

    private Phase _currentPhase;

    //Create base prefab with specific model so we can switch between different screw types later
    public void createScrewGuide(string modelName)
    {
        GameObject screwGuideObjectNew = Instantiate(_screwGuidePrefab);
        ScrewGuide screwGuideNew = screwGuideObjectNew.GetComponent<ScrewGuide>();

        GameObject modelPrefab = Resources.Load<GameObject>(_pathToModelFolder + modelName);
        GameObject modelNew = Instantiate(modelPrefab);

        //Calculate screw depth from renderer
        //1) reset transform for calculations
        modelNew.transform.position = Vector3.zero;
        modelNew.transform.rotation = Quaternion.identity;
        Renderer[] modelRenderer = modelNew.GetComponentsInChildren<Renderer>();
        Bounds boundsScrewModel = new Bounds();
        for (int i = 0; i < modelRenderer.Length; i++)
            boundsScrewModel.Encapsulate(modelRenderer[i].bounds);
        screwGuideNew.screwDepth = boundsScrewModel.min.y;

        //Set model parent and reset transform 
        modelNew.transform.SetParent(screwGuideNew.modelParent);
        modelNew.transform.localPosition = Vector3.zero;
        modelNew.transform.localRotation = Quaternion.identity;
        //Set screw parent and reset transform 
        screwGuideNew.transform.SetParent(transform);
        screwGuideNew.transform.localPosition = Vector3.zero;
        screwGuideNew.transform.localRotation = Quaternion.identity;

        _liGuides.Add(screwGuideNew);
        _focusedScrewGuide = screwGuideNew;

        BoxCollider collider = modelNew.GetComponent<BoxCollider>();
        {
            if (collider == null)
                Debug.LogError("Every Screw Model needs a BoxCollider Component!");
            else
            {
                screwGuideNew.transferCollider(collider);
                Destroy(collider);
            }
        }

        screwGuideNew.enterPhase(Phase.Placement);
        setInteractionEnabled(false);
    }
    public void deleteScrewGuide(ScrewGuide _guide)
    {
        _liGuides.Remove(_guide);
        if (focusedScrewGuide == _guide)
            focusedScrewGuide = null;

        Destroy(_guide.gameObject);
    }

    public void enterPhase(Phase phaseNew)
    {
        _currentPhase = phaseNew;

        for (int i = 0; i < _liGuides.Count; i++)
            _liGuides[i].enterPhase(_currentPhase);

        setInteractionEnabled(true);
    }

    //enable interaction only if no screw is being focused
    public delegate void InteractionEnabled(bool enabled);
    public InteractionEnabled OnInteractionEnabled;

    public void setInteractionEnabled(bool enabled)
    {
        if (OnInteractionEnabled != null)
            OnInteractionEnabled.Invoke(enabled);
    }
}
