using System;
using ThetaStar.Grid.Data;

namespace ThetaStar.Navigation.Data
{
    [Serializable]
    public struct StartEnd
    {
        public GridTarget Start;
        public GridTarget End;
    }
}
