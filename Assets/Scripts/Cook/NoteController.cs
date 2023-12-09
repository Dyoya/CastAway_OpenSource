using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    CookController theTimingManager;
    [SerializeField] GameObject note = null;
    CookController theCookController;
    private AudioSource audioSource;
    private string charredFood = "½¡";
    private void Start()
    {
        theTimingManager = FindObjectOfType<CookController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            note.GetComponent<Note>().enabled = false;
            theTimingManager.CloseCookUI();
            theTimingManager.CookFood(theTimingManager.getSlotInt(), charredFood);
        }
    }

    public void CookButtonClicked()
    {
        if (theTimingManager.getFoodSlot() != 0)
        {
            audioSource = GetComponent<AudioSource>();
            if (note != null)
            {
                if (!note.GetComponent<Note>().enabled)
                {
                    note.GetComponent<Note>().enabled = true;
                    audioSource.Play();
                    theTimingManager.boxNote = note;
                }
                else
                {
                    theTimingManager.CheckTiming();
                    audioSource.Stop();
                    note.GetComponent<Note>().enabled = false;
                }
            }
        }     
    }
}
