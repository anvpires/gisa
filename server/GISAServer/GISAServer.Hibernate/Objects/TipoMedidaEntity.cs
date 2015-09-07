using System;
using System.ComponentModel;
using Iesi.Collections;
using Iesi.Collections.Generic;
using GISAServer.Hibernate.Utils;
using GISAServer.Hibernate.Exceptions;

namespace GISAServer.Hibernate.Objects
{    
	/// <summary>
	/// An object representation of the TipoMedida table
	/// </summary>
	[Serializable]
	public partial class TipoMedidaEntity
	{
		private System.Int64 _Id;

		private System.String _Designacao;
		private readonly ISet<SFRDUFComponenteEntity> _FKSFRDUFComponenteTipoMedida = new HashedSet<SFRDUFComponenteEntity>();
		private readonly ISet<SFRDUFDescricaoFisicaEntity> _FKSFRDUFDescricaoFisicaTipoMedida1 = new HashedSet<SFRDUFDescricaoFisicaEntity>();
		private System.Boolean _IsDeleted;
		private System.Byte[] _Versao;

		public virtual System.String Designacao
		{
			get
			{
				return _Designacao;
			}
			set
			{
				if (value == null)
				{
					throw new NullReferenceException("Designacao must not be null.");
				}
				_Designacao = value;
			}
		}

		public virtual ISet<SFRDUFComponenteEntity> FKSFRDUFComponenteTipoMedida
		{
			get
			{
				return _FKSFRDUFComponenteTipoMedida;
			}
		}

		public virtual ISet<SFRDUFDescricaoFisicaEntity> FKSFRDUFDescricaoFisicaTipoMedida1
		{
			get
			{
				return _FKSFRDUFDescricaoFisicaTipoMedida1;
			}
		}

		public virtual System.Int64 Id
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


	}
}
