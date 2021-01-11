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
    private ScrewGuide _focusedScrewGuide;
    public ScrewGuide focusedScrewGuide
    {
        get { return _focusedScrewGuide; }
        set 
        { 
            _focusedScrewGuide = value;
            setInteractionEnabled(_focusedScrewGuide == null);
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
        modelNew.transform.SetParent(screwGuideNew.modelParent);
        modelNew.transform.localPosition = Vector3.zero;
        modelNew.transform.localRotation = Quaternion.identity;

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
    }

    //enable interaction only if no screw is being focused
    public void setInteractionEnabled(bool enabled)
    {
        if (_currentPhase == Phase.Placement)
        {
            for (int i = 0; i < _liGuides.Count; i++)
                _liGuides[i].placement.setInteractionEnabled(enabled);
        }
    }
}
