using UnityEngine;

/// <summary>
/// Coloque este script em cada sprite do grupo de scroll.
///
/// Para Farback01 + Farback02 (complementares):
///   - Farback01: Position X = 0,   groupSize = 2, scrollSpeed = 2
///   - Farback02: Position X = [largura do sprite], groupSize = 2, scrollSpeed = 2
///   Quando um sai pela esquerda, pula 2 larguras à frente do outro.
///
/// Para Stars (camada separada, em frente aos farbacks):
///   - Stars_A: Position X = 0,   groupSize = 2, scrollSpeed = 1
///   - Stars_B: Position X = [largura do sprite], groupSize = 2, scrollSpeed = 1
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Tooltip("Velocidade de scroll (pixels/unidades por segundo)")]
    public float scrollSpeed = 2f;

    [Tooltip("Quantos sprites existem neste grupo. " +
             "Farback01+Farback02 = 2. Stars_A+Stars_B = 2.")]
    public int groupSize = 2;

    private float spriteWidth;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"[ParallaxBackground] {name} precisa de um SpriteRenderer!");
            return;
        }
        spriteWidth = sr.bounds.size.x;
    }

    void Update()
    {
        // Move para a esquerda
        transform.position -= new Vector3(scrollSpeed * Time.deltaTime, 0f, 0f);

        // Borda esquerda da câmera
        float camLeft = Camera.main.transform.position.x
                        - Camera.main.orthographicSize * Camera.main.aspect;

        // Quando o sprite saiu completamente pela esquerda,
        // pula (groupSize * spriteWidth) à frente → cai atrás do outro sprite do grupo
        if (transform.position.x + spriteWidth / 2f < camLeft)
            transform.position += new Vector3(groupSize * spriteWidth, 0f, 0f);
    }
}
