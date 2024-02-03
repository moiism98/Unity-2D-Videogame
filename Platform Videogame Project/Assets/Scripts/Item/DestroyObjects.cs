using UnityEngine;

public class DestroyObjects : MonoBehaviour
{
    /// <summary>
    /// This method has to be attached to some gameObjects as the enemy death or collectables's effects which have to be destroyed after being used.
    /// </summary> <summary>
    /// 
    /// </summary>
    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}
