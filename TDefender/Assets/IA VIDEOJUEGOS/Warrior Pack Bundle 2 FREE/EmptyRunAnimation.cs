using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyRunAnimation : MonoBehaviour
{
    // Como la animacion llama a dos eventos es encesario capturarlos para que no surgan errores
    void FootR() { return; }
    void FootL() { return; }
    void Hit() {return;}
}
