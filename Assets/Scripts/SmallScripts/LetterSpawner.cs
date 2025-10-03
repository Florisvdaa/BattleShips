using UnityEngine;

public class LetterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject letterPrefab;
    [SerializeField] private float letterScale;
    [SerializeField] private Vector3 letterRotation;

    private void Start()
    {
        SpawnLetter();
    }

    private void SpawnLetter()
    {
        GameObject letter = Instantiate(letterPrefab, this.transform);

        Vector3 scale = new Vector3(letterScale, letterScale, letterScale);

        letter.transform.Rotate(letterRotation);
        letter.transform.localScale = scale;
    }
}
