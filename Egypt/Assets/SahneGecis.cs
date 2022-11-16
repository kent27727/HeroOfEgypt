using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SahneGecis : MonoBehaviour
{
    public void SahneDegis(int sahne_id)
    {
        SceneManager.LoadScene(sahne_id);
    }
}
