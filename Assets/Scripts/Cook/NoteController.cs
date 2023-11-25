using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    CookController theTimingManager;
    [SerializeField] GameObject note = null;

    private void Start()
    {
        theTimingManager = FindObjectOfType<CookController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            note.GetComponent<Note>().enabled = false;
        }
    }

    public void CookButtonClicked()
    {
        if (note != null)
        {
            if (!note.GetComponent<Note>().enabled)
            {
                note.GetComponent<Note>().enabled = true;
                theTimingManager.boxNote = note;
            }
            else
            {
                theTimingManager.CheckTiming();
                note.GetComponent<Note>().enabled = false;               
            }
        }
    }
}
