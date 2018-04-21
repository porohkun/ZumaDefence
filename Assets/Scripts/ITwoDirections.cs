using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITwoDirections<T>
{
    T Next { get; set; }
    T Preview { get; set; }
}
