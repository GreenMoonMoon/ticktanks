using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void Damage();
}

public interface IPowerable
{
    void Powerup(string powerUpType);
}

// Probably superfluous
public interface IDestuctible
{
    void Destruct();
}