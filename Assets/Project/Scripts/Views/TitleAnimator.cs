using UnityEngine;
using TMPro;

public class TitleAnimator : MonoBehaviour
{
    private TMP_Text _textComponent;

    [Header("Animation Variables")]
    [SerializeField] private float _amplitude = 6f; 
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _letterOffset = 6f;

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    private void Update() 
    {
        AnimateMath();    
    }

    private void AnimateMath()
    {
        _textComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = _textComponent.textInfo;

        for(int i=0; i< textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // ignora espaços em branco e quebra de linha 
            if(!charInfo.isVisible) continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Obtém os vértices atuais da malha desta letra específica
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices; 

            // Calcula a curva Seno baseada no tempo
            float offsetY = Mathf.Sin((Time.time * _speed) + (i * _letterOffset)) * _amplitude; 
            Vector3 offsetVector = new Vector3(0, offsetY, 0);

            // Aplica o deslocamento aos 4 vértices que compõem o retângulo (quad) da letra
            destinationVertices[vertexIndex + 0] += offsetVector;
            destinationVertices[vertexIndex + 1] += offsetVector;
            destinationVertices[vertexIndex + 2] += offsetVector;
            destinationVertices[vertexIndex + 3] += offsetVector;
        }

        // Envia as novas posições de volta para o componente renderizar na tela 

        for(int i=0; i< textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            _textComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
