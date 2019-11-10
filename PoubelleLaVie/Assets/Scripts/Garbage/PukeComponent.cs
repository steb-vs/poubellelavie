using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PukeComponent : Garbage, ISpeedModifier
{
    public float SpeedModifier => 0.25f;
}
