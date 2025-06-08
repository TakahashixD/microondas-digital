using System;

namespace backend_microondas.Exceptions
{
    public class MicroondasException : Exception
    {
        public MicroondasException(){ }
        public MicroondasException(string texto) : base(texto) { }
        public MicroondasException(string texto, Exception e) : base(texto, e) { }
    }
}
