using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;


public class MicrophoneInput : MonoBehaviour
{
    private AudioClip micClip;
    private string micName;
    private int sampleWindow = 128;  // Sample size to analyze
    public Transform objectToResize;
    public Slider sliderToChange;


    void Start()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
        }
        else
        {
            // We do not have permission to use the microphone.
            // Ask for permission or proceed without the functionality enabled.
            Permission.RequestUserPermission(Permission.Microphone);

        }

        // Check if the device has a microphone

        if (Microphone.devices.Length > 0)
        {
            // Choose the first available microphone
            micName = Microphone.devices[0];

            // Start recording with the microphone (looping, no limit on duration)
            micClip = Microphone.Start(micName, true, 10, 44100);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        if (Microphone.IsRecording(micName))
        {
            // Get the amplitude of the current microphone input
            float amplitude = GetAmplitude();

            Debug.Log("Amplitude: " + amplitude);
            objectToResize.localScale = new Vector3(amplitude, amplitude, amplitude); //you could resize this object in one axis only ie. a rectangular bar or all of them.
            sliderToChange.value = amplitude;  //the slider will show the amplitude as its value. you can set the slider's max in the inspector if needed


        }
    }

    // Method to calculate the amplitude (RMS value)
    float GetAmplitude()
    {
        // Create an array to store the samples
        float[] samples = new float[sampleWindow];

        // Get the samples from the microphone input
        int micPosition = Microphone.GetPosition(micName) - sampleWindow + 1;
        if (micPosition < 0) return 0;
        micClip.GetData(samples, micPosition);

        // Calculate RMS (Root Mean Square) amplitude
        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / sampleWindow);  // Return the RMS value as amplitude
    }
}

