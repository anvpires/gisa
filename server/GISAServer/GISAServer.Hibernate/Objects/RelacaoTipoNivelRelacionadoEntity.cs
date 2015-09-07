using System;
using System.Collections.Generic;
using System.ComponentModel;
using Iesi.Collections;
using Iesi.Collections.Generic;
using GISAServer.Hibernate.Utils;
using GISAServer.Hibernate.Exceptions;

namespace GISAServer.Hibernate.Objects
{    
	/// <summary>
	/// An object representation of the RelacaoTipoNivelRelacionado table
	/// </summary>
	[Serializable]
	public partial class RelacaoTipoNivelRelacionadoEntity
	{
		private PairIdComponent _Id;

		private TipoNivelRelacionadoEntity _ID;
		private System.Boolean _IsDeleted;
		private TipoNivelRelacionadoEntity _Upper;
		private System.Byte[] _Versao;

		public virtual PairIdComponent Id
		{
			get
			{
				return _Id;
			}
			set
			{
				_Id = value;
			}
		}

		public virtual TipoNivelRelacionadoEntity ID
		{
			get
			{
				return _ID;
			}
			set
			{
				_ID = value;
			}
		}

		public virtual System.Boolean IsDeleted
		{
			get
			{
				return _IsDeleted;
			}
			set
			{
				_IsDeleted = value;
			}
		}

		public virtual TipoNivelRelacionadoEntity Upper
		{
			get
			{
				return _Upper;
			}
			set
			{
				_Upper = value;
			}
		}

		public virtual System.Byte[] Versao
		{
			get
			{
				return _Versao;
			}
			set
			{
				_Versao = value;
			}
		}


		protected bool Equals(RelacaoTipoNivelRelacionadoEntity entity)
		{
			if (entity == null) return false;
			if (!base.Equals(entity)) return false;
			if (!Equals(_Id, entity._Id)) return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as RelacaoTipoNivelRelacionadoEntity);
		}

		public override int GetHashCode()
		{
			int result = base.GetHashCode();
			result = 29*result + _Id.GetHashCode();
			return result;
		}

	}
}
