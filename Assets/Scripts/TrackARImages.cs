using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackARImages : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    public GameObject thingToInstantiate;
    public Dictionary<ARTrackedImage, GameObject> trackedObjects = new Dictionary<ARTrackedImage, GameObject>();

    void OnEnable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackedImagesChanged += OnChanged;
        else
            Debug.LogError("ARTrackedImageManager is not assigned.");
    }

    void OnDisable()
    {
        if (m_TrackedImageManager != null)
            m_TrackedImageManager.trackedImagesChanged -= OnChanged;
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            if (thingToInstantiate == null)
            {
                Debug.LogError("Prefab to instantiate is not assigned.");
                return;
            }

            GameObject g = Instantiate(thingToInstantiate);

            // Set world position and rotation
            g.transform.SetPositionAndRotation(newImage.transform.position, newImage.transform.rotation);

            // Set scale to match the image
            g.transform.localScale = new Vector3(newImage.size.x, 1f, newImage.size.y); // Use the actual image size

            if (!trackedObjects.ContainsKey(newImage))
            {
                trackedObjects.Add(newImage, g);
            }
        }

        // Handle updated images
        foreach (var updatedImage in eventArgs.updated)
        {
            if (trackedObjects.TryGetValue(updatedImage, out GameObject g))
            {
                g.transform.SetPositionAndRotation(updatedImage.transform.position, updatedImage.transform.rotation);
                g.transform.localScale = new Vector3(updatedImage.size.x, 1f, updatedImage.size.y);
            }
        }

        // Handle removed images
        foreach (var removedImage in eventArgs.removed)
        {
            if (trackedObjects.TryGetValue(removedImage, out GameObject g))
            {
                Destroy(g); // Destroy the associated GameObject
                trackedObjects.Remove(removedImage);
            }
        }
    }
}
