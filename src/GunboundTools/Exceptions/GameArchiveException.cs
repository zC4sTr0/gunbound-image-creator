using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GunboundTools.Exceptions
{
    public class GameArchiveException : Exception
    {
        /// <summary>
        /// Obtiene el código de la exceptión
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Lanza una excepción del tipo GameArchiveException
        /// </summary>
        /// <param name="message"></param>
        public GameArchiveException(string message)
            : base(message)
        {

        }
    }
}
