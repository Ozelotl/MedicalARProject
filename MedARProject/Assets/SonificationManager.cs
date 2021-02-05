using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonificationManager : MonoBehaviour
{
    #region fields
    [Header("Scene References")]
    [SerializeField]
    private GameObject _relaxingSound;
    [SerializeField]
    private GameObject _alertSound;
    [SerializeField]
    private GameObject _ConfirmOrientationSound;

    [Header("General relaxing Sound Attriubutes")]
    [SerializeField]
    [Range(0, 1)]
    private float _relax_volume_general;


    [Header("PositionPhase")]
    [SerializeField]
    [Range(0f, 10f)]
    private float _fadingIn_time;
    [SerializeField]
    [Range(0f, 10f)]
    private float _fadingOut_time;


    [Header("Depth Phase")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _occlusion_distance_factor;
    [SerializeField]
    [Range(0f, 1f)]
    private float _relax_volume_occluded;
    [SerializeField]
    [Range(0f, 22000f)]
    private float _occlusion_cut_off_frequency;
    [SerializeField]
    [Range(0f, 1f)]
    private float _alertAudio_min;
    [SerializeField]
    [Range(0.5f, 1f)]
    private float _ratio_begin_of_critical_region;


    [Header("Manual mode")]
    [SerializeField]
    private bool _manual_screw_focus;
    [SerializeField]
    private bool _manual_correct_orientation;
    [SerializeField]
    private bool _manual_Depth_manipulation;
    [SerializeField]
    [Range(-1, 10)]
    private float _manual_screw_length;
    [SerializeField]
    [Range(-1, 11)]
    private float _manual_drilling_depth;








    #endregion






    private PhaseManager.Phase _currentPhase; //gets updated from phaseManager
    private bool _sonifyPosition; //gets updated from PositionVisualization
    private bool _sonifyOrientation; //gets updated from OrientationVisualiztation
    private AudioSource _relaxAudio;
    private AudioLowPassFilter _relaxAudioLowPass;
    private AudioSource _alertAudio;
    private bool _relaxSoundOn;
    private bool _orientationConfirmed;
    private bool _alertSoundOn;

    //information on drilling depth
    private float _screwLength, _drillingDepth;


    // ############## setter ##################

    public void setPhase(PhaseManager.Phase phase)
    {
        _currentPhase = phase;
    }

    public void setSonifyPosition(bool sonifyPosition)
    {
        _sonifyPosition = sonifyPosition;
    }

    public void setSonifyOrientation(bool sonifyOrientation)
    {
        _sonifyOrientation = sonifyOrientation;
    }


    public void setDrillingInformation(float screwLength, float drillingDepth)
    {
        _screwLength = screwLength;
        _drillingDepth = drillingDepth;

    }




    // ############# routines ###################



    // Start is called before the first frame update
    void Start()
    {
        _relaxSoundOn = false;
        _orientationConfirmed = false;
        _alertSoundOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        sonify();
        //Debug.Log(_currentPhase); --> works

    }



    // decide dependant on the phase how to sonify
    void sonify()
    {
        switch (_currentPhase)
        {
            case PhaseManager.Phase.Position:
                sonifyPosition();
                break;

            case PhaseManager.Phase.Orientation:
                sonifyOrientation();
                break;

            case PhaseManager.Phase.Depth:
                sonifyDepth();
                break;

            default:
                break;

        }




    }


    // #################### position phase ###################################


    void sonifyPosition()
    {
        if (_relaxSoundOn == false && (_sonifyPosition || _manual_screw_focus)) // no sound is played before and screw in focus for at least two seconds --> fade in relaxing sound
        {
            StartCoroutine(relaxAudioFadeIn());
        }
        else if (_relaxSoundOn == true && _sonifyPosition == false && _manual_screw_focus == false) // sound on, screw not in focus any more --> fade out immideately
        {
            StartCoroutine(relaxAudioFadeOut());
        }



    }

    private IEnumerator relaxAudioFadeIn()
    {
        //Debug.Log("Starting to wait two seconds");
        // wait for two seconds and check condition again
        yield return new WaitForSeconds(2);
        //Debug.Log("Waited two seconds");


        
        if (_relaxSoundOn == false && (_sonifyPosition || _manual_screw_focus))
        {
            
            Debug.Log("Activating relaxing sound");
            _relaxingSound.SetActive(true);
            _relaxAudio = _relaxingSound.GetComponent<AudioSource>();
            _relaxAudio.volume = 0;
            _relaxAudioLowPass = _relaxingSound.GetComponent<AudioLowPassFilter>();
            _relaxSoundOn = true;

            // fade in
            StartCoroutine(AudioFade(_relaxAudio, _fadingIn_time, _relax_volume_general));
            

        }
    }


    private IEnumerator relaxAudioFadeOut()
    {
        _relaxSoundOn = false;

        Debug.Log("Deactivate relaxing sound");
        
        // fade out
        StartCoroutine(AudioFade(_relaxAudio, _fadingOut_time, 0));

        //wait for _fading_time and set relaxaudiosource inactive then
        yield return new WaitForSeconds(_fadingOut_time);
        _relaxingSound.SetActive(false);
    }


    public static IEnumerator AudioFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }




    // #################### orientation phase ###################################

    void sonifyOrientation() //overlay relax sound with speech confirmation that tool is aligned correctly
    {
        if (_orientationConfirmed == false && (_sonifyOrientation || _manual_correct_orientation)) //orientation was not confirmed before
        {
            StartCoroutine(confirmOrientation());
        }
        else if (_orientationConfirmed == true && _sonifyOrientation == false && _manual_correct_orientation == false)
        {
            // reset flag for "orientation was confirmed once"
            _orientationConfirmed = false;
            _ConfirmOrientationSound.SetActive(false);
        }


    }

    private IEnumerator confirmOrientation()
    {
        // wait for two seconds and check condition again
        yield return new WaitForSeconds(2);
        Debug.Log("Correct Orientation was kept for two seconds");

        if (_orientationConfirmed == false && (_sonifyOrientation || _manual_correct_orientation))
        {
            _orientationConfirmed = true;

            Debug.Log("Play confirmation for correct orientation");
            //TODO play audio source of sonification manager
            _ConfirmOrientationSound.SetActive(true);






        }
    }



    // #################### depth phase ###################################
    void sonifyDepth() //occlude sound when tool enters spine, play alert sound when approaching end of screw guide
    {
        if (_manual_Depth_manipulation)
        {
            setDrillingInformation(_manual_screw_length, _manual_drilling_depth);
        }
        // relax sound must be acivated --> successfull activation in position phase assumed

        if(_screwLength >= 0 && _drillingDepth >= 0) //positive values indicate that screw is in focus and everything can be calculated
        {
            
            // occlude relax sound dependant on current drilling depth and make it more quiet
            float occlusion_ratio = _drillingDepth / (_occlusion_distance_factor * _screwLength);

            if (occlusion_ratio < 1) // tool is not considered as too deep inside spine
            {
                _relaxAudio.volume = _relax_volume_general - occlusion_ratio * (_relax_volume_general - _relax_volume_occluded);
                _relaxAudioLowPass.cutoffFrequency = 22000 - occlusion_ratio * (22000 - _occlusion_cut_off_frequency);
            }
            else if (occlusion_ratio > 1) // tool deep enough inside spine
            {
                _relaxAudio.volume = _relax_volume_occluded;
                _relaxAudioLowPass.cutoffFrequency = _occlusion_cut_off_frequency;
            }


            // activate alert sound when to near at end of screw

            float depth_ratio = _drillingDepth / _screwLength;

            if (depth_ratio >= _ratio_begin_of_critical_region) //consider e.g. 90% of screw lenght as beginning of critical
            {
                // activate alert sound object if not already on
                if(_alertSoundOn == false)
                {
                    _alertSound.SetActive(true);
                    _alertAudio = _alertSound.GetComponent<AudioSource>();
                    _alertSoundOn = true;
                }


                _alertAudio.volume = _alertAudio_min + ((depth_ratio - _ratio_begin_of_critical_region) / (1f - _ratio_begin_of_critical_region)) * (1f - _alertAudio_min);  


            }
            else if(depth_ratio >= 1) // to deep in spine, alert with max volumne
            {
                _alertAudio.volume = 1;
            }
            else // not critical --> deactivate alert sound
            {
                if(_alertSoundOn)
                {
                    _alertAudio.volume = 0;
                    _alertSound.SetActive(false);
                    _alertSoundOn = false;
                }

            }




        }
        else // no screw in focus and tool not in roi
        {
            // make sure no alarm is played
            _alertSound.SetActive(false);
            _alertSoundOn = false;

        }


    }



}
