using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script manages an object pool to reuse GameObjects instead of creating and destroying them frequently.
// Use GetObject() to spawn an object at a location, and DestroyObject() to return it to the pool for reuse.
public abstract class GameObjectPool : MonoBehaviour
{
    [SerializeField]
    protected GameObject objectPrefab; // Object to spawn

    protected Stack<GameObject> objectPool = new Stack<GameObject>(); // Stack to contain the object

    public virtual GameObject GetObject() // Function to get the object
    {
        if(objectPool.Count > 0) // Check if there are any objects in the stack
        {
            GameObject obj = objectPool.Pop(); // Get the object from the stack

            obj.SetActive(true); // Set the object to active
            return obj; // Return the object
        }
        else return Instantiate(objectPrefab); // Instantiate a new object if it doesn't exist in the stack 
    }
    public virtual void ReturnObject(GameObject obj) // Function to destroy objects
    {
        objectPool.Push(obj); // Places object back into stack 
        obj.SetActive(false);
    }
}