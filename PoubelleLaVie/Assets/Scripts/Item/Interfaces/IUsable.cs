using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IUsable
{
    /// <summary>
    /// Gets the current object position.
    /// </summary>
    Vector3 Position { get; }

    /// <summary>
    /// Gets if the object is heavy.
    /// </summary>
    bool IsHeavy { get; }

    int Priority { get; }

    /// <summary>
    /// Trigger the object use behavior.
    /// </summary>
    /// <param name="sender"></param>
    void Use(GameObject sender);

    /// <summary>
    /// Triggers the object take behavior.
    /// </summary>
    /// <param name="sender"></param>
    bool Take(GameObject sender);

    /// <summary>
    /// Triggers the object drop behavior.
    /// </summary>
    /// <param name="sender"></param>
    bool Drop(GameObject sender);
}
