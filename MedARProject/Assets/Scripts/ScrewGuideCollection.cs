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
        set { _focusedScrewGuide = value; }
    }

    public void createScrewGuide(string modelName)
    {
        //Create base prefab with specific model so we can switch between different screw types later

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

        screwGuideNew.enterPlacementPhase();
    }

    public void enterPlacementPhase()
    {
        for (int i = 0; i < _liGuides.Count; i++)
            _liGuides[i].enterPlacementPhase();
    }

    public void enterVisualizationPhase()
    {
        for (int i = 0; i < _liGuides.Count; i++)
            _liGuides[i].enterVisualizationPhase();
    }
}
