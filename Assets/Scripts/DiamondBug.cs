using UnityEngine;

public class DiamondBug : MonoBehaviour
{

    public float speed = 0.08f;

    float randomDirection = 0;
    public void Update()
    {
        if (randomDirection == 0)
            randomDirection = Random.Range(0, 2) == 0 ? -1 : 1;
        this.transform.position += new Vector3(randomDirection * speed, 0);
    }

    void OnMouseDown()
    {
        Destroy(this.gameObject);
        for(int i = 0; i < 6; i++)
            Game.Instance.InstantiateAndMove(Game.Instance.diamondPrefab, this.transform.position);
        Game.Instance._userDatas.diamond += 5;
        Game.Instance.SaveData();   
    }
}
