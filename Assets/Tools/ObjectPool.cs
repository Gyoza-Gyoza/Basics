using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages an object pool to reuse GameObjects instead of creating and destroying them frequently.
// Use GetObject() to spawn an object at a location, and DestroyObject() to return it to the pool for reuse.
public abstract class ObjectPool : MonoBehaviour
{
    [SerializeField]
    protected GameObject objectPrefab; // Object to spawn

    protected Stack<GameObject> objectPool = new Stack<GameObject>(); // Stack to contain the object

    public virtual GameObject GetObject(Vector3 location) // Function to get the object
    {
        objectPool.TryPop(out GameObject obj); // Attempt to get the object

        if (obj == null) return Instantiate(objectPrefab, location, Quaternion.identity); // Instantiate a new object if it doesn't exist in the stack 
        else
        {
            obj.SetActive(true);
            obj.transform.position = location;
            return obj; // Returns the object from the stack
        }
    }
    public virtual void DestroyObject(GameObject objectToDestroy) // Function to destroy objects
    {
        objectPool.Push(objectToDestroy); // Places object back into stack 
        objectToDestroy.SetActive(false);
    }
}
