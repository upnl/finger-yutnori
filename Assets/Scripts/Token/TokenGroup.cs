using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenGroup : MonoBehaviour
{
    private List<Token> tokens;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Merges otherTokenGroup into this TokenGroup
    /// </summary>
    /// <param name="otherTokenGroup"></param>
    public void Merge(TokenGroup otherTokenGroup)
    {
        foreach (Token token in otherTokenGroup.tokens)
        {
            token.tokenGroup = this;
            tokens.Add(token);
        }
        Destroy(otherTokenGroup);
    }
}
