using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent (typeof (ARTrackedImageManager))]
public class TrackedImages : MonoBehaviour
{
    [SerializeField] GameObject[] arObjectsToPlace;

    private ARTrackedImageManager m_TrackedImagemanager;

    private Dictionary<string, GameObject> arObjects = new Dictionary<string, GameObject>();
    // [SerializeField] GameObject instructions;

    void Awake() {
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
      m_TrackedImagemanager = GetComponent<ARTrackedImageManager> ();
        foreach(GameObject arObject in arObjectsToPlace) {
            GameObject newARObject = Instantiate(arObject, Vector3.zero, Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(newARObject.name, newARObject);
            newARObject.SetActive(false);
        }
    }

    void OnEnable() {
        m_TrackedImagemanager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable() {
        m_TrackedImagemanager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    
    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) {

        foreach(ARTrackedImage trackedImage in eventArgs.added) {
            UpdateARImage(trackedImage);
        }

        foreach(ARTrackedImage trackedImage in eventArgs.updated) {
            // UpdateARImage(trackedImage)
            if(trackedImage.trackingState == TrackingState.Tracking) {
                UpdateARImage(trackedImage);
                // instructions.SetActive(false);
            }
            else {
                arObjects[trackedImage.referenceImage.name].SetActive(false);
                // instructions.SetActive(true);
            }
        }

        foreach(ARTrackedImage trackedImage in eventArgs.removed) {
          arObjects[trackedImage.referenceImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage) {
        // Assing and Place Game Object
        AssingGameObject(trackedImage);
    }

    void AssingGameObject(ARTrackedImage rTrackedImage) {
        // string name = rTrackedImage.referenceImage.name;
        GameObject prefab = arObjects[rTrackedImage.referenceImage.name];
        prefab.transform.position = rTrackedImage.transform.position;
        prefab.transform.rotation = rTrackedImage.transform.rotation;
        prefab.SetActive(true);

        foreach(GameObject go in arObjects.Values) {
          if(go.name != rTrackedImage.referenceImage.name){
            go.SetActive(false);
          }
        }
    }
}
