using System;
using System.Collections;
using System.Data;
using GISA.Model;

namespace GISA.Reports
{
	/// <summary>
	/// Um InventarioResumido � um Inventario sujeito a um grupo restrito 
	/// de n�veis documentais que s�o inicialmente passados como par�metros
	/// </summary>
	public class InventarioResumido : Inventario
	{
        public InventarioResumido(string FileName, bool isTopDown, long idTrustee) : base(FileName, isTopDown, idTrustee) { }

        public InventarioResumido(string FileName, ArrayList parameters, long idTrustee) : base(FileName, parameters, idTrustee) { }

        public InventarioResumido(string FileName, ArrayList parameters, bool isTopDown, long idTrustee) : base(FileName, parameters, isTopDown, idTrustee) { }
	}
}